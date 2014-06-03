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
using System.Threading;

namespace Atomix.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Gestures : GameComponent
    {
        private static IEqualityComparer<RecognizedGesture> _gesturesComparer = new RecognizedGestureComparer();
        private static Gestures _instance;
        public static GesturesState GetState()
        {
            return new GesturesState(_instance.GetRecognizedGestures(), _instance._knownGestures);
        }

        private Skeletons _skeletons;
        private Recognizer _recognizer;
        private string _gesturesLocation;
        private RecognizedGesture _recognizedGesture;
        private KnownGestures _knownGestures;
        //private Thread _processingThread;

        public Gestures(Game game, Skeletons skeletons, string gesturesLocation)
            : base(game)
        {
            _instance = this;

            _skeletons = skeletons;
            _recognizer = new Recognizer();
            _gesturesLocation = gesturesLocation;
            //_processingThread = new Thread(ProcessRecognizingWorker);
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

            //_processingThread.Start();
            _recognizer.MinimalBufferLength = 35;
            _recognizer.Start();

            base.Initialize();
        }

        private const int MinimalFramesToProcess = 4;
        private readonly object _locker = new object();
        private bool _isUpdating = false;
        private ConcurrentQueue<Skeleton> _pendingSkeletons = new ConcurrentQueue<Skeleton>();
        private ConcurrentQueue<RecognizedGesture> _recognizedGestures = new ConcurrentQueue<RecognizedGesture>();

        public IEnumerable<RecognizedGesture> GetRecognizedGestures()
        {
            int count = _recognizedGestures.Count;
            if (count > 0)
            {
                RecognizedGesture[] gestures = new RecognizedGesture[count];
                for (int i = 0; i < count; i++)
                {
                    RecognizedGesture gesture;
                    if (_recognizedGestures.TryDequeue(out gesture))
                    {
                        gestures[i] = gesture;
                    }
                }

                return gestures.Where(g => g != null).Distinct(_gesturesComparer);
            }

            return null;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (_skeletons.TrackedSkeleton != null)
                _pendingSkeletons.Enqueue(_skeletons.TrackedSkeleton);

            if (_isUpdating == false && _pendingSkeletons.Count > MinimalFramesToProcess)
            {
                Task.Factory.StartNew(() => ProcessRecognizing());
            }

            base.Update(gameTime);
        }

        //public void ProcessRecognizingWorker()
        //{
        //    while (true)
        //    {
        //        ProcessRecognizing();

        //        Thread.Sleep(1);
        //    }
        //}

        public void ProcessRecognizing()
        {
            if (_pendingSkeletons.Count == 0)
                return;

            _isUpdating = true;

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

            _isUpdating = false;
        }
    }
}
