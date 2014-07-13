using Mach.Kinect;
using Mach.Xna.Kinect.HandState;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Mach.Xna.Kinect.Components
{
    public class KinectCursor : DrawableGameComponent
    {
        private const int CursorPositionsBufferLenth = 1;
        
        private ContentManager _content;
        private bool leftHanded;
        private Vector2 _renderOffset;
        private float _scale;
        private SpriteFont _font;
        private Vector2[] cursorPositionsBuffer;
        private int cursorPositionsBufferIndex;
        private int frame;
        private float distanceTolerance = 1;
        private Vector2 lastHandPosition = new Vector2(0, 0);
        private double lastHandTime;
        private string _textToRender;
        private IHandStateTracker _handStateTracker;
        private bool _hideMouseCursorWhenHandTracked;
        private ICursorTracker _cursorTracker;
        private Vector2 cursorPosition;
        private int lastDepthFrameDataLength;
        private Texture2D _colorVideo;

        protected Texture2D _handTexture;
        protected SpriteBatch _spriteBatch;
        protected VisualKinectManager _KinectChooser;
        protected Skeletons _skeletons;
        protected bool _isHandTracked = false;

        public ICursorTracker CursorTracker
        {
            get { return _cursorTracker; }
            set { _cursorTracker = value; }
        }

        public IHandStateTracker HandStateTracker
        {
            get { return _handStateTracker; }
            set { _handStateTracker = value; }
        }
        public VideoStreamComponent VideoStreamData
        {
            get;
            set;
        }
        public bool HideMouseCursorWhenHandTracked
        {
            get { return _hideMouseCursorWhenHandTracked; }
            set { _hideMouseCursorWhenHandTracked = value; }
        }
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        public Vector2 RenderOffset
        {
            get { return _renderOffset; }
            set { _renderOffset = value; }
        }
        public bool IsHandClosed
        {
            get;
            protected set;
        }

        public bool IsHandPressed
        {
            get;
            protected set;
        }

        public Vector2 HandPosition
        {
            get { return new Vector2((int)cursorPosition.X, (int)cursorPosition.Y); }
        }

        public Vector3 HandRealPosition
        {
            get;
            set;
        }

        public SkeletonPoint HandRealPositionPoint
        {
            get;
            set;
        }
        
        public bool IsHandTracked
        {
            get { return _isHandTracked; }
        }

        public Texture2D HandTexture
        {
            get { return _handTexture; }
            set { _handTexture = value; }
        }

        public KinectCursor(Game game, VisualKinectManager chooser)
            : base(game)
        {
            _KinectChooser = chooser;
            _skeletons = chooser.Skeletons;
            _renderOffset = Vector2.Zero;
            _scale = 1;

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            cursorPositionsBuffer = new Vector2[CursorPositionsBufferLenth];
            cursorPositionsBufferIndex = -1;
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
            _cursorTracker = new RelativeCursorTracker(chooser.Manager);
        }

        public KinectCursor(Game game, ContentManager content, VisualKinectManager chooser)
            : base(game)
        {
            _KinectChooser = chooser;
            _skeletons = chooser.Skeletons;
            _renderOffset = Vector2.Zero;
            _scale = 1;

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            cursorPositionsBuffer = new Vector2[CursorPositionsBufferLenth];
            cursorPositionsBufferIndex = -1;
            _content = content;
            _cursorTracker = new RelativeCursorTracker(chooser.Manager);
        }

        protected override void LoadContent()
        {
            _handTexture = _content.Load<Texture2D>("Hand");
            _font = _content.Load<SpriteFont>("NormalFont");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _textToRender = string.Empty;

            if (_KinectChooser.Sensor != null)
            {
                if (_skeletons != null && _skeletons.TrackedSkeleton != null)
                {
                    leftHanded =
                        (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                        _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState != JointTrackingState.Tracked)
                        ||
                        (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                        _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position.Z < _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.Z);

                    bool isHandTracked;
                    Vector2 cursor = _cursorTracker.GetCursorPosition(_skeletons.TrackedSkeleton, leftHanded, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height, out isHandTracked);

                    var handPoint = _skeletons.TrackedSkeleton.Joints[leftHanded ? JointType.HandLeft : JointType.HandRight].Position;
                    HandRealPosition = new Vector3(handPoint.X, handPoint.Y, handPoint.Z);
                    HandRealPositionPoint = handPoint;

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
                _handStateTracker.Update(leftHanded, cursorPosition);
            }

            if (_skeletons.TrackedSkeleton != null)
            {
                // check for hand
                if (cursorPosition != Vector2.Zero)
                {
                    Vector2 handPosition = cursorPosition;

                    if (Vector2.Distance(handPosition, lastHandPosition) > 5)
                    {
                        lastHandTime = 0;
                    }
                    else
                    {
                        lastHandTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    lastHandPosition = handPosition;

                }

                var cursorPos = GetHarmonizedCursorPosition();

                // Avoid flickering
                if (cursorPos != Vector2.Zero)
                {
                    cursorPosition = cursorPos;
                }
            }
            else
            {
                // No tracked skeleton is present so we hide cursor.
                cursorPosition = Vector2.Zero;
            }

            base.Update(gameTime);
        }

         public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_textToRender != null)
            {
                Vector2 FontOrigin = _font.MeasureString(_textToRender) / 2;

                _spriteBatch.DrawString(_font, _textToRender, new Vector2(600, 20), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            if (cursorPosition != Vector2.Zero)
            {
                _spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }

            if (_handStateTracker != null)
            {
                _handStateTracker.Draw(gameTime, _spriteBatch, _font, _scale, _renderOffset);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void AddCursorPosition(Vector2 cursorPosition)
        {
            if (cursorPosition == Vector2.Zero)
                return;

            cursorPositionsBufferIndex = (cursorPositionsBufferIndex + 1) % cursorPositionsBuffer.Length;
            cursorPositionsBuffer[cursorPositionsBufferIndex] = cursorPosition;
        }

        private Vector2 GetHarmonizedCursorPosition()
        {
            if (cursorPositionsBufferIndex < 0) return Vector2.Zero;

            var valuesX = cursorPositionsBuffer.Select(p => p.X);

            if (valuesX.Count() < 1) return Vector2.Zero;

            var valuesY = cursorPositionsBuffer.Select(p => p.Y);

            float x = valuesX.Sum() / valuesX.Count();
            float y = valuesY.Sum() / valuesY.Count();

            return new Vector2(x, y);
        }

        private float GetDistanceBetweenJoints(Skeleton skeleton, JointType join1, JointType join2)
        {
            DepthImagePoint join1DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[join1].Position, _KinectChooser.Sensor.DepthStream.Format);
            DepthImagePoint join2DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[join2].Position, _KinectChooser.Sensor.DepthStream.Format);

            Vector2 joint1Position = new Vector2(join1DepthPoint.X, join1DepthPoint.Y);
            Vector2 joint2Position = new Vector2(join2DepthPoint.X, join2DepthPoint.Y);

            return Vector2.Distance(joint1Position, joint2Position);
        }
    }
}
