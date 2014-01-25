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
        Texture2D brickTexture;
        Texture2D emptyTexture;
        Texture2D carbonTexture;
        Texture2D hydrogenTexture;
        Texture2D oxygenTexture;

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

            brickTexture = this.Content.Load<Texture2D>("Board/Brick");
            emptyTexture = this.Content.Load<Texture2D>("Board/Empty");
            carbonTexture = this.Content.Load<Texture2D>("Board/Carbon");
            hydrogenTexture = this.Content.Load<Texture2D>("Board/Hydrogen");
            oxygenTexture = this.Content.Load<Texture2D>("Board/Oxygen");

            // Load level
            currentLevel = Content.Load<AtomixData.Level>("Levels/Level1");

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Vector2 mPosition = new Vector2(0, 0);

            spriteBatch.Begin();

            int width = 25;
            int height = 25;

            for (int x = 0; x < currentLevel.Rows; x++)
            {
                for (int y = 0; y < currentLevel.Columns; y++)
                {
                    Texture2D tile = emptyTexture;

                    switch (currentLevel.Board[x, y].Type)
                    {
                        case TileType.Wall:
                            tile = brickTexture;
                            break;
                        case TileType.Empty:
                            tile = emptyTexture;
                            break;
                        case TileType.Carbon:
                            tile = carbonTexture;
                            break;
                        case TileType.Oxygen:
                            tile = oxygenTexture;
                            break;
                        case TileType.Hydrogen:
                            tile = hydrogenTexture;
                            break;
                    }

                    spriteBatch.Draw(tile, mPosition, Color.White);

                    mPosition.X += width;

                }
                mPosition.X = 0;
                mPosition.Y += height;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
