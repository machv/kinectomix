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

            _kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _kinect.ColorFrameReady += kinect_ColorFrameReady;
            //kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            _kinect.SkeletonStream.Enable();

            _kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default; // tracking of top body 
            _kinect.Start();
            _kinect.ElevationAngle = 0;

            _skeletons = new Skeletons();
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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

            if (_skeletons.TrackedSkeleton != null)
            {
                ringPosition = SkeletonToColorMap(_skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position);
            }

            base.Update(gameTime);
        }

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
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
