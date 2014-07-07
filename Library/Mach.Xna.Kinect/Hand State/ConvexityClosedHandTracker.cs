using Mach.Xna.Kinect.Algorithms;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mach.Xna.Kinect.HandState
{
    public class ConvexityClosedHandTracker : IHandStateTracker
    {
        private List<Tuple<Point, Point>> _lines = new List<Tuple<Point, Point>>();
        private VisualKinectManager _kinectChooser;
        private string _textToRender;
        private bool _isHandClosed;
        private Rectangle _handBoundingBox;
        private short[] lastDepthFrameData;
        private int lastDepthFrameDataLength;
        private Texture2D _pointTexture;

        public VideoStreamComponent VideoStreamData { get; set; }

        public bool IsStateActive
        {
            get { return _isHandClosed; }
        }

        public ConvexityClosedHandTracker(VisualKinectManager chooser)
        {
            _kinectChooser = chooser;
        }

        public void ProcessDepthData(DepthImageFrame depthFrame)
        {
            if (depthFrame != null)
            {
                // Create array for pixel data and copy it from the image frame
                short[] pixelData = new short[depthFrame.PixelDataLength];
                depthFrame.CopyPixelDataTo(pixelData);

                lastDepthFrameData = pixelData;
                lastDepthFrameDataLength = depthFrame.PixelDataLength;
            }
        }

        public void ProcessSkeletonData(SkeletonFrame frame) { }

        public void Update(bool leftHanded, Vector2 cursorPosition)
        {
            _isHandClosed = false; // Expect not closed

            if (_kinectChooser.Skeletons.TrackedSkeleton == null)
                return;

            JointType wristType = leftHanded ? JointType.WristLeft : JointType.WristRight;
            JointType handType = leftHanded ? JointType.HandLeft : JointType.HandRight;

            if (_kinectChooser.Skeletons.TrackedSkeleton.Joints[handType].TrackingState != JointTrackingState.Tracked)
                return;

            DepthImagePoint _handDepthPoint = _kinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_kinectChooser.Skeletons.TrackedSkeleton.Joints[handType].Position, _kinectChooser.Sensor.DepthStream.Format);
            DepthImagePoint wristDepthPoint = _kinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_kinectChooser.Skeletons.TrackedSkeleton.Joints[wristType].Position, _kinectChooser.Sensor.DepthStream.Format);

            SkeletonPoint hand = _kinectChooser.Skeletons.TrackedSkeleton.Joints[handType].Position;
            SkeletonPoint wrist = _kinectChooser.Skeletons.TrackedSkeleton.Joints[wristType].Position;
            Vector2 handVector = new Vector2(_handDepthPoint.X, _handDepthPoint.Y);
            Vector2 wristVector = new Vector2(wristDepthPoint.X, wristDepthPoint.Y);
            float distance = GetDistanceBetweenJoints(_kinectChooser.Skeletons.TrackedSkeleton, handType, wristType);
            float distanceHead = GetDistanceBetweenJoints(_kinectChooser.Skeletons.TrackedSkeleton, JointType.Head, JointType.ShoulderCenter);

            // podivame se, v jake vzdalenosti bod je
            // i kdyz prevadime body ze skeletonu do depth space, tak to vraci body i mimo ten obrazek, proto 
            // je nutne takhle osetrit okrajove podminky pri cteni surovych dat
            int stride = _kinectChooser.Sensor.DepthStream.FrameWidth;
            int index = (_handDepthPoint.Y > stride ? stride : _handDepthPoint.Y) * stride + _handDepthPoint.X;
            if (index > lastDepthFrameData.Length) index = lastDepthFrameData.Length - 1;
            if (index < 0) index = 0;

            int player = lastDepthFrameData[index] & DepthImageFrame.PlayerIndexBitmask;
            int realDepth = lastDepthFrameData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

            if (realDepth > 0)
            {
                // Hint for searching bounding box of hand
                int _handRadius = (int)(distanceHead * 2);

                if (_handRadius <= _handDepthPoint.X && _handRadius <= _handDepthPoint.Y)
                {
                    Rectangle hintRectangle = new Rectangle(_handDepthPoint.X - _handRadius / 2, _handDepthPoint.Y - _handRadius / 2, _handRadius, _handRadius);

                    // Clear buffer for drawing
                    _lines.Clear();

                    int tolerance = 50; // in milimeters
                    int width, height;
                    Rectangle handRect = CalculateHandDimensions(hintRectangle, lastDepthFrameData, stride, realDepth, tolerance, out width, out height);
                    if (handRect != Rectangle.Empty)
                    {
                        _handBoundingBox = new Rectangle(handRect.X - 10, handRect.Y - 10, handRect.Width + 20, handRect.Height + 20);

                        // Lines for checking
                        List<int> changes = GetChangesList(lastDepthFrameData, stride, realDepth, tolerance);

                        short isOpenMatches = 0;
                        foreach (int parts in changes)
                        {
                            if (parts > 3) // ......--------...... = empty HAND empty = at least 3 parts
                            {
                                isOpenMatches++;
                            }
                        }

                        _isHandClosed = isOpenMatches <= 1;

                        _textToRender = _isHandClosed ? "Closed" : "Open";
                        _textToRender += string.Format(" ({0})", isOpenMatches);
                        _textToRender += string.Format(" Width: {0}px, Height: {1}px (ratio {2}) / depth: {3} cm", width, height, Math.Round(width / (double)height, 2), realDepth / 10d);
                    }

                    byte[] pixels = null;
                    if (VideoStreamData != null && VideoStreamData.VideoFrame != null)
                    {
                        pixels = new byte[VideoStreamData.VideoFrame.Width * VideoStreamData.VideoFrame.Height * 4];
                        VideoStreamData.VideoFrame.GetData(pixels);
                    }

                    for (int y = _handBoundingBox.Top; y < _handBoundingBox.Bottom; y++)
                    {
                        for (int x = _handBoundingBox.Left; x < _handBoundingBox.Right; x++)
                        {
                            int i = y * stride + x;
                            if (i < lastDepthFrameData.Length && i >= 0)
                            {
                                int realPixelDepth = lastDepthFrameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                                int playerIndex = lastDepthFrameData[i] & DepthImageFrame.PlayerIndexBitmask;

                                if (playerIndex > 0 && playerIndex == player)
                                {
                                    int colorOffset = i * 4;

                                    // Skip pixels outside depth tolerance
                                    if (realPixelDepth < (realDepth - tolerance) || realPixelDepth > (realDepth + tolerance))
                                    {
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
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, float scale, Vector2 renderOffset)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new Color[] { Color.White });
            }

            if (_textToRender != null)
            {
                Vector2 FontOrigin = font.MeasureString(_textToRender) / 2;

                spriteBatch.DrawString(font, _textToRender, new Vector2(600, 20), Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            Rectangle translated = new Rectangle((int)(_handBoundingBox.X / scale) + (int)renderOffset.X, (int)(_handBoundingBox.Y / scale) + (int)renderOffset.Y, (int)(_handBoundingBox.Width / scale), (int)(_handBoundingBox.Height / scale));

            DrawBoudingBox(spriteBatch, translated, Color.Red, 1);

            // draw lines for grid
            foreach (var line in _lines)
            {
                Vector2 start = new Vector2(line.Item1.X / scale, line.Item1.Y / scale);
                Vector2 end = new Vector2(line.Item2.X / scale, line.Item2.Y / scale);
                Vector2 diff = end - start;
                Vector2 scaleVector = new Vector2(1.0f, diff.Length() / _pointTexture.Height);

                float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

                Color color = Color.CornflowerBlue;

                spriteBatch.Draw(_pointTexture, (start + renderOffset), null, color, angle, new Vector2(0.5f, 0.0f), scaleVector, SpriteEffects.None, 1.0f);
            }
        }

        private void DrawBoudingBox(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }


        private float GetDistanceBetweenJoints(Skeleton skeleton, JointType join1, JointType join2)
        {
            DepthImagePoint join1DepthPoint = _kinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[join1].Position, _kinectChooser.Sensor.DepthStream.Format);
            DepthImagePoint join2DepthPoint = _kinectChooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeleton.Joints[join2].Position, _kinectChooser.Sensor.DepthStream.Format);

            Vector2 joint1Position = new Vector2(join1DepthPoint.X, join1DepthPoint.Y);
            Vector2 joint2Position = new Vector2(join2DepthPoint.X, join2DepthPoint.Y);

            return Vector2.Distance(joint1Position, joint2Position);
        }

        private Rectangle CalculateHandDimensions(Rectangle rectangle, short[] frameData, int stride, int realDepth, int tolerance, out int width, out int height)
        {
            int top = int.MaxValue;
            int left = int.MaxValue;
            int bottom = 0;
            int right = 0;

            for (int y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                for (int x = rectangle.Left; x < rectangle.Right; x++)
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

            // If we didnt matched any hand, return empty
            if (top == int.MaxValue)
                return Rectangle.Empty;

            return new Rectangle(left, top, width, height);
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
            for (int i = _handBoundingBox.Top; i < _handBoundingBox.Bottom; i += step)
            {
                Point lineStart = new Point(_handBoundingBox.Left, i);
                Point lineEnd = new Point(_handBoundingBox.Right, i);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // vertical uniform grid
            for (int i = _handBoundingBox.Left; i < _handBoundingBox.Right; i += step)
            {
                Point lineStart = new Point(i, _handBoundingBox.Bottom);
                Point lineEnd = new Point(i, _handBoundingBox.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // horizontal "cake"
            for (int i = _handBoundingBox.Top; i < _handBoundingBox.Bottom; i += step)
            {
                int j = _handBoundingBox.Bottom - i + _handBoundingBox.Top;
                Point lineStart = new Point(_handBoundingBox.Left, i);
                Point lineEnd = new Point(_handBoundingBox.Right, j);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // vertical "cake"
            for (int i = _handBoundingBox.Left; i < _handBoundingBox.Right; i += step)
            {
                int j = _handBoundingBox.Right - i + _handBoundingBox.Left;
                Point lineStart = new Point(i, _handBoundingBox.Bottom);
                Point lineEnd = new Point(j, _handBoundingBox.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // + some random horizontal lines
            for (int i = 0; i < 5; i++)
            {
                Point lineStart = new Point(_handBoundingBox.Left, rand.Next(_handBoundingBox.Top, _handBoundingBox.Bottom));
                Point lineEnd = new Point(_handBoundingBox.Right, rand.Next(_handBoundingBox.Top, _handBoundingBox.Bottom));
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            // + some random vertical lines
            for (int i = 0; i < 5; i++)
            {
                Point lineStart = new Point(rand.Next(_handBoundingBox.Left, _handBoundingBox.Right), _handBoundingBox.Bottom);
                Point lineEnd = new Point(rand.Next(_handBoundingBox.Left, _handBoundingBox.Right), _handBoundingBox.Top);
                int parts = GetLineParts(lineStart, lineEnd, frameData, stride, realDepth, tolerance);

                changes.Add(parts);
            }

            return changes;
        }

        private int GetLineParts(Point start, Point end, short[] frame, int stride, int realDepth, int tolerance)
        {
            _lines.Add(Tuple.Create(start, end));

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
    }
}
