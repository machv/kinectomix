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
using AtomixData;
using System.Xml;
using Microsoft.Kinect;

namespace Atomix
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AtomixGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Level currentLevel;
        IGameScreen gameScreen;
        KinectChooser chooser;
        SkeletonRenderer skeletonRenderer;
        Skeletons _skeletons = new Skeletons();
        Texture2D _handTexture;

        public AtomixGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.IsFullScreen = true;

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
            this.IsMouseVisible = true;

            chooser = new KinectChooser(this);
            skeletonRenderer = new SkeletonRenderer(this, chooser.Sensor, _skeletons, new Vector2(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 / 2, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 / 2));

            Components.Add(chooser);
            Components.Add(skeletonRenderer);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _handTexture = Content.Load<Texture2D>("Images/Hand");

            // Load level
            currentLevel = Content.Load<AtomixData.Level>("Levels/Level1");

            //gameScreen = new LevelScreen(this, currentLevel, spriteBatch);
            gameScreen = new StartScreen(this, spriteBatch);

            gameScreen.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            gameScreen.UnloadContent();
        }

        Vector2 cursorPosition;

        float xPrevious;
        float yPrevious;
        int MoveThreshold = 0;

        double RightHandX;
        double RightHandY;

        // http://stackoverflow.com/questions/12569706/how-to-use-skeletal-joint-to-act-as-cursor-using-bounds-no-gestures
        /// <summary>
        /// Shoulders = top of screen
        /// Hips = bottom of screen
        /// Left Should = left most on screen
        /// </summary>
        /// <param name="skeleton"></param>

        private void TrackHandMovement(Skeleton skeleton)
        {
            Joint leftHand = skeleton.Joints[JointType.HandLeft];
            Joint rightHand = skeleton.Joints[JointType.HandRight];

            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];

            Joint rightHip = skeleton.Joints[JointType.HipRight];

            // the right hand joint is being tracked
            if (rightHand.TrackingState == JointTrackingState.Tracked)
            {
                // the hand is sufficiently in front of the shoulder
                if (rightShoulder.Position.Z - rightHand.Position.Z > 0.4)
                {
                    double xScaled = (rightHand.Position.X - leftShoulder.Position.X) / ((rightShoulder.Position.X - leftShoulder.Position.X) * 2) * GraphicsDevice.Viewport.Bounds.Width;
                    double yScaled = (rightHand.Position.Y - rightShoulder.Position.Y) / (rightHip.Position.Y - rightShoulder.Position.Y) * GraphicsDevice.Viewport.Bounds.Height;

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

        // Hand tracking variables
        Vector2 lastHandPosition = new Vector2(0, 0);
        double lastHandTime;

        DepthImagePoint _handDepthPoint;
        int[] _histogram;
        int _handRadius;

        DepthImagePoint _handRectPoint;
        Rectangle _handRect;
        string _textToRender;
        short[] lastDepthFrameData = null;
        // / Hand trackign variables


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

            if (chooser.Sensor != null)
            {
                using (SkeletonFrame skeletonFrame = chooser.Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    if (skeletonFrame != null)
                    {
                        Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                        //Copy the skeleton data to our array
                        skeletonFrame.CopySkeletonDataTo(skeletonData);

                        _skeletons.Items = skeletonData;
                        _skeletons.TrackedSkeleton = _skeletons.Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                        if (_skeletons.TrackedSkeleton != null)
                        {
                            Joint rightHand = _skeletons.TrackedSkeleton.Joints[JointType.HandRight];
                            Joint head = _skeletons.TrackedSkeleton.Joints[JointType.Head];
                            Joint leftShoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderLeft];
                            Joint rightShoulder = _skeletons.TrackedSkeleton.Joints[JointType.ShoulderRight];
                            Joint rightHip = _skeletons.TrackedSkeleton.Joints[JointType.HipRight];

                            SkeletonPoint handPoint = _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position;
                            int width = GraphicsDevice.Viewport.Bounds.Width;
                            int height = GraphicsDevice.Viewport.Bounds.Height;

                            var colorPt = chooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(handPoint, chooser.Sensor.ColorStream.Format);

                            double ratioX = (double)colorPt.X / chooser.Sensor.ColorStream.FrameWidth;
                            double ratioY = (double)colorPt.Y / chooser.Sensor.ColorStream.FrameHeight;

                            //http://stackoverflow.com/questions/13313005/kinect-sdk-1-6-and-joint-scaleto-method
                            double xScaled = (rightHand.Position.X - leftShoulder.Position.X) / ((rightShoulder.Position.X - leftShoulder.Position.X) * 2) * width;
                            double yScaled = (rightHand.Position.Y - head.Position.Y) / (rightHip.Position.Y - head.Position.Y) * height;

                            cursorPosition = new Vector2();
                            cursorPosition.X = (int)xScaled; // (int)(width * ratioX);
                            cursorPosition.Y = (int)yScaled; // (int)(height * ratioY);

                            TrackHandMovement(_skeletons.TrackedSkeleton);
                            cursorPosition.X = (int)RightHandX;
                            cursorPosition.Y = (int)RightHandY;
                        }
                    }
                }

                using (ColorImageFrame colorVideoFrame = chooser.Sensor.ColorStream.OpenNextFrame(0))
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

                        _colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                        _colorVideo.SetData(bgraPixelData);
                    }
                }

                using (DepthImageFrame depthFrame = chooser.Sensor.DepthStream.OpenNextFrame(0))
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

                _handDepthPoint = chooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30);
                var wristDepthPoint = chooser.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(_skeletons.TrackedSkeleton.Joints[JointType.WristLeft].Position, DepthImageFormat.Resolution640x480Fps30);

                SkeletonPoint hand = _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position;
                SkeletonPoint wrist = _skeletons.TrackedSkeleton.Joints[JointType.WristLeft].Position;
                Vector2 handVector = new Vector2(_handDepthPoint.X, _handDepthPoint.Y);
                Vector2 wristVector = new Vector2(wristDepthPoint.X, wristDepthPoint.Y);

                // podivame se, v jake vzdalenosti bod je
                int stride = 480;
                int index = _handDepthPoint.Y * stride + _handDepthPoint.X;
                short[] frameData = lastDepthFrameData;
                int player = frameData[index] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = frameData[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                float angle = (float)Math.Atan2(hand.Y - wrist.Y, hand.X - wrist.X) - MathHelper.PiOver2;
                if (realDepth > 0)
                {
                    float radius = 35000 / realDepth;


                    _handRadius = (int)(radius * 1.5);
                    if (_handRadius <= _handDepthPoint.X && _handRadius <= _handDepthPoint.Y)
                    {
                        _handRect = new Rectangle((int)(_handDepthPoint.X - _handRadius), (int)(_handDepthPoint.Y - _handRadius), _handRadius * 2, _handRadius * 2);



                        // transform 13-bit depth information into an 8-bit intensity appropriate
                        // for display (we disregard information in most significant bit)
                        //                byte intensity = (byte)(~(realDepth >> 4));

                        _histogram = new int[256];

                        for (int y = _handRect.Top; y < _handRect.Bottom; y++)
                        {
                            for (int x = _handRect.Left; x < _handRect.Right; x++)
                            {
                                int i = y * stride + x;
                                if (i < frameData.Length && i >= 0)
                                {
                                    int playerIndex = frameData[i] & DepthImageFrame.PlayerIndexBitmask;
                                    if (playerIndex > 0)
                                    {
                                        int realPixelDepth = frameData[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                                        // transform 13-bit depth information into an 8-bit intensity appropriate
                                        // for display (we disregard information in most significant bit)
                                        byte intensity = (byte)(~(realPixelDepth >> 4));

                                        _histogram[intensity]++;
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

                    if (max > (_handRect.Width * _handRect.Height) / 3)
                    {
                        _textToRender = "Open!";
                    }
                    else
                    {
                        _textToRender = "Closed";
                    }

                    _textToRender += string.Format(" [{0:P2}]", (double)max / (_handRect.Width * _handRect.Height));
                }

                double depth = Math.Round(realDepth / 10f, 2);

                //_textToRender = "Player's " + player + " hand at [" + _handDepthPoint.X + "x" + _handDepthPoint.Y + "] in depth " + depth + " cm, radius " + radius + ".";
            }

            // Hand trackign stop



            gameScreen.Update(gameTime);

            base.Update(gameTime);
        }

        Texture2D _colorVideo;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_colorVideo != null)
            {
                spriteBatch.Begin();
                int scale = 2;
                spriteBatch.Draw(_colorVideo, new Rectangle(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 / scale, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 / scale, 640 / scale, 480 / scale), Color.White);
                //spriteBatch.Draw(_colorVideo, new Vector2(500, 20), null, Color.White,0, new Vector2(0,0), 0.5f, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            gameScreen.Draw(gameTime);

            if (cursorPosition != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            if (_textToRender != null)
            {
                // Find the center of the string
                //Vector2 FontOrigin = _font1.MeasureString(_textToRender) / 2;
                // Draw the string
                //_spriteBatch.DrawString(_font1, _textToRender, _fontPos, Color.Red,
                //    0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            base.Draw(gameTime);
        }

        public void ChangeScreen(IGameScreen screen)
        {
            gameScreen.UnloadContent();

            gameScreen = screen;

            gameScreen.LoadContent();
        }
    }
}
