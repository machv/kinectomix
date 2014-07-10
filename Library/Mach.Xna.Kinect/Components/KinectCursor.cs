﻿using Mach.Kinect;
using Mach.Xna.Kinect.HandState;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Mach.Xna.Kinect.Components
{
    public enum HandMovementTracking
    {
        Relative,
        Absolute,
    }

    public class KinectCursor : DrawableGameComponent
    {
        private const int CursorPositionsBufferLenth = 1;
        protected SpriteBatch _spriteBatch;
        private bool leftHanded;
        private Vector2 _renderOffset;
        private float _scale;
        protected Texture2D _handTexture;
        private SpriteFont _font;
        private Vector2[] cursorPositionsBuffer;
        private int cursorPositionsBufferIndex;
        private int frame;
        private float distanceTolerance = 1;
        private Vector2 lastHandPosition = new Vector2(0, 0);
        private double lastHandTime;
        private string _textToRender;
        private IHandStateTracker _handTracker;
        private bool _hideMouseCursorWhenHandTracked;
        private ContentManager _content;

        Vector2 cursorPosition;
        Vector2 cursorPositionInteraction;

        delegate Vector2 TrackingFunctionDelegate(Skeleton skeleton);

        TrackingFunctionDelegate _handTracking;

        private HandMovementTracking _trackingType;
        public HandMovementTracking HandTracking
        {
            get { return _trackingType; }
            set { _trackingType = value; }
        }

        float xPrevious;
        float yPrevious;
        float MoveThreshold = 0.005f;

        float handX;
        float handY;
        private int lastDepthFrameDataLength;
        private Texture2D _colorVideo;


        protected VisualKinectManager _KinectChooser;
        protected Skeletons _skeletons;

        public IHandStateTracker HandTracker
        {
            get { return _handTracker; }
            set { _handTracker = value; }
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
        public bool IsHandClosed { get; protected set; }

        public bool IsHandPressed { get; protected set; }

        public Vector2 HandPosition { get { return new Vector2((int)cursorPosition.X, (int)cursorPosition.Y); } }

        public Vector3 HandRealPosition { get; set; }

        public SkeletonPoint HandRealPositionPoint { get; set; }

        protected bool _isHandTracked = false;
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

                    //cursorPosition = TrackHandMovementAbsolute(_skeletons.TrackedSkeleton);
                    bool isTracked;
                    Vector2 cursor = TrackHandMovementRelative(_skeletons.TrackedSkeleton, out isTracked);

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
                    else if (!isTracked)
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


                if (_handTracker != null)
                {
                    using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
                    {
                        if (depthFrame != null)
                        {
                            _handTracker.ProcessDepthData(depthFrame);
                        }
                    }

                    using (SkeletonFrame skeletonFrame = _KinectChooser.Sensor.SkeletonStream.OpenNextFrame(0))
                    {
                        if (skeletonFrame != null)
                        {
                            _handTracker.ProcessSkeletonData(skeletonFrame);
                        }
                    }
                }
            }

            if (_skeletons.TrackedSkeleton != null && _handTracker != null)
            {
                _handTracker.Update(leftHanded, cursorPosition);
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

            if (_handTracker != null)
            {
                _handTracker.Draw(gameTime, _spriteBatch, _font, _scale, _renderOffset);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }



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

            Joint centerShoulder = skeleton.Joints[JointType.ShoulderCenter];
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

                    float xScaled = (hand.Position.X - centerShoulder.Position.X) / ((sameShoulder.Position.X - centerShoulder.Position.X) * 2) * GraphicsDevice.Viewport.Bounds.Width;

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
