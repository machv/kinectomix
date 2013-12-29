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
using Microsoft.Kinect;


namespace KinectFirstSteps
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AtomixGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        KinectSensor _kinect;
        Texture2D _colorVideo;
        Skeletons _skeletons;

        public AtomixGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeKinectSensor();

            Components.Add(new Components.SkeletonRenderer(this, _kinect, _skeletons, SkeletonToColorMap));

            base.Initialize();
        }

        private void InitializeKinectSensor()
        {
            _kinect = KinectSensor.KinectSensors.FirstOrDefault();

            //_kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _kinect.ColorFrameReady += kinect_ColorFrameReady;
            //kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            _kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _kinect.DepthFrameReady += _kinect_DepthFrameReady;
            _kinect.SkeletonStream.Enable();

            _kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default; // tracking of top body 
            _kinect.Start();
            _kinect.ElevationAngle = 0;

            _skeletons = new Skeletons();
        }

        private byte[] ConvertDepthFrame(short[] depthFrameData, DepthImageFrame depthFrame)
        {
            int RedIndex = 0, GreenIndex = 1, BlueIndex = 2, AlphaIndex = 3;
            byte[] depthFrame32 = new byte[depthFrame.Width * depthFrame.Height * 4];

            for (int i16 = 0, i32 = 0; i16 < depthFrameData.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrameData[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrameData[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                depthFrame32[i32 + RedIndex] = (byte)(intensity);
                depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                depthFrame32[i32 + AlphaIndex] = 255;
            }

            rawDepthFrame = depthFrame32;

            return depthFrame32;
        }

        byte[] rawDepthFrame;
        short[] lastDepthFrameData = null;

        void _kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame colorVideoFrame = e.OpenDepthImageFrame())
            {
                if (colorVideoFrame != null)
                {
                    // Create array for pixel data and copy it from the image frame
                    short[] pixelData = new short[colorVideoFrame.PixelDataLength];
                    colorVideoFrame.CopyPixelDataTo(pixelData);

                    _colorVideo = new Texture2D(_graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                    _colorVideo.SetData(ConvertDepthFrame(pixelData, colorVideoFrame));

                    lastDepthFrameData = pixelData;
                }
            }
        }

        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorVideoFrame = e.OpenColorImageFrame())
            {
                if (colorVideoFrame != null)
                {
                    // Create array for pixel data and copy it from the image frame
                    Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                    colorVideoFrame.CopyPixelDataTo(pixelData);

                    //Convert RGBA to BGRA
                    Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                    for (int i = 0; i < pixelData.Length; i += 4)
                    {
                        bgraPixelData[i] = pixelData[i + 2];
                        bgraPixelData[i + 1] = pixelData[i + 1];
                        bgraPixelData[i + 2] = pixelData[i];
                        bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                    }

                    _colorVideo = new Texture2D(_graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                    _colorVideo.SetData(bgraPixelData);
                }
            }
        }

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        private Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != _kinect) && (null != _kinect.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = _kinect.CoordinateMapper.MapSkeletonPointToColorPoint(point, _kinect.ColorStream.Format);
                return new Vector2(colorPt.X, colorPt.Y);
            }

            return Vector2.Zero;
        }

        SpriteFont _font1;
        Vector2 _fontPos;
        string _textToRender;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            _font1 = Content.Load<SpriteFont>("Font1");
            _fontPos = new Vector2(500,
                GraphicsDevice.Viewport.Height - 50);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Vector2 lastHandPosition = new Vector2(0, 0);
        double lastHandTime;

        DepthImagePoint handDepthPoint;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (_kinect != null)
            {
                try
                {
                    using (SkeletonFrame skeletonFrame = _kinect.SkeletonStream.OpenNextFrame(0))
                    {
                        if (skeletonFrame != null)
                        {
                            Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                            //Copy the skeleton data to our array
                            skeletonFrame.CopySkeletonDataTo(skeletonData);

                            _skeletons.Items = skeletonData;
                            _skeletons.TrackedSkeleton = _skeletons.Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Report an error message
                }
            }

            // check for ring position
            if (_skeletons.TrackedSkeleton != null && _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                Vector2 handPosition = SkeletonToColorMap(_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position);

                if (Vector2.Distance(handPosition, lastHandPosition) > 5)
                {
                    lastHandTime = 0;
                }
                else
                {
                    lastHandTime += gameTime.ElapsedGameTime.TotalSeconds;
                }

                lastHandPosition = handPosition;

                if (lastHandTime > 1)
                {
                    //TODO: uncomment ring position
                    ///ringPosition = handPosition;
                    hasRingMatch = true;
                    isToRight = true;
                }

                //                DepthImagePixel[] depthPixels;
                //                depthPixels = new DepthImagePixel[_kinect.DepthStream.FramePixelDataLength];
                //                lastDepthFrame.CopyDepthImagePixelDataTo(depthPixels);

                handDepthPoint = _kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30);

                SkeletonPoint hand = _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position;
                SkeletonPoint wrist = _skeletons.TrackedSkeleton.Joints[JointType.WristLeft].Position;
                Vector2 handVector = new Vector2(hand.X, hand.Y);
                Vector2 wristVector = new Vector2(wrist.X, wrist.Y);

                float angle = (float)Math.Atan2(hand.Y - wrist.Y, hand.X - wrist.X) - MathHelper.PiOver2;
                float radius = Vector2.Distance(handVector, wristVector);

                if (wrist.X < hand.X)
                {
                    // go up
                }

                //lastDepthFrame.

                // podivame se, v jake vzdalenosti bod je
                short[] frameData = lastDepthFrameData;

                int stride = 640;
                int index = handDepthPoint.Y * stride + handDepthPoint.X;

                int player = frameData[index] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = frameData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
//                byte intensity = (byte)(~(realDepth >> 4));

                int[] histogram = new int[255];

                int startIndex = (int)((handDepthPoint.Y - radius) * stride + (handDepthPoint.X - radius));
                int endIndex = (int)((handDepthPoint.Y + radius) * stride + (handDepthPoint.X + radius));
                for (int i16 = startIndex; i16 < endIndex; i16++)
                {
                    int playerIndex = frameData[i16] & DepthImageFrame.PlayerIndexBitmask;
                    if (playerIndex > 0)
                    {
                        int realPixelDepth = frameData[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                        // transform 13-bit depth information into an 8-bit intensity appropriate
                        // for display (we disregard information in most significant bit)
                        byte intensity = (byte)(~(realPixelDepth >> 4));

                        histogram[intensity]++;
                    }

                }

                double depth = Math.Round(realDepth / 10f, 2);

                _textToRender = "Player's " + player + " hand at [" + handDepthPoint.X + "x" + handDepthPoint.Y + "] in depth " + depth + " cm, radius " + radius + ".";
            }

            // check for gesture move
            // to right and after ring match
            if (hasRingMatch && isToRight)
            {
                // if y-coordinates are near -- same line
                if (Math.Abs(lastHandPosition.Y - ringPosition.Y) < 10)
                {
                    toRightDifference = lastHandPosition.X - ringPosition.X;

                    // if the move is more that threshold, mark it as gesture and reset position
                    if (toRightDifference > 20)
                    {
                        rightMove = true;
                        hasRingMatch = false;
                        isToRight = false;
                    }
                }
                else
                {
                    isToRight = false;

                    // if the move is more that threshold, mark it as gesture and reset position
                    if (toRightDifference > 50)
                    {
                        rightMove = true;
                        hasRingMatch = false;
                    }
                }
            }


            base.Update(gameTime);
        }
        float toRightDifference = 0;

        // if is right gesture correct
        bool rightMove = false;

        // could be right festure?
        bool isToRight = false;

        // have we cursor aquired?
        bool hasRingMatch = false;
        Vector2 ringPosition = new Vector2(650, 100);

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_colorVideo != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_colorVideo, new Rectangle(0, 0, 640, 480), Color.White);
                _spriteBatch.End();
            }

            _spriteBatch.Begin();

            Texture2D ring = Content.Load<Texture2D>("ring");
            _spriteBatch.Draw(ring, ringPosition, Color.White);

            // draw hand
            Texture2D jointTexture = Content.Load<Texture2D>("Joint");
            Vector2 jointOrigin = new Vector2(jointTexture.Width / 2, jointTexture.Height / 2);

            _spriteBatch.Draw(
                jointTexture,
                new Vector2(handDepthPoint.X, handDepthPoint.Y),
                null,
                Color.Pink,
                0.0f,
                jointOrigin,
                1.0f,
                SpriteEffects.None,
                0.0f);

            if (_textToRender != null)
            {
                // Find the center of the string
                Vector2 FontOrigin = _font1.MeasureString(_textToRender) / 2;
                // Draw the string
                _spriteBatch.DrawString(_font1, _textToRender, _fontPos, Color.Red,
                    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            Texture2D SimpleTexture = new Texture2D(GraphicsDevice, 1, 1, false,
    SurfaceFormat.Color);

            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            SimpleTexture.SetData<Int32>(pixel, 0, SimpleTexture.Width * SimpleTexture.Height);

            // Paint a 100x1 line starting at 20, 50
            _spriteBatch.Draw(SimpleTexture, new Rectangle(20, 50, 100, 1), Color.Blue);

            // Draw histogram


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
