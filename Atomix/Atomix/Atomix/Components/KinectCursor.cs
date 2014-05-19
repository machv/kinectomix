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
        const int CursorPositionsBufferLenth = 15;
        protected KinectChooser _KinectChooser;
        protected Skeletons _skeletons;
        SpriteBatch spriteBatch;
        bool leftHanded = false;
        Vector2 _renderOffset;
        float _scale;
        Texture2D _handTexture;
        Texture2D _pointTextures;
        SpriteFont font;
        Vector2[] cursorPositionsBuffer;
        int cursorPositionsBufferIndex;

        private AnimatedTexture SpriteTexture;
        private const float Depth = 0.5f;
        public VideoStreamComponent VideoStreamData { get; set; }

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

            SpriteTexture = new AnimatedTexture(Vector2.Zero, 0, 0.25f, Depth);
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

        protected bool _isHandTracked = false;
        public bool IsHandTracked
        {
            get { return _isHandTracked; }
            protected set { _isHandTracked = value; }
        }

        private const int Frames = 10;
        private const int FramesPerSec = 10;

        protected override void LoadContent()
        {
            _handTexture = Game.Content.Load<Texture2D>("Images/Hand");
            font = Game.Content.Load<SpriteFont>("Fonts/Normal");
            dotTexture = Game.Content.Load<Texture2D>("Images/Dot");
            _pointTextures = Game.Content.Load<Texture2D>("Images/Joint");

            SpriteTexture.Load(Game.Content, "HandAnimation", Frames, FramesPerSec);

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

        protected DepthImageFrame lastDepthFrame;

        public override void Update(GameTime gameTime)
        {
            _textToRender = string.Empty;

            if (_KinectChooser.Sensor != null)
            {
                if (_KinectChooser.SkeletonData != null)
                {
                    //_KinectChooser.Interactions.ProcessSkeleton(_KinectChooser.SkeletonData, _KinectChooser.Sensor.AccelerometerGetCurrentReading(), _KinectChooser.SkeletonTimestamp);
                    _skeletons.Items = _KinectChooser.SkeletonData;

                    if (_skeletons.TrackedSkeleton != null)
                    {
                        //cursorPosition = TrackHandMovementAbsolute(_skeletons.TrackedSkeleton);
                        var cursor = TrackHandMovementRelative(_skeletons.TrackedSkeleton);

                        AddCursorPosition(cursor);

                        if (cursor != Vector2.Zero)
                            _isHandTracked = true;
                    }
                    else
                    {
                        _isHandTracked = false;
                    }
                }

                using (DepthImageFrame depthFrame = _KinectChooser.Sensor.DepthStream.OpenNextFrame(0))
                {
                    if (depthFrame != null)
                    {
                        // Create array for pixel data and copy it from the image frame
                        short[] pixelData = new short[depthFrame.PixelDataLength];
                        depthFrame.CopyPixelDataTo(pixelData);

                        //_KinectChooser.Interactions.ProcessDepth(depthPixels, depthFrame.Timestamp);

                        lastDepthFrameData = pixelData;
                        lastDepthFrameDataLength = depthFrame.PixelDataLength;

                        lastDepthFrame = depthFrame;
                    }
                }

                //using (InteractionFrame frame = _KinectChooser.Interactions.OpenNextFrame(0))
                //{
                //    if (frame != null)
                //    {
                //        UserInfo[] info = new UserInfo[6];
                //        frame.CopyInteractionDataTo(info);

                //        var usr = info.Where(i => i.SkeletonTrackingId > 0).FirstOrDefault();
                //        if (usr != null)
                //        {
                //            foreach (var interaction in usr.HandPointers)
                //            {
                //                if (interaction.HandType == InteractionHandType.Right)
                //                {
                //                    //_textToRender = string.Format("Interaction: [{0}x{1}]", interaction.X, interaction.Y, _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.X, _skeletons.TrackedSkeleton.Joints[JointType.HandRight].Position.Y);

                //                    if (interaction.HandEventType == InteractionHandEventType.Grip)
                //                    {
                //                        IsHandClosed = true;
                //                    }
                //                    else if (interaction.HandEventType == InteractionHandEventType.GripRelease)
                //                    {
                //                        IsHandClosed = false;
                //                    }

                //                    cursorPositionInteraction = new Vector2();
                //                    cursorPositionInteraction.X = (int)(interaction.X * GraphicsDevice.Viewport.Bounds.Width);
                //                    cursorPositionInteraction.Y = (int)(interaction.Y * GraphicsDevice.Viewport.Bounds.Height);
                //                }
                //            }
                //        }
                //    }
                //}
            }

            // Hand tracking START
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

                    JointType handType = leftHanded ? JointType.HandLeft : JointType.HandRight;
                    JointType wristType = leftHanded ? JointType.WristLeft : JointType.WristRight;

                    _handDepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[handType].Position, DepthImageFormat.Resolution640x480Fps30);
                    var wristDepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[wristType].Position, DepthImageFormat.Resolution640x480Fps30);

                    SkeletonPoint hand = _skeletons.TrackedSkeleton.Joints[handType].Position;
                    SkeletonPoint wrist = _skeletons.TrackedSkeleton.Joints[wristType].Position;
                    Vector2 handVector = new Vector2(_handDepthPoint.X, _handDepthPoint.Y);
                    Vector2 wristVector = new Vector2(wristDepthPoint.X, wristDepthPoint.Y);
                    //float distance = Vector2.Distance(handVector, wristVector);
                    float distance = GetDistanceBetweenJoints(_skeletons.TrackedSkeleton, handType, wristType);

                    //float distance = GetDistanceBetweenJoints(_skeletons.TrackedSkeleton, JointType.Head, JointType.HipCenter);

                    //var head = _skeletons.TrackedSkeleton.Joints[JointType.Head].Position;
                    //var shoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderCenter].Position;
                    float distanceHead = GetDistanceBetweenJoints(_skeletons.TrackedSkeleton, JointType.Head, JointType.ShoulderCenter);
                    //float distance1 = (head.Y - shoulder.Y) * 100; // in milimeters

                    // podivame se, v jake vzdalenosti bod je
                    // i kdyz prevadime body ze skeletonu do depth space, tak to vraci body i mimo ten obrazek, proto 
                    // je nutne takhle osetrit okrajove podminky pri cteni surovych dat
                    short[] frameData = lastDepthFrameData;
                    int stride = 640;
                    int index = (_handDepthPoint.Y > stride ? stride : _handDepthPoint.Y) * stride + _handDepthPoint.X;
                    if (index > frameData.Length) index = frameData.Length - 1;
                    if (index < 0) index = 0;

                    int player = frameData[index] & DepthImageFrame.PlayerIndexBitmask;
                    int realDepth = frameData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                    //byte[] colorPixels = new byte[lastDepthFrameDataLength * sizeof(int)];

                    //// Convert the depth to BGRA
                    //int colorPixelIndex = 0;
                    //for (int i = 0; i < frameData.Length; ++i)
                    //{
                    //    // Get the depth for this pixel
                    //    int depth = frameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                    //    // To convert to a byte, we're discarding the most-significant
                    //    // rather than least-significant bits.
                    //    // We're preserving detail, although the intensity will "wrap."
                    //    // Values outside the reliable depth range are mapped to 0 (black).

                    //    byte intensity = (byte)(~(depth >> 4));

                    //    // Write out blue byte
                    //    colorPixels[colorPixelIndex++] = intensity;

                    //    // Write out green byte
                    //    colorPixels[colorPixelIndex++] = intensity;

                    //    // Write out red byte                        
                    //    colorPixels[colorPixelIndex++] = intensity;

                    //    // Write alpha byte
                    //    colorPixels[colorPixelIndex++] = (Byte)255;
                    //}

                    float angle = (float)Math.Atan2(hand.Y - wrist.Y, hand.X - wrist.X) - MathHelper.PiOver2;
                    int handArea = 0;
                    if (realDepth > 0)
                    {
                        _histogram = new int[256];

                        //float radius = 35000 / (float)realDepth;
                        //if (radius < 1) radius = 1;

                        _handRadius = (int)(distanceHead);

                        if (_handRadius <= _handDepthPoint.X && _handRadius <= _handDepthPoint.Y)
                        {
                            //_handRect = new Rectangle((int)(_handDepthPoint.X - _handRadius), (int)(_handDepthPoint.Y - _handRadius), _handRadius * 2, _handRadius * 2);
                            _handRect = new Rectangle(_handDepthPoint.X - _handRadius / 2, _handDepthPoint.Y - _handRadius / 2, _handRadius, _handRadius);

                            // transform 13-bit depth information into an 8-bit intensity appropriate
                            // for display (we disregard information in most significant bit)
                            //                byte intensity = (byte)(~(realDepth >> 4));

                            handArea = 0;

                            int tolerance = 40; // in milimeters

                            // Lines for checking
                            List<int> changes = GetChangesList(frameData, stride, realDepth, tolerance);

                            short isOpenMatches = 0;
                            foreach (int parts in changes)
                            {
                                if (parts > 3) // ......--------...... = empty HAND empty = at least 3 parts
                                {
                                    // probably open hand?
                                    isOpenMatches++;
                                }
                            }

                            _textToRender = isOpenMatches > 2 ? "Open" : "Closed";
                            _textToRender += string.Format(" ({0})", isOpenMatches);

                            //TODO check only one person at time
                            int width, height;
                            CalculateHandDimensions(frameData, stride, realDepth, tolerance, out width, out height);
                            _textToRender += string.Format(" Width: {0}px, Height: {1}px", width, height);

                            byte[] pixels = null;
                            if (VideoStreamData != null && VideoStreamData.VideoFrame != null)
                            {
                                pixels = new byte[VideoStreamData.VideoFrame.Width * VideoStreamData.VideoFrame.Height * 4];
                                VideoStreamData.VideoFrame.GetData(pixels);
                            }

                            for (int y = _handRect.Top; y < _handRect.Bottom; y++)
                            {
                                for (int x = _handRect.Left; x < _handRect.Right; x++)
                                {
                                    int i = y * stride + x;
                                    if (i < frameData.Length && i >= 0)
                                    {
                                        int realPixelDepth = frameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                                        // transform 13-bit depth information into an 8-bit intensity appropriate
                                        // for display (we disregard information in most significant bit)
                                        byte intensity = (byte)(~(realPixelDepth >> 4));

                                        int playerIndex = frameData[i] & DepthImageFrame.PlayerIndexBitmask;

                                        if (playerIndex > 0 && playerIndex == player)
                                        {
                                            _histogram[intensity]++;

                                            int colorOffset = i * 4;

                                            // Skip pixels outside depth tolerance
                                            if (realPixelDepth < (realDepth - tolerance) || realPixelDepth > (realDepth + tolerance))
                                            {
                                                // ouside tolerance
                                                //continue;

                                                if (pixels != null)
                                                {
                                                    // Write out red byte
                                                    pixels[colorOffset++] = 255;

                                                    // Write out green byte
                                                    pixels[colorOffset++] = 0;

                                                    // blue
                                                    pixels[colorOffset++] = 0;

                                                    // Alpha
                                                    pixels[colorOffset++] = 255;
                                                }
                                            }
                                            else
                                            {
                                                // Inside tolerance

                                                handArea++;

                                                if (pixels != null)
                                                {
                                                    // Write out red byte
                                                    pixels[colorOffset++] = 0;

                                                    // Write out green byte
                                                    pixels[colorOffset++] = (_handDepthPoint.X == x && _handDepthPoint.Y == y) ? (byte)255 : (byte)0;

                                                    // Write out red byte                        
                                                    pixels[colorOffset++] = (_handDepthPoint.X == x && _handDepthPoint.Y == y) ? (byte)0 : (byte)255;

                                                    // Alpha
                                                    pixels[colorOffset++] = 255;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (pixels != null)
                            {
                                VideoStreamData.VideoFrame.SetData(pixels);
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

                        //_textToRender += string.Format(" [{0:P2}], Width = {1} px, Height = {2} px", Math.Round((double)handArea / (_handRect.Width * _handRect.Height), 2), handWidth, handHeight);
                    }

                    //_colorVideo = new Texture2D(GraphicsDevice, 640, 480);
                    //_colorVideo.SetData(colorPixels);
                }

                var cursorPos = GetHarmonizedCursorPosition();

                // Avoid flickering
                if (cursorPos != Vector2.Zero)
                {
                    // animate only if on same place
                    if (Vector2.Distance(cursorPosition, cursorPos) > distanceTolerance)
                    {
                        SpriteTexture.Reset();
                        SpriteTexture.Play();
                        IsHandPressed = false;
                    }

                    cursorPosition = cursorPos;
                }


                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                SpriteTexture.UpdateFrame(elapsed);

                if (SpriteTexture.Frame == Frames - 1)
                {
                    IsHandPressed = true;
                    SpriteTexture.Pause();
                }
            }
            else
            {
                // No tracked skeleton is present so we hide cursor.
                cursorPosition = Vector2.Zero;
            }

            base.Update(gameTime);
        }

        private void CalculateHandDimensions(short[] frameData, int stride, int realDepth, int tolerance, out int width, out int height)
        {
            top = int.MaxValue;
            left = int.MaxValue;
            bottom = 0;
            right = 0;

            for (int y = _handRect.Top; y < _handRect.Bottom; y++)
            {
                for (int x = _handRect.Left; x < _handRect.Right; x++)
                {
                    int i = y * stride + x;
                    if (i < frameData.Length && i >= 0)
                    {
                        int realPixelDepth = frameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                        int playerIndex = frameData[i] & DepthImageFrame.PlayerIndexBitmask;

                        // Checking only within tolerance
                        if (playerIndex > 0 && realPixelDepth >= (realDepth - tolerance) && realPixelDepth <= (realDepth + tolerance))
                        {
                            top = Math.Min(top, y);
                            left = Math.Min(left, x);
                            bottom = Math.Max(bottom, y);
                            right = Math.Max(right, x);
                        }
                    }
                }
            }
            width = right - left;
            height = bottom - top;
        }

        private List<int> GetChangesList(short[] frameData, int stride, int realDepth, int tolerance)
        {
            // Clear buffer for drawing
            _lines.Clear();

            // Count parts for testing open/closed
            List<int> changes = new List<int>();
            int step = 10;
            Random rand = new Random();

            // horizontal uniform grid
            for (int i = _handRect.Top; i < _handRect.Bottom; i += step)
            {
                Point lineStart = new Point(_handRect.Left, i);
                Point lineEnd = new Point(_handRect.Right, i);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // vertical uniform grid
            for (int i = _handRect.Left; i < _handRect.Right; i += step)
            {
                Point lineStart = new Point(i, _handRect.Bottom);
                Point lineEnd = new Point(i, _handRect.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // horizontal "cake"
            for (int i = _handRect.Top; i < _handRect.Bottom; i += step)
            {
                int j = _handRect.Bottom - i + _handRect.Top;
                Point lineStart = new Point(_handRect.Left, i);
                Point lineEnd = new Point(_handRect.Right, j);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // vertical "cake"
            for (int i = _handRect.Left; i < _handRect.Right; i += step)
            {
                int j = _handRect.Right - i + _handRect.Left;
                Point lineStart = new Point(i, _handRect.Bottom);
                Point lineEnd = new Point(j, _handRect.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // + some random horizontal lines
            for (int i = 0; i < 5; i++)
            {
                Point lineStart = new Point(_handRect.Left, rand.Next(_handRect.Top, _handRect.Bottom));
                Point lineEnd = new Point(_handRect.Right, rand.Next(_handRect.Top, _handRect.Bottom));
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // + some random vertical lines
            for (int i = 0; i < 5; i++)
            {
                Point lineStart = new Point(rand.Next(_handRect.Left, _handRect.Right), _handRect.Bottom);
                Point lineEnd = new Point(rand.Next(_handRect.Left, _handRect.Right), _handRect.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            return changes;
        }

        int frame = 0;
        float distanceTolerance = 0.5f;

        List<Tuple<Point, Point>> _lines = new List<Tuple<Point, Point>>();

        private int GetLineParts(Point start, Point end, short[] frame, int stride, int realDepth, int tolerance)
        {
            _lines.Add(Tuple.Create<Point, Point>(start, end));

            int changes = 0;
            int previousPixel = 0;

            IEnumerable<Point> points = Bresenham.GetLinePoints(start, end);
            foreach (Point point in points)
            {
                int i = point.Y * stride + point.X;
                if (i < frame.Length && i >= 0)
                {
                    int realPixelDepth = frame[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                    // transform 13-bit depth information into an 8-bit intensity appropriate
                    // for display (we disregard information in most significant bit)
                    byte intensity = (byte)(~(realPixelDepth >> 4));

                    int playerIndex = frame[i] & DepthImageFrame.PlayerIndexBitmask;
                    // Checking of convex for lines only within tolerance
                    if (realPixelDepth >= (realDepth - tolerance) && realPixelDepth <= (realDepth + tolerance))
                    {
                        // Change, add to counter
                        if (playerIndex != previousPixel)
                            changes++;

                        previousPixel = playerIndex;
                    }
                }
            }

            return changes;
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
            var join1DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[join1].Position, DepthImageFormat.Resolution640x480Fps30);
            var join2DepthPoint = _KinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[join2].Position, DepthImageFormat.Resolution640x480Fps30);

            Vector2 joint1Position = new Vector2(join1DepthPoint.X, join1DepthPoint.Y);
            Vector2 joint2Position = new Vector2(join2DepthPoint.X, join2DepthPoint.Y);

            return Vector2.Distance(joint1Position, joint2Position);
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

            var cursor = new Vector2();
            cursor.X = (int)(width * ratioX);
            cursor.Y = (int)(height * ratioY);

            return cursor;
        }

        private Texture2D dotTexture;

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (_textToRender != null)
            {
                Vector2 FontOrigin = font.MeasureString(_textToRender) / 2;

                spriteBatch.DrawString(font, _textToRender, new Vector2(500, 20), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            //if (_handRect != null)
            //{
            Rectangle translated = new Rectangle((int)(_handRect.X / _scale) + (int)_renderOffset.X, (int)(_handRect.Y / _scale) + (int)_renderOffset.Y, (int)(_handRect.Width / _scale), (int)(_handRect.Height / _scale));

            DrawBoudingBox(translated, Color.Red, 1);

            // draw lines for grid
            foreach (var line in _lines)
            {
                Vector2 start = new Vector2(line.Item1.X / _scale, line.Item1.Y / _scale);
                Vector2 end = new Vector2(line.Item2.X / _scale, line.Item2.Y / _scale);
                Vector2 diff = end - start;
                Vector2 scale = new Vector2(1.0f, diff.Length() / dotTexture.Height);

                float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

                Color color = Color.CornflowerBlue;

                spriteBatch.Draw(dotTexture, (start + _renderOffset), null, color, angle, new Vector2(0.5f, 0.0f), scale, SpriteEffects.None, 1.0f);
            }

            //spriteBatch.Draw(_pointTextures, new Vector2(left, top) + _renderOffset, Color.Yellow);

            //}

            if (cursorPosition != Vector2.Zero)
            {
                spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);

                SpriteTexture.DrawFrame(spriteBatch, cursorPosition);
            }

            //if (_colorVideo != null)
            //{
            //    spriteBatch.Draw(_colorVideo, new Rectangle(50, 50, 640, 480), Color.White);
            //}

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
        Vector2 cursorPositionInteraction;

        float xPrevious;
        float yPrevious;
        float MoveThreshold = 0; //0.005f;

        float RightHandX;
        float RightHandY;
        private int lastDepthFrameDataLength;
        private Texture2D _colorVideo;

        // http://stackoverflow.com/questions/12569706/how-to-use-skeletal-joint-to-act-as-cursor-using-bounds-no-gestures
        /// <summary>
        /// Shoulders = top of screen
        /// Hips = bottom of screen
        /// Left Should = left most on screen
        /// </summary>
        /// <param name="skeleton"></param>
        private Vector2 TrackHandMovementRelative(Skeleton skeleton)
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
                    float xScaled = (rightHand.Position.X - leftShoulder.Position.X) / ((rightShoulder.Position.X - leftShoulder.Position.X) * 2) * GraphicsDevice.Viewport.Bounds.Width;
                    float yScaled = _KinectChooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated ?
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

                        return new Vector2(RightHandX, RightHandY);
                    }
                }
            }

            // As fallback return zero position.
            return Vector2.Zero;
        }
    }
}
