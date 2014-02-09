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

        public AtomixGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.IsFullScreen = true;

            chooser = new KinectChooser(this);
            //skeletonRenderer = new SkeletonRenderer(this, chooser.Sensor);

            Components.Add(chooser);
            //Components.Add(new Components.SkeletonRenderer(this, _kinect, _skeletons, SkeletonToColorMap));

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

            using (ColorImageFrame colorVideoFrame = chooser.Sensor.ColorStream.OpenNextFrame(500))
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

            if (_colorVideo != null)
            {
                spriteBatch.Begin();
                int scale = 2;
                spriteBatch.Draw(_colorVideo, new Rectangle(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 / scale, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 / scale, 640 / scale, 480 / scale), Color.White);
                //spriteBatch.Draw(_colorVideo, new Vector2(500, 20), null, Color.White,0, new Vector2(0,0), 0.5f, SpriteEffects.None, 0);
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
