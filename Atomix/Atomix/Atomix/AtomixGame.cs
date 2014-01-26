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
        MouseState mouseState;
        MouseState lastMouseState;

        int TileWidth = 25;
        int TileHeight = 25;

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

            bool clickOccurred = false;

            // The active state from the last frame is now old
            lastMouseState = mouseState;

            // Get the mouse state relevant for this frame
            mouseState = Mouse.GetState();

            // Recognize a single click of the left mouse button
            if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                // React to the click
                // ...
                clickOccurred = true;
            }

            if (clickOccurred)
            {
                var mousePosition = new Point(mouseState.X, mouseState.Y);

                // Find nearest point
                Vector2 position = new Vector2(10, 10);
                Vector2 mPosition = new Vector2(position.X, position.Y);

                for (int x = 0; x < currentLevel.Board.RowsCount; x++)
                {
                    for (int y = 0; y < currentLevel.Board.ColumnsCount; y++)
                    {
                        if (!currentLevel.Board[x, y].IsFixed)
                        {
                            Rectangle tile = new Rectangle((int)mPosition.X, (int)mPosition.Y, TileWidth, TileHeight);
                            if (tile.Contains(mousePosition))
                            {
                                currentLevel.Board[x, y].IsSelected = true;

                                //zjistit jakymi smery se muze pohnout
                            }
                            else
                            {
                                currentLevel.Board[x, y].IsFixed = false;
                            }
                        }

                        mPosition.X += TileWidth;
                    }

                    mPosition.X = position.X;
                    mPosition.Y += TileHeight;
                }
            }


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


            spriteBatch.Begin();

            DrawBoard(spriteBatch, new Vector2(10, 10), currentLevel.Board);

            DrawBoard(spriteBatch, new Vector2(600, 10), currentLevel.Molecule.Definition);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBoard(SpriteBatch spriteBach, Vector2 position, BoardTileCollection board)
        {
            Vector2 mPosition = new Vector2(position.X, position.Y);

            for (int x = 0; x < board.RowsCount; x++)
            {
                for (int y = 0; y < board.ColumnsCount; y++)
                {
                    Texture2D tile = emptyTexture;

                    switch (board[x, y].Type)
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

                    mPosition.X += TileWidth;

                }
                mPosition.X = position.X;
                mPosition.Y += TileHeight;
            }
        }
    }
}
