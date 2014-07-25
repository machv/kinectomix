using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Kinect;
using Mach.Kinect.Gestures;
using Mach.Kinect;
using Mach.Xna.Kinect.Gestures;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Recognizes gestures against prerecorded gestures.
    /// </summary>
    public class Gestures : GameComponent
    {
        private static IEqualityComparer<RecognizedGesture> _gesturesComparer = new RecognizedGestureComparer();
        private static Gestures _instance;

        private int _minimalFramesToProcess = 4;
        private KinectManager _kinectManager;
        private Recognizer _recognizer;
        private string _gesturesLocation;
        private RecognizedGesture _recognizedGesture;
        private KnownGestures _knownGestures;
        private bool _isUpdating;
        private bool _isEven;
        private ConcurrentQueue<Skeleton> _pendingSkeletons;
        private ConcurrentQueue<RecognizedGesture> _recognizedGestures;

        /// <summary>
        /// Gets or sets the minimal frames to process recognizing of gesture.
        /// </summary>
        /// <value>
        /// The minimal frames to process.
        /// </value>
        public int MinimalFramesToProcess
        {
            get { return _minimalFramesToProcess; }
            set
            {
                if (value > 0)
                {
                    _minimalFramesToProcess = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gestures"/> class.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Kinect Manager used for tracking skeletons.</param>
        /// <param name="gesturesLocation">Path to prerecorded gestures.</param>
        public Gestures(Game game, KinectManager kinectManager, string gesturesLocation)
            : base(game)
        {
            _instance = this;

            _kinectManager = kinectManager;
            _recognizer = new Recognizer();
            _gesturesLocation = gesturesLocation;
            _pendingSkeletons = new ConcurrentQueue<Skeleton>();
            _recognizedGestures = new ConcurrentQueue<RecognizedGesture>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gestures" /> class.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Kinect Manager used for tracking skeletons.</param>
        /// <param name="gestures">The gestures with initialized instances.</param>
        /// <exception cref="System.InvalidOperationException">Prepared gestures have to have Instance ready.</exception>
        public Gestures(Game game, KinectManager kinectManager, KnownGesture[] gestures)
            : base(game)
        {
            // Check that all gestures are initialized
            foreach (KnownGesture gesture in gestures)
            {
                if (gesture.Instance == null)
                {
                    throw new System.InvalidOperationException("Prepared gestures have to have Instance ready.");
                }
            }

            _instance = this;

            _kinectManager = kinectManager;
            _recognizer = new Recognizer();
            _knownGestures = new KnownGestures(gestures);
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

            if (_knownGestures == null)
            {
                KnownGesture[] knownGestures = Game.Content.Load<KnownGesture[]>("Gestures");
                _knownGestures = new KnownGestures(knownGestures);
            }

            foreach (KnownGesture knownGesture in _knownGestures)
            {
                if (knownGesture.Instance == null)
                {
                    using (Stream stream = TitleContainer.OpenStream(_gesturesLocation + knownGesture.Filename))
                    {
                        Gesture gesture = seralizer.Deserialize(stream) as Gesture;

                        knownGesture.Instance = gesture;
                        _recognizer.AddGesture(gesture);
                    }
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
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_kinectManager.Skeletons != null)
            {
                if (_isEven)
                {
                    if (_kinectManager.Skeletons.TrackedSkeleton != null)
                        _pendingSkeletons.Enqueue(_kinectManager.Skeletons.TrackedSkeleton);
                }
                _isEven = !_isEven;

                if (_isUpdating == false && _pendingSkeletons.Count > _minimalFramesToProcess)
                {
                    Task.Factory.StartNew(() => ProcessRecognizing());
                }
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
