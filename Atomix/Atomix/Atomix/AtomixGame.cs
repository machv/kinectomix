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
        Texture2D carbonSelectedTexture;
        Texture2D hydrogenTexture;
        Texture2D hydrogenSelectedTexture;
        Texture2D oxygenTexture;
        Texture2D oxygenSelectedTexture;
        Texture2D arrowTexture;
        MouseState mouseState;
        MouseState lastMouseState;

        int TileWidth = 49;
        int TileHeight = 49;

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
            carbonSelectedTexture = this.Content.Load<Texture2D>("Board/CarbonSelected");
            hydrogenTexture = this.Content.Load<Texture2D>("Board/Hydrogen");
            hydrogenSelectedTexture = this.Content.Load<Texture2D>("Board/HydrogenSelected");
            oxygenTexture = this.Content.Load<Texture2D>("Board/Oxygen");
            oxygenSelectedTexture = this.Content.Load<Texture2D>("Board/OxygenSelected");
            arrowTexture = this.Content.Load<Texture2D>("Board/Up");

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

                for (int i = 0; i < currentLevel.Board.RowsCount; i++)
                {
                    for (int j = 0; j < currentLevel.Board.ColumnsCount; j++)
                    {
                        Rectangle tile = new Rectangle((int)mPosition.X, (int)mPosition.Y, TileWidth, TileHeight);
                        if (tile.Contains(mousePosition))
                        {
                            if (!currentLevel.Board[i, j].IsFixed)
                            {
                                ClearBoard();

                                currentLevel.Board[i, j].IsSelected = true;
                                currentLevel.Board[i, j].Movements = Direction.None;

                                //zjistit jakymi smery se muze pohnout
                                if (currentLevel.Board.CanGoUp(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Up;
                                    currentLevel.Board[i - 1, j].Type = TileType.Up;
                                }
                                if (currentLevel.Board.CanGoDown(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Down;
                                    currentLevel.Board[i + 1, j].Type = TileType.Down;
                                }
                                if (currentLevel.Board.CanGoLeft(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Left;
                                    currentLevel.Board[i, j - 1].Type = TileType.Left;
                                }
                                if (currentLevel.Board.CanGoRight(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Right;
                                    currentLevel.Board[i, j + 1].Type = TileType.Right;
                                }
                            }
                            else if (currentLevel.Board[i, j].Type == TileType.Right ||
                                     currentLevel.Board[i, j].Type == TileType.Left ||
                                     currentLevel.Board[i, j].Type == TileType.Up ||
                                     currentLevel.Board[i, j].Type == TileType.Down)
                            {
                                Point coordinates = new Point(i, j);
                                Point newCoordinates = NewPosition(coordinates, currentLevel.Board[i, j].Type);
                                Point atomCoordinates = GetAtomPosition(coordinates, currentLevel.Board[i, j].Type);

                                BoardTile atom = currentLevel.Board[atomCoordinates.X, atomCoordinates.Y];
                                currentLevel.Board[atomCoordinates.X, atomCoordinates.Y] = currentLevel.Board[newCoordinates.X, newCoordinates.Y];
                                currentLevel.Board[newCoordinates.X, newCoordinates.Y] = atom;

                                // Check victory
                                CheckFinish();

                                ClearBoard();
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

        private bool CheckFinish()
        {
            for (int i = 0; i < currentLevel.Board.RowsCount; i++)
            {
                for (int j = 0; j < currentLevel.Board.ColumnsCount; j++)
                {
                    // We expect match
                    bool isMatch = true;

                    for (int x = 0; x < currentLevel.Molecule.Definition.RowsCount; x++)
                    {
                        for (int y = 0; y < currentLevel.Molecule.Definition.ColumnsCount; y++)
                        {
                            // If we have empty tile in definition it is like a wildcard that matches everything
                            if (currentLevel.Molecule.Definition[x, y].Type == TileType.Empty)
                                continue;

                            if (currentLevel.Board[i + x, j + y].Type != currentLevel.Molecule.Definition[x, y].Type)
                            {
                                isMatch = false;
                                break;
                            }
                        }

                        // If we did not matched we can stop here and continue to next tile on the board.
                        if (!isMatch)
                            break;
                    }

                    // Expectation was correct
                    if (isMatch)
                        return true;
                }
            }

            return false;
        }

        private Point GetAtomPosition(Point directionTilePosition, TileType direction)
        {
            Point atomPosition = directionTilePosition;

            switch (direction)
            {
                case TileType.Right:
                    atomPosition.Y -= 1;
                    break;
                case TileType.Left:
                    atomPosition.Y += 1;
                    break;
                case TileType.Down:
                    atomPosition.X -= 1;
                    break;
                case TileType.Up:
                    atomPosition.X += 1;
                    break;
            }

            return atomPosition;
        }

        private Point NewPosition(Point coordinates, TileType direction)
        {
            Point newCoordinates = coordinates;

            switch (direction)
            {
                case TileType.Right:
                    while (newCoordinates.Y < currentLevel.Board.ColumnsCount)
                    {
                        if (currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Right && currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Empty)
                            break;

                        newCoordinates.Y++;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case TileType.Left:
                    while (newCoordinates.Y >= 0)
                    {
                        if (currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Left && currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Empty)
                            break;

                        newCoordinates.Y--;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case TileType.Down:
                    while (newCoordinates.X < currentLevel.Board.RowsCount)
                    {
                        if (currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Down && currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Empty)
                            break;

                        newCoordinates.X++;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case TileType.Up:
                    while (newCoordinates.X >= 0)
                    {
                        if (currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Up && currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type != TileType.Empty)
                            break;

                        newCoordinates.X--;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
            }

            return newCoordinates;
        }

        private void SwitchTiles(BoardTile boardTile1, BoardTile boardTile2)
        {
            throw new NotImplementedException();
        }

        private void ClearBoard()
        {
            for (int x = 0; x < currentLevel.Board.RowsCount; x++)
            {
                for (int y = 0; y < currentLevel.Board.ColumnsCount; y++)
                {
                    switch (currentLevel.Board[x, y].Type)
                    {
                        case TileType.Down:
                        case TileType.Up:
                        case TileType.Right:
                        case TileType.Left:
                            currentLevel.Board[x, y].Type = TileType.Empty;
                            break;
                    }

                    currentLevel.Board[x, y].IsSelected = false;
                    currentLevel.Board[x, y].Movements = Direction.None;
                }
            }
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

            DrawBoard(spriteBatch, new Vector2(10, 10), currentLevel.Board, true);

            DrawBoard(spriteBatch, new Vector2(600, 10), currentLevel.Molecule.Definition);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawBoard(SpriteBatch spriteBach, Vector2 position, BoardTileCollection board, bool drawEmptyTiles = false)
        {
            Vector2 mPosition = new Vector2(position.X, position.Y);
            bool drawEmpty = false;

            for (int x = 0; x < board.RowsCount; x++)
            {
                for (int y = 0; y < board.ColumnsCount; y++)
                {
                    Texture2D tile = null;
                    drawEmpty = true;
                    float RotationAngle = 0;
                    Vector2 origin = new Vector2();

                    switch (board[x, y].Type)
                    {
                        case TileType.Wall:
                            tile = brickTexture;
                            drawEmpty = false;
                            break;
                        case TileType.Empty:
                            break;
                        case TileType.Carbon:
                            tile = board[x, y].IsSelected ? carbonSelectedTexture : carbonTexture;
                            break;
                        case TileType.Oxygen:
                            tile = board[x, y].IsSelected ? oxygenSelectedTexture : oxygenTexture;
                            break;
                        case TileType.Hydrogen:
                            tile = board[x, y].IsSelected ? hydrogenSelectedTexture : hydrogenTexture;
                            break;
                        case TileType.Up:
                            tile = arrowTexture;
                            break;
                        case TileType.Down:
                            tile = arrowTexture;
                            RotationAngle = MathHelper.Pi;
                            origin.X = tile.Width;
                            origin.Y = tile.Height;
                            break;
                        case TileType.Left:
                            tile = arrowTexture;
                            RotationAngle = -1 * MathHelper.Pi / 2;
                            origin.X = tile.Width;
                            origin.Y = 0;
                            break;
                        case TileType.Right:
                            tile = arrowTexture;
                            RotationAngle = MathHelper.Pi / 2;
                            //origin.X = tile.Width;
                            origin.Y = tile.Height;
                            break;
                    }

                    if (drawEmpty && drawEmptyTiles)
                        spriteBatch.Draw(emptyTexture, mPosition, Color.White);

                    if (tile != null)
                    {
                        spriteBatch.Draw(tile, mPosition, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                        //                       spriteBatch.Draw(tile, mPosition, Color.White);
                    }

                    mPosition.X += TileWidth;

                }
                mPosition.X = position.X;
                mPosition.Y += TileHeight;
            }
        }
    }
}
