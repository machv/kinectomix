using AtomixData;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atomix.Components
{
    public class KinectCursor : DrawableGameComponent
    {
        const int CursorPositionsBufferLenth = 1;
        protected KinectChooser _KinectChooser;
        protected Skeletons _skeletons;
        SpriteBatch spriteBatch;
        bool leftHanded = true;
        Vector2 _renderOffset;
        float _scale;
        Texture2D _handTexture;
        Texture2D _pointTextures;
        SpriteFont font;
        Vector2[] cursorPositionsBuffer;
        int cursorPositionsBufferIndex;

        private IHandStateTracker _handTracker;

        public IHandStateTracker HandTracker
        {
            get { return _handTracker; }
            set { _handTracker = value; }
        }


        private AnimatedTexture _animatedHand;
        private const float Depth = 0.5f;
        public VideoStreamComponent VideoStreamData { get; set; }

        private bool _hideMouseCursorWhenHandTracked;
        public bool HideMouseCursorWhenHandTracked
        {
            get { return _hideMouseCursorWhenHandTracked; }
            set { _hideMouseCursorWhenHandTracked = value; }
        }

        /// <summary>
        /// Enables tracking of open/close hand.
        /// </summary>
        /// <returns></returns>
        public bool EnableHandStatusTracking { get; set; }

        public KinectCursor(Game game, KinectChooser chooser, Skeletons skeletons, Vector2 offset, float scale)
            : base(game)
        {
            _KinectChooser = chooser;
            _skeletons = skeletons;
            _renderOffset = offset;
            _scale = scale;

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            cursorPositionsBuffer = new Vector2[CursorPositionsBufferLenth];
            cursorPositionsBufferIndex = -1;

            _animatedHand = new AnimatedTexture(Vector2.Zero, 0, 0.25f, Depth);
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

        public bool IsHandClosed { get; protected set; }

        public bool IsHandPressed { get; protected set; }

        public Vector2 HandPosition { get { return new Vector2((int)cursorPosition.X, (int)cursorPosition.Y); } }

        public Vector3 HandRealPosition { get;set; }

        protected bool _isHandTracked = false;
        public bool IsHandTracked
        {
            get { return _isHandTracked; }
        }

        private const int Frames = 10;
        private const int FramesPerSec = 10;

        protected override void LoadContent()
        {
            _handTexture = Game.Content.Load<Texture2D>("Images/Hand");
            font = Game.Content.Load<SpriteFont>("Fonts/Normal");
            _pointTextures = Game.Content.Load<Texture2D>("Images/Joint");
            _animatedHand.Load(Game.Content, "HandAnimation", Frames, FramesPerSec);

            base.LoadContent();
        }

        Vector2 lastHandPosition = new Vector2(0, 0);
        double lastHandTime;

        DepthImagePoint _handDepthPoint;
        int[] _histogram;
        int _handRadius;

        Rectangle _handRect;
        string _textToRender;
        short[] lastDepthFrameData = null;

        int top;
        int left;
        int bottom;
        int right;

        public override void Update(GameTime gameTime)
        {
            _textToRender = string.Empty;

            if (_KinectChooser.Sensor != null)
            {
                if (_KinectChooser.SkeletonData != null)
                {
                    if (_skeletons.TrackedSkeleton != null)
                    {
                        leftHanded =
                            (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                            _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState != JointTrackingState.Tracked)
                            ||
                            (_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                            _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position.Z < _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.Z);

                        //cursorPosition = TrackHandMovementAbsolute(_skeletons.TrackedSkeleton);
                        bool isTracked;
                        var cursor = TrackHandMovementRelative(_skeletons.TrackedSkeleton, out isTracked);

                        var handPoint = _skeletons.TrackedSkeleton.Joints[leftHanded ? JointType.HandLeft : JointType.HandRight].Position;
                        HandRealPosition = new Vector3(handPoint.X, handPoint.Y, handPoint.Z);

                        AddCursorPosition(cursor);

                        if (cursor != Vector2.Zero)
                        {
                            _isHandTracked = true;

                            if (_hideMouseCursorWhenHandTracked)
                                Game.IsMouseVisible = false;
                        }
                        else if(!isTracked)
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
                }

                using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
                {
                    if (depthFrame != null)
                    {
                        // Create array for pixel data and copy it from the image frame
                        short[] pixelData = new short[depthFrame.PixelDataLength];
                        depthFrame.CopyPixelDataTo(pixelData);

                        lastDepthFrameData = pixelData;
                        lastDepthFrameDataLength = depthFrame.PixelDataLength;

                        if (_handTracker != null)
                        {
                            _handTracker.ProcessDepthData(depthFrame);
                        }
                    }
                }
            }

            if (_skeletons.TrackedSkeleton != null && _handTracker != null)
            {
                _handTracker.Update(leftHanded);
            }

            if (_skeletons.TrackedSkeleton != null)
            {
                // check for hand
                if (cursorPosition != Vector2.Zero)
                {
                    Vector2 handPosition = cursorPosition; // SkeletonToColorMap(_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position);

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
                    // animate only if on same place
                    if (Vector2.Distance(cursorPosition, cursorPos) > distanceTolerance)
                    {
                        _animatedHand.Reset();
                        _animatedHand.Play();
                        IsHandPressed = false;
                    }

                    cursorPosition = cursorPos;
                }


                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _animatedHand.UpdateFrame(elapsed);

                if (_animatedHand.Frame == Frames - 1)
                {
                    IsHandPressed = true;
                    _animatedHand.Pause();
                }
            }
            else
            {
                // No tracked skeleton is present so we hide cursor.
                cursorPosition = Vector2.Zero;
            }

            base.Update(gameTime);
        }

        int frame = 0;
        float distanceTolerance = 1;

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

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (_textToRender != null)
            {
                Vector2 FontOrigin = font.MeasureString(_textToRender) / 2;

                spriteBatch.DrawString(font, _textToRender, new Vector2(600, 20), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            if (_handTracker != null)
            {
                _handTracker.Draw(gameTime, spriteBatch, _scale, _renderOffset);
            }

            if (cursorPosition != Vector2.Zero)
            {
                spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);

                _animatedHand.DrawFrame(spriteBatch, cursorPosition);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        Vector2 cursorPosition;
        Vector2 cursorPositionInteraction;

        float xPrevious;
        float yPrevious;
        float MoveThreshold = 0.005f;

        float handX;
        float handY;
        private int lastDepthFrameDataLength;
        private Texture2D _colorVideo;

        private Vector2 TrackHandMovementAbsolute(Skeleton skeleton)
        {
            JointType hand = leftHanded ? JointType.HandLeft : JointType.HandRight;

            int width = GraphicsDevice.Viewport.Bounds.Width;
            int height = GraphicsDevice.Viewport.Bounds.Height;
            SkeletonPoint handPoint = skeleton.Joints[hand].Position;
            var colorPt = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(handPoint, _KinectChooser.Sensor.ColorStream.Format);
            double ratioX = (double)colorPt.X / _KinectChooser.Sensor.ColorStream.FrameWidth;
            double ratioY = (double)colorPt.Y / _KinectChooser.Sensor.ColorStream.FrameHeight;

            var cursor = new Vector2();
            cursor.X = (int)(width * ratioX);
            cursor.Y = (int)(height * ratioY);

            return cursor;
        }

        // http://stackoverflow.com/questions/12569706/how-to-use-skeletal-joint-to-act-as-cursor-using-bounds-no-gestures
        /// <summary>
        /// Shoulders = top of screen
        /// Hips = bottom of screen
        /// Left Should = left most on screen
        /// </summary>
        /// <param name="skeleton"></param>
        private Vector2 TrackHandMovementRelative(Skeleton skeleton)
        {
            bool isTracked;

            return TrackHandMovementRelative(skeleton, out isTracked);
        }

        private Vector2 TrackHandMovementRelative(Skeleton skeleton, out bool isHandTracked)
        {
            isHandTracked = false;

            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint rightHand = skeleton.Joints[JointType.HandRight];

            Joint hand = leftHanded ? leftHand : rightHand;

            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];

            Joint oppositeShoulder = leftHanded ? rightShoulder : leftShoulder;
            Joint sameShoulder = leftHanded ? leftShoulder : rightShoulder;
            Joint hip = leftHanded ? skeleton.Joints[JointType.HipLeft] : skeleton.Joints[JointType.HipRight];

            //Joint rightHip = skeleton.Joints[JointType.HipRight];
            Joint head = skeleton.Joints[JointType.Head];

            // the hand joint is tracked
            if (hand.TrackingState == JointTrackingState.Tracked)
            {
                // the hand is sufficiently in front of the shoulder
                if (sameShoulder.Position.Z - hand.Position.Z > 0.2)
                {
                    isHandTracked = true; 

                    float xScaled = (hand.Position.X - oppositeShoulder.Position.X) / ((sameShoulder.Position.X - oppositeShoulder.Position.X) * 2) * GraphicsDevice.Viewport.Bounds.Width;

                    if (leftHanded)
                        xScaled = GraphicsDevice.Viewport.Bounds.Width - xScaled;

                    float yScaled = _KinectChooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated ?
                        (hand.Position.Y - sameShoulder.Position.Y) / ((sameShoulder.Position.Y - head.Position.Y) / 2) * GraphicsDevice.Viewport.Bounds.Height :
                        (hand.Position.Y - sameShoulder.Position.Y) / (hip.Position.Y - sameShoulder.Position.Y) * GraphicsDevice.Viewport.Bounds.Height;

                    if (yScaled < 0) yScaled = 0;
                    if (yScaled > GraphicsDevice.Viewport.Bounds.Height) yScaled = GraphicsDevice.Viewport.Bounds.Height;

                    if (xScaled < 0) xScaled = 0;
                    if (xScaled > GraphicsDevice.Viewport.Bounds.Width) xScaled = GraphicsDevice.Viewport.Bounds.Width;

                    // the hand has moved enough to update screen position (jitter control / smoothing)
                    if (Math.Abs(hand.Position.X - xPrevious) > MoveThreshold || Math.Abs(hand.Position.Y - yPrevious) > MoveThreshold)
                    {
                        handX = xScaled;
                        handY = yScaled;

                        xPrevious = hand.Position.X;
                        yPrevious = hand.Position.Y;

                        return new Vector2(handX, handY);
                    }
                }
            }

            // As fallback return zero position.
            return Vector2.Zero;
        }
    }
}
