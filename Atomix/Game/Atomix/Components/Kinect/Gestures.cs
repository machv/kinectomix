using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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
    /// Recognizes gestures against prerecorded gestures.
    /// </summary>
    public class Gestures : GameComponent
    {
        private static IEqualityComparer<RecognizedGesture> _gesturesComparer = new RecognizedGestureComparer();
        private static Gestures _instance;

        private const int MinimalFramesToProcess = 4;
        private Skeletons _skeletons;
        private Recognizer _recognizer;
        private string _gesturesLocation;
        private RecognizedGesture _recognizedGesture;
        private KnownGestures _knownGestures;
        private bool _isUpdating;
        private bool _isEven;
        private ConcurrentQueue<Skeleton> _pendingSkeletons;
        private ConcurrentQueue<RecognizedGesture> _recognizedGestures;

        /// <summary>
        /// Creates new instance of gestures component.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="skeletons">Skeletons tracked from the Kinect sensor.</param>
        /// <param name="gesturesLocation">Path to prerecorded gestures.</param>
        public Gestures(Game game, Skeletons skeletons, string gesturesLocation)
            : base(game)
        {
            _instance = this;

            _skeletons = skeletons;
            _recognizer = new Recognizer();
            _gesturesLocation = gesturesLocation;
            _pendingSkeletons = new ConcurrentQueue<Skeleton>();
            _recognizedGestures = new ConcurrentQueue<RecognizedGesture>();
        }

        /// <summary>
        /// Gets the current state of the gestures recognizer, including known gestures and recognized gestures.
        /// </summary>
        /// <returns>Current state of the gestures recognizer.</returns>
        public static GesturesState GetState()
        {
            return new GesturesState(_instance.GetRecognizedGestures(), _instance._knownGestures);
        }

        /// <summary>
        /// Initializes gestures component.
        /// </summary>
        public override void Initialize()
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Gesture));
            KnownGesture[] knownGestures = Game.Content.Load<KnownGesture[]>("Gestures");

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

        /// <summary>
        /// Gets recognized gestures from recognizer pipeline.
        /// </summary>
        /// <returns>Recognized gestures.</returns>
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
        /// Processes new skeleton into the recognizer.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (_isEven)
            {
                if (_skeletons.TrackedSkeleton != null)
                    _pendingSkeletons.Enqueue(_skeletons.TrackedSkeleton);
            }
            _isEven = !_isEven;

            if (_isUpdating == false && _pendingSkeletons.Count > MinimalFramesToProcess)
            {
                Task.Factory.StartNew(() => ProcessRecognizing());
            }

            base.Update(gameTime);
        }

        private void ProcessRecognizing()
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
