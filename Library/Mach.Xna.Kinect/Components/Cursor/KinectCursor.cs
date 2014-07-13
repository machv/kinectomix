using Mach.Kinect;
using Mach.Xna.Kinect.HandState;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Cursor for Kinect sensor.
    /// </summary>
    public class KinectCursor : DrawableGameComponent
    {
        private int _cursorPositionsBufferLength = 1;
        private ContentManager _content;
        private bool _leftHanded;
        private Vector2 _renderOffset;
        private float _scale;
        private SpriteFont _font;
        private Vector2[] _cursorPositionsBuffer;
        private int _cursorPositionsBufferIndex;
        private IHandStateTracker _handStateTracker;
        private bool _hideMouseCursorWhenHandTracked;
        private bool _setMouseCursorLocation;
        private ICursorMapper _cursorMapper;
        private Vector2 _cursorPosition;
        private SkeletonPoint _handRealPosition;
        private bool _isHandTracked;
        private Texture2D _handTexture;
        private VisualKinectManager _KinectChooser;
        private Skeletons _skeletons;

        /// <summary>
        /// <see cref="SpriteBatch"/> for rendering output.
        /// </summary>
        protected SpriteBatch _spriteBatch;

        /// <summary>
        /// Gets or sets render scale for displayed information.
        /// </summary>
        /// <returns>Render offset for displayed information.</returns>
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        /// <summary>
        /// Gets or sets render offset for displayed information.
        /// </summary>
        /// <returns>Render offset for displayed information.</returns>
        public Vector2 RenderOffset
        {
            get { return _renderOffset; }
            set { _renderOffset = value; }
        }
        /// <summary>
        /// Gets or sets length of the cursor buffer.
        /// </summary>
        /// <returns>Length of the cursor buffer.</returns>
        public int CursorPositionsBufferLength
        {
            get { return _cursorPositionsBufferLength; }
            set
            {
                if (_cursorPositionsBufferLength != value)
                {
                    int lengthToCopy = Math.Min(_cursorPositionsBufferLength, value);
                    Vector2[] newCursorPositionsBuffer = new Vector2[value];

                    Array.Copy(_cursorPositionsBuffer, newCursorPositionsBuffer, lengthToCopy);

                    if (_cursorPositionsBufferIndex >= value)
                        _cursorPositionsBufferIndex = value - 1;

                    _cursorPositionsBuffer = newCursorPositionsBuffer;
                    _cursorPositionsBufferLength = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets mapping of the cursor to the screen.
        /// </summary>
        /// <returns>Mapping of the cursor.</returns>
        public ICursorMapper CursorMapper
        {
            get { return _cursorMapper; }
            set { _cursorMapper = value; }
        }
        /// <summary>
        /// Gets or sets <see cref="IHandStateTracker"/> checking state of the hand.
        /// </summary>
        /// <returns><see cref="IHandStateTracker"/> checking state of the hand.</returns>
        public IHandStateTracker HandStateTracker
        {
            get { return _handStateTracker; }
            set { _handStateTracker = value; }
        }
        /// <summary>
        /// Gets or sets if OS cursor should be hidden when hand is tracked.
        /// </summary>
        /// <returns>True when OS cursor will be hidden when hand is tracked.</returns>
        public bool HideSystemCursorWhenHandTracked
        {
            get { return _hideMouseCursorWhenHandTracked; }
            set { _hideMouseCursorWhenHandTracked = value; }
        }
        /// <summary>
        /// Gets or sets if location of OS mouse cursor should be updated by this component.
        /// </summary>
        /// <returns>If true location of OS mouse cursor is updated by this component.</returns>
        public bool UpdateSystemCursorPosition
        {
            get { return _setMouseCursorLocation; }
            set { _setMouseCursorLocation = value; }
        }
        /// <summary>
        /// Gets if tracked hand state is active.
        /// </summary>
        /// <returns>True if tracked hand state is active.</returns>
        public bool IsHandStateActive
        {
            get
            {
                if (_handStateTracker != null)
                    return _handStateTracker.IsStateActive;

                return false;
            }
        }
        /// <summary>
        /// Gets position of the cursor on the screen.
        /// </summary>
        /// <returns>Position of the cursor on the screen.</returns>
        public Vector2 CursorPosition
        {
            get { return _cursorPosition; }
        }
        /// <summary>
        /// Gets position of tracked hand in real world coordinate space.
        /// </summary>
        /// <returns>Position of tracked hand in real world coordinate space.</returns>
        public SkeletonPoint HandRealPosition
        {
            get { return _handRealPosition; }
        }
        /// <summary>
        /// Gets if hand is currently tracked for the cursor.
        /// </summary>
        /// <returns></returns>
        public bool IsHandTracked
        {
            get { return _isHandTracked; }
        }
        /// <summary>
        /// Gets or sets texture that is rendered on the cursor position.
        /// </summary>
        /// <returns>Texture that is rendered on the cursor position.</returns>
        public Texture2D Texture
        {
            get { return _handTexture; }
            set { _handTexture = value; }
        }

        /// <summary>
        /// Creates new instance of <see cref="KinectCursor"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Manager handling connected sensor.</param>
        public KinectCursor(Game game, VisualKinectManager kinectManager)
            : base(game)
        {
            _KinectChooser = kinectManager;
            _skeletons = kinectManager.Skeletons;
            _renderOffset = Vector2.Zero;
            _scale = 1;

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            _cursorPositionsBuffer = new Vector2[_cursorPositionsBufferLength];
            _cursorPositionsBufferIndex = -1;
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
            _cursorMapper = new RelativeCursorMapper(kinectManager.Manager);
        }

        /// <summary>
        /// Creates new instance of <see cref="KinectCursor"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Manager handling connected sensor.</param>
        /// <param name="content">ContentManager containing required assets.</param>
        public KinectCursor(Game game, ContentManager content, VisualKinectManager kinectManager)
            : base(game)
        {
            _KinectChooser = kinectManager;
            _skeletons = kinectManager.Skeletons;
            _renderOffset = Vector2.Zero;
            _scale = 1;

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            _cursorPositionsBuffer = new Vector2[_cursorPositionsBufferLength];
            _cursorPositionsBufferIndex = -1;
            _content = content;
            _cursorMapper = new RelativeCursorMapper(kinectManager.Manager);
        }

        /// <summary>
        /// Loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            _handTexture = _content.Load<Texture2D>("Hand");
            _font = _content.Load<SpriteFont>("NormalFont");

            base.LoadContent();
        }

        /// <summary>
        /// Updates position of the cursor.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_KinectChooser.Sensor != null)
            {
                if (_skeletons != null && _skeletons.TrackedSkeleton != null)
                {
                    _leftHanded =
                        (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                        _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState != JointTrackingState.Tracked)
                        ||
                        (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                        _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position.Z < _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.Z);

                    bool isHandTracked;
                    Vector2 cursor = _cursorMapper.GetCursorPosition(_skeletons.TrackedSkeleton, _leftHanded, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height, out isHandTracked);

                    _handRealPosition = _skeletons.TrackedSkeleton.Joints[_leftHanded ? JointType.HandLeft : JointType.HandRight].Position;

                    AddCursorPosition(cursor);

                    if (cursor != Vector2.Zero)
                    {
                        _isHandTracked = true;

                        if (_hideMouseCursorWhenHandTracked)
                            Game.IsMouseVisible = false;
                    }
                    else if (!isHandTracked)
                    {
                        _isHandTracked = false;

                        if (_hideMouseCursorWhenHandTracked)
                            Game.IsMouseVisible = true;
                    }
                }
                else
                {
                    _isHandTracked = false;

                    if (_hideMouseCursorWhenHandTracked)
                        Game.IsMouseVisible = true;
                }

                if (_handStateTracker != null)
                {
                    using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
                    {
                        if (depthFrame != null)
                        {
                            _handStateTracker.ProcessDepthData(depthFrame);
                        }
                    }

                    using (SkeletonFrame skeletonFrame = _KinectChooser.Sensor.SkeletonStream.OpenNextFrame(0))
                    {
                        if (skeletonFrame != null)
                        {
                            _handStateTracker.ProcessSkeletonData(skeletonFrame);
                        }
                    }
                }
            }

            if (_skeletons.TrackedSkeleton != null && _handStateTracker != null)
            {
                _handStateTracker.Update(_leftHanded, _cursorPosition);
            }

            if (_skeletons.TrackedSkeleton != null)
            {
                Vector2 cursorPos = GetHarmonizedCursorPosition();

                // Avoid flickering
                if (cursorPos != Vector2.Zero)
                {
                    _cursorPosition = cursorPos;

                    if (_setMouseCursorLocation)
                    {
                        Mouse.SetPosition((int)_cursorPosition.X, (int)_cursorPosition.Y);
                    }
                }
            }
            else
            {
                // No tracked skeleton is present so we hide cursor.
                _cursorPosition = Vector2.Zero;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws current position of the cursor.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_cursorPosition != Vector2.Zero && _handTexture != null)
            {
                _spriteBatch.Draw(_handTexture, _cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }

            if (_handStateTracker != null)
            {
                _handStateTracker.Draw(gameTime, _spriteBatch, _font, _scale, _renderOffset);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds position of the cursor to the position buffer.
        /// </summary>
        /// <param name="cursorPosition">Cursor position.</param>
        protected void AddCursorPosition(Vector2 cursorPosition)
        {
            if (cursorPosition == Vector2.Zero)
                return;

            _cursorPositionsBufferIndex = (_cursorPositionsBufferIndex + 1) % _cursorPositionsBuffer.Length;
            _cursorPositionsBuffer[_cursorPositionsBufferIndex] = cursorPosition;
        }

        private Vector2 GetHarmonizedCursorPosition()
        {
            if (_cursorPositionsBufferIndex < 0) return Vector2.Zero;

            var valuesX = _cursorPositionsBuffer.Select(p => p.X);

            if (valuesX.Count() < 1) return Vector2.Zero;

            var valuesY = _cursorPositionsBuffer.Select(p => p.Y);

            float x = valuesX.Sum() / valuesX.Count();
            float y = valuesY.Sum() / valuesY.Count();

            return new Vector2(x, y);
        }
    }
}
