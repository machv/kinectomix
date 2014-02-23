using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components
{
    public class KinectCursor : DrawableGameComponent
    {
        KinectChooser _KinectChooser;
        Skeletons _skeletons;
        SpriteBatch spriteBatch;
        bool leftHanded = false;
        Vector2 _kinectDebugOffset;
        float _scale;
        Texture2D _handTexture;
        SpriteFont font;

        public KinectCursor(Game game, KinectChooser chooser, Skeletons skeletons, Vector2 offset, float scale)
            : base(game)
        {
            _KinectChooser = chooser;
            _skeletons = skeletons;
            _kinectDebugOffset = offset;
            _scale = scale;

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        protected override void LoadContent()
        {
            _handTexture = Game.Content.Load<Texture2D>("Images/Hand");
            font = Game.Content.Load<SpriteFont>("Fonts/Normal");

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

        public override void Update(GameTime gameTime)
        {
            _textToRender = string.Empty;

            if (_KinectChooser.Sensor != null)
            {
                using (SkeletonFrame skeletonFrame = _KinectChooser.Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    if (skeletonFrame != null)
                    {
                        Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                        //Copy the skeleton data to our array
                        skeletonFrame.CopySkeletonDataTo(skeletonData);

                        _skeletons.Items = skeletonData;

                        if (_skeletons.TrackedSkeleton != null)
                        {
                            int width = GraphicsDevice.Viewport.Bounds.Width;
                            int height = GraphicsDevice.Viewport.Bounds.Height;

                            // Relative mapping of cursor
                            // Based on idea at http://stackoverflow.com/questions/13313005/kinect-sdk-1-6-and-joint-scaleto-method
                            Joint rightHand = _skeletons.TrackedSkeleton.Joints[JointType.HandRight];
                            Joint head = _skeletons.TrackedSkeleton.Joints[JointType.Head];
                            Joint leftShoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderLeft];
                            Joint rightShoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderRight];
                            Joint rightHip = _skeletons.TrackedSkeleton.Joints[JointType.HipRight];
                            double xScaled = (rightHand.Position.X - leftShoulder.Position.X) / ((rightShoulder.Position.X - leftShoulder.Position.X) * 2) * width;

                            double yScaled;
                            if (_KinectChooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated)
                            {
                                // In seated mode isn't hip tracked, so we have to replace this positions
                                yScaled = (rightHand.Position.Y - head.Position.Y) / (rightHand.Position.X - leftShoulder.Position.X) * height;
                            }
                            else
                            {
                                yScaled = (rightHand.Position.Y - head.Position.Y) / (rightHip.Position.Y - head.Position.Y) * height;
                            }

                            cursorPosition = new Vector2();
                            cursorPosition.X = (int)xScaled; // (int)(width * ratioX);
                            cursorPosition.Y = (int)yScaled; // (int)(height * ratioY);

                            //Vector2 pos = TrackHandMovementAbsolute(_skeletons.TrackedSkeleton);

                            TrackHandMovementRelative(_skeletons.TrackedSkeleton);
                            cursorPosition.X = (int)RightHandX;
                            cursorPosition.Y = (int)RightHandY;
                        }
                    }
                }

                using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
                {
                    if (depthFrame != null)
                    {
                        // Create array for pixel data and copy it from the image frame
                        short[] pixelData = new short[depthFrame.PixelDataLength];
                        depthFrame.CopyPixelDataTo(pixelData);

                        //_colorVideo = new Texture2D(_graphics.GraphicsDevice, depthFrame.Width, depthFrame.Height);
                        //_colorVideo.SetData(ConvertDepthFrame(pixelData, depthFrame));

                        lastDepthFrameData = pixelData;
                    }
                }
            }

            // Hand tracking START
            if (_skeletons.TrackedSkeleton != null)
            {
                //using (InteractionFrame frame = _KinectChooser.Interactions.OpenNextFrame(0))
                //{
                //    if (frame == null)
                //        return;


                //    int id = _skeletons.TrackedSkeleton.TrackingId;

                //    UserInfo[] usrInfo = new UserInfo[6];

                //    frame.CopyInteractionDataTo(usrInfo);

                //    UserInfo usr = usrInfo.Where(u => u.SkeletonTrackingId == id).FirstOrDefault();
                //    if (usr != null)
                //    {
                //        bool f = true;
                //    }
                //}
            }

            // check for hand
            if (cursorPosition != Vector2.Zero && _skeletons.TrackedSkeleton != null)
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

                JointType handType = leftHanded ? JointType.HandLeft : JointType.HandRight;
                JointType wristType = leftHanded ? JointType.WristLeft : JointType.WristRight;

                _handDepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[handType].Position, DepthImageFormat.Resolution640x480Fps30);
                var wristDepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[wristType].Position, DepthImageFormat.Resolution640x480Fps30);

                SkeletonPoint hand = _skeletons.TrackedSkeleton.Joints[handType].Position;
                SkeletonPoint wrist = _skeletons.TrackedSkeleton.Joints[wristType].Position;
                Vector2 handVector = new Vector2(_handDepthPoint.X, _handDepthPoint.Y);
                Vector2 wristVector = new Vector2(wristDepthPoint.X, wristDepthPoint.Y);
                //float distance = Vector2.Distance(handVector, wristVector);

                //float distance = GetDistanceBetweenJoints(_skeletons.TrackedSkeleton, JointType.Head, JointType.HipCenter);

                var head = _skeletons.TrackedSkeleton.Joints[JointType.Head].Position;
                var shoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderCenter].Position;
                float distance = (head.Y - shoulder.Y) * 100; // in milimeters

                // podivame se, v jake vzdalenosti bod je
                // i kdyz prevadime body ze skeletonu do depth space, tak to vraci body i mimo ten obrazek, proto 
                // je nutne takhle osetrit okrajove podminky pri cteni surovych dat
                short[] frameData = lastDepthFrameData;
                int stride = 640;
                int index = (_handDepthPoint.Y > stride ? stride : _handDepthPoint.Y) * stride + _handDepthPoint.X;
                if (index > frameData.Length) index = frameData.Length - 1;

                int player = frameData[index] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = frameData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;


                float angle = (float)Math.Atan2(hand.Y - wrist.Y, hand.X - wrist.X) - MathHelper.PiOver2;
                int handArea = 0;
                if (realDepth > 0)
                {
                    _histogram = new int[256];

                    float radius = 35000 / (float)realDepth;
                    radius = distance;

                    radius = realDepth / 4 - 200;

                    if (radius < 1) radius = 1;

                    _handRadius = (int)(radius * 1.5);

                    if (_handRadius <= _handDepthPoint.X && _handRadius <= _handDepthPoint.Y)
                    {
                        _handRect = new Rectangle((int)(_handDepthPoint.X - _handRadius), (int)(_handDepthPoint.Y - _handRadius), _handRadius * 2, _handRadius * 2);

                        // transform 13-bit depth information into an 8-bit intensity appropriate
                        // for display (we disregard information in most significant bit)
                        //                byte intensity = (byte)(~(realDepth >> 4));


                        handArea = 0;

                        for (int y = _handRect.Top; y < _handRect.Bottom; y++)
                        {
                            for (int x = _handRect.Left; x < _handRect.Right; x++)
                            {
                                int i = y * stride + x;
                                if (i < frameData.Length && i >= 0)
                                {
                                    int playerIndex = frameData[i] & DepthImageFrame.PlayerIndexBitmask;
                                    if (playerIndex > 0 && playerIndex == player)
                                    {
                                        int realPixelDepth = frameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                                        // transform 13-bit depth information into an 8-bit intensity appropriate
                                        // for display (we disregard information in most significant bit)
                                        byte intensity = (byte)(~(realPixelDepth >> 4));

                                        _histogram[intensity]++;

                                        handArea++;
                                    }
                                }
                            }
                        }
                    }

                    int max = 0;
                    for (int i = 0; i < _histogram.Length; i++)
                    {
                        max = Math.Max(max, _histogram[i]);
                    }

                    //if (max > (_handRect.Width * _handRect.Height) / 3)
                    if (handArea < (_handRect.Width * _handRect.Height) / 2)
                    {
                        _textToRender += "Open!";
                    }
                    else
                    {
                        _textToRender += "Closed";
                    }

                    _textToRender += string.Format(" [{0:P2}]", Math.Round((double)handArea / (_handRect.Width * _handRect.Height), 2));
                }
            }

            base.Update(gameTime);
        }



        private float GetDistanceBetweenJoints(Skeleton skeleton, JointType join1, JointType join2)
        {
            var join1DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[join1].Position, DepthImageFormat.Resolution640x480Fps30);
            var join2DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[join2].Position, DepthImageFormat.Resolution640x480Fps30);

            Vector2 joint1Position = new Vector2(join1DepthPoint.X, join1DepthPoint.Y);
            Vector2 joint2Position = new Vector2(join2DepthPoint.X, join2DepthPoint.Y);

            return Vector2.Distance(joint1Position, joint1Position);
        }

        // Absolute mapping of cursor
        private Vector2 TrackHandMovementAbsolute(Skeleton skeleton)
        {
            int width = GraphicsDevice.Viewport.Bounds.Width;
            int height = GraphicsDevice.Viewport.Bounds.Height;
            SkeletonPoint handPoint = skeleton.Joints[JointType.HandLeft].Position;
            var colorPt = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(handPoint, _KinectChooser.Sensor.ColorStream.Format);
            double ratioX = (double)colorPt.X / _KinectChooser.Sensor.ColorStream.FrameWidth;
            double ratioY = (double)colorPt.Y / _KinectChooser.Sensor.ColorStream.FrameHeight;
            cursorPosition = new Vector2();
            cursorPosition.X = (int)(width * ratioX);
            cursorPosition.Y = (int)(height * ratioY);

            return cursorPosition;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (_textToRender != null)
            {
                Vector2 FontOrigin = font.MeasureString(_textToRender) / 2;

                spriteBatch.DrawString(font, _textToRender, new Vector2(500, 20), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            if (_handRect != null)
            {
                Rectangle translated = new Rectangle((int)(_handRect.X / _scale) + (int)_kinectDebugOffset.X, (int)(_handRect.Y / _scale) + (int)_kinectDebugOffset.Y, (int)(_handRect.Width / _scale), (int)(_handRect.Height / _scale));

                DrawBoudingBox(translated, Color.Red, 1);
            }

            if (cursorPosition != null)
            {
                spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // credits http://stackoverflow.com/questions/13893959/how-to-draw-the-border-of-a-square
        Texture2D _pointTexture;
        private void DrawBoudingBox(Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        Vector2 cursorPosition;

        float xPrevious;
        float yPrevious;
        float MoveThreshold = 0.005f;

        double RightHandX;
        double RightHandY;

        // http://stackoverflow.com/questions/12569706/how-to-use-skeletal-joint-to-act-as-cursor-using-bounds-no-gestures
        /// <summary>
        /// Shoulders = top of screen
        /// Hips = bottom of screen
        /// Left Should = left most on screen
        /// </summary>
        /// <param name="skeleton"></param>

        private void TrackHandMovementRelative(Skeleton skeleton)
        {
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint rightHand = skeleton.Joints[JointType.HandRight];

            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];

            Joint rightHip = skeleton.Joints[JointType.HipRight];
            Joint head = skeleton.Joints[JointType.Head];

            // the right hand joint is being tracked
            if (rightHand.TrackingState == JointTrackingState.Tracked)
            {
                // the hand is sufficiently in front of the shoulder
                if (rightShoulder.Position.Z - rightHand.Position.Z > 0.2)
                {
                    double xScaled = (rightHand.Position.X - leftShoulder.Position.X) / ((rightShoulder.Position.X - leftShoulder.Position.X) * 2) * GraphicsDevice.Viewport.Bounds.Width;
                    double yScaled = _KinectChooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated ?
                        (rightHand.Position.Y - rightShoulder.Position.Y) / ((rightShoulder.Position.Y - head.Position.Y) / 2) * GraphicsDevice.Viewport.Bounds.Height :
                        (rightHand.Position.Y - rightShoulder.Position.Y) / (rightHip.Position.Y - rightShoulder.Position.Y) * GraphicsDevice.Viewport.Bounds.Height;

                    if (yScaled < 0) yScaled = 0;
                    if (yScaled > GraphicsDevice.Viewport.Bounds.Height) yScaled = GraphicsDevice.Viewport.Bounds.Height;

                    if (xScaled < 0) xScaled = 0;
                    if (xScaled > GraphicsDevice.Viewport.Bounds.Width) xScaled = GraphicsDevice.Viewport.Bounds.Width;

                    // the hand has moved enough to update screen position (jitter control / smoothing)
                    if (Math.Abs(rightHand.Position.X - xPrevious) > MoveThreshold || Math.Abs(rightHand.Position.Y - yPrevious) > MoveThreshold)
                    {
                        RightHandX = xScaled;
                        RightHandY = yScaled;

                        xPrevious = rightHand.Position.X;
                        yPrevious = rightHand.Position.Y;
                    }
                }
            }
        }
    }
}
