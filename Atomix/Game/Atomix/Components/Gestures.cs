using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Kinectomix.Logic.Gestures;
using System.Xml.Serialization;
using System.IO;
using Kinectomix.Logic.Game;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Kinect;

namespace Atomix.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Gestures : GameComponent
    {
        private static Gestures _instance;
        public static GesturesState GetState()
        {
            return new GesturesState(_instance._recognizedGesture, _instance._knownGestures);
        }

        private Skeletons _skeletons;
        private Recognizer _recognizer;
        private string _gesturesLocation;
        private RecognizedGesture _recognizedGesture;
        private KnownGestures _knownGestures;

        public Gestures(Game game, Skeletons skeletons, string gesturesLocation)
            : base(game)
        {
            _instance = this;

            _skeletons = skeletons;
            _recognizer = new Recognizer();
            _gesturesLocation = gesturesLocation;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Gesture));

            var knownGestures = Game.Content.Load<KnownGesture[]>("Gestures");
            _knownGestures = new KnownGestures(knownGestures);

            foreach (KnownGesture knownGesture in _knownGestures)
            {
                using (Stream stream = TitleContainer.OpenStream(_gesturesLocation + knownGesture.Filename))
                {
                    Gesture gesture = seralizer.Deserialize(stream) as Gesture;

                    knownGesture.Instance = gesture;
                    _recognizer.AddGesture(gesture);
                }
            }

            _recognizer.MinimalBufferLength = 35;
            _recognizer.Start();

            base.Initialize();
        }

        private readonly object _locker = new object();
        private bool _isUpdating = false;
        private ConcurrentQueue<Skeleton> _pendingSkeletons = new ConcurrentQueue<Skeleton>();
        private ConcurrentQueue<RecognizedGesture> _recognizedGestures = new ConcurrentQueue<RecognizedGesture>();

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            lock (_locker)
            {
                if (_isUpdating == false && _pendingSkeletons.Count > 0)
                {
                    Task.Factory.StartNew(() => ProcessRecognizing());
                }
                else
                {
                    _pendingSkeletons.Enqueue(_skeletons.TrackedSkeleton);
                }
            }

            base.Update(gameTime);
        }

        public void ProcessRecognizing()
        {
            if (_pendingSkeletons.Count == 0)
                return;

            lock (_locker)
            {
                _isUpdating = true;
            }

            Skeleton[] skeletons = new Skeleton[_pendingSkeletons.Count];
            for (int i = 0; i < skeletons.Length; i++)
            {
                Skeleton skeleton;
                if (_pendingSkeletons.TryDequeue(out skeleton))
                    skeletons[i] = skeleton;
            }

            foreach (Skeleton skeleton in skeletons)
            {
                _recognizer.ProcessSkeleton(skeleton);
                if (_recognizer.RecognizedGesture != null)
                {
                    _recognizedGestures.Enqueue(_recognizer.RecognizedGesture);
                }
            }

            lock (_locker)
            {
                _isUpdating = false;
            }
        }
    }
}
