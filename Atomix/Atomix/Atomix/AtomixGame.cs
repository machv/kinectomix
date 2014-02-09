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
                            SkeletonPoint handPoint = _skeletons.TrackedSkeleton.Joints[JointType.HandLeft].Position;
                            int width = GraphicsDevice.Viewport.Bounds.Width;
                            int height = GraphicsDevice.Viewport.Bounds.Height;

                            var colorPt = chooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(handPoint, chooser.Sensor.ColorStream.Format);

                            double ratioX = (double)colorPt.X / chooser.Sensor.ColorStream.FrameWidth;
                            double ratioY = (double)colorPt.Y / chooser.Sensor.ColorStream.FrameHeight;

                            cursorPosition = new Vector2();
                            cursorPosition.X = (int) (width * ratioX);
                            cursorPosition.Y = (int) (height * ratioY);
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
            }

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

            if (cursorPosition != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(_handTexture, cursorPosition, null, Color.White,0, new Vector2(0,0), 0.25f, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            gameScreen.Draw(gameTime);

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
