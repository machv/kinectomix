using AtomixData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class LevelScreen : IGameScreen
    {
        Level currentLevel;
        SpriteBatch _spriteBatch;
        Game _game;

        public LevelScreen(Game game, Level currentLevel, SpriteBatch spriteBatch)
        {
            this.currentLevel = currentLevel;
            _spriteBatch = spriteBatch;
            _game = game;

            boardPosition = new Vector2(20, 60);

            CalculateBoardTilePositions(boardPosition, currentLevel.Board);
            CalculateBoardTilePositions(new Vector2(600, 20), currentLevel.Molecule.Definition);
        }

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
        SoundEffect applause;
        Vector2 boardPosition;
        SpriteFont normalFont;

        int TileWidth = 49;
        int TileHeight = 49;

        // Animation stuff
        protected bool isMovementAnimation = false;
        protected Vector2 atomPosition;
        protected Vector2 finalPosition;
        Point destination;
        TileType atomToMove;
        TileType moveDirection;
        float atomSpeed = 200f;

        // Scoring
        int moves;

        public void LoadContent()
        {
            brickTexture = _game.Content.Load<Texture2D>("Board/Brick");
            emptyTexture = _game.Content.Load<Texture2D>("Board/Empty");
            carbonTexture = _game.Content.Load<Texture2D>("Board/Carbon");
            carbonSelectedTexture = _game.Content.Load<Texture2D>("Board/CarbonSelected");
            hydrogenTexture = _game.Content.Load<Texture2D>("Board/Hydrogen");
            hydrogenSelectedTexture = _game.Content.Load<Texture2D>("Board/HydrogenSelected");
            oxygenTexture = _game.Content.Load<Texture2D>("Board/Oxygen");
            oxygenSelectedTexture = _game.Content.Load<Texture2D>("Board/OxygenSelected");
            arrowTexture = _game.Content.Load<Texture2D>("Board/Up");
            applause = _game.Content.Load<SoundEffect>("Sounds/Applause");
            normalFont = _game.Content.Load<SpriteFont>("Fonts/Normal");
        }

        public void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            if (isMovementAnimation)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 destinationPosition = currentLevel.Board[destination.X, destination.Y].RenderPosition;

                if (atomPosition.X == destinationPosition.X)
                {
                    int direction = moveDirection == TileType.Up ? -1 : 1;
                    atomPosition.Y += direction * atomSpeed * elapsed;
                }

                if (atomPosition.Y == destinationPosition.Y)
                {
                    int direction = moveDirection == TileType.Left ? -1 : 1;
                    atomPosition.X += direction * atomSpeed * elapsed;
                }

                Rectangle tile = new Rectangle((int)destinationPosition.X, (int)destinationPosition.Y, TileWidth / 8, TileHeight / 8);
                if (tile.Contains((int)atomPosition.X, (int)atomPosition.Y))
                {
                    isMovementAnimation = false;
                    currentLevel.Board[destination.X, destination.Y].Type = atomToMove;
                }
            }
            else
            {
                bool clickOccurred = false;

                // The active state from the last frame is now old
                lastMouseState = mouseState;

                // Get the mouse state relevant for this frame
                mouseState = Mouse.GetState();

                // Recognize a single click of the left mouse button
                if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                {
                    // React to the click
                    clickOccurred = true;
                }

                if (clickOccurred)
                {
                    var mousePosition = new Point(mouseState.X, mouseState.Y);

                    // Find nearest point
                    Vector2 mPosition = new Vector2(boardPosition.X, boardPosition.Y);

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

                                    BoardTile atom = currentLevel.Board[atomCoordinates.X, atomCoordinates.Y]; // remember atom

                                    // start animation
                                    isMovementAnimation = true;
                                    atomPosition = atom.RenderPosition;
                                    destination = newCoordinates;
                                    atomToMove = atom.Type;
                                    moveDirection = currentLevel.Board[i, j].Type;

                                    //instead of just switching do the animation
                                    currentLevel.Board[atomCoordinates.X, atomCoordinates.Y] = currentLevel.Board[newCoordinates.X, newCoordinates.Y]; //switch empty place to the atom
                                    currentLevel.Board[newCoordinates.X, newCoordinates.Y] = atom; // new field will be atom
                                    currentLevel.Board[newCoordinates.X, newCoordinates.Y].Type = TileType.Empty; // but now will be rendered as empty

                                    CalculateBoardTilePositions(boardPosition, currentLevel.Board);

                                    moves += 1;

                                    //BoardTile atom = currentLevel.Board[atomCoordinates.X, atomCoordinates.Y];
                                    //currentLevel.Board[atomCoordinates.X, atomCoordinates.Y] = currentLevel.Board[newCoordinates.X, newCoordinates.Y];
                                    //currentLevel.Board[newCoordinates.X, newCoordinates.Y] = atom;

                                    // Check victory
                                    bool isFinished = CheckFinish();
                                    if (isFinished)
                                    {
                                        applause.Play();
                                    }

                                    ClearBoard();
                                }
                            }

                            mPosition.X += TileWidth;
                        }

                        mPosition.X = boardPosition.X;
                        mPosition.Y += TileHeight;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            DrawBoard(_spriteBatch, currentLevel.Board, true);
            DrawBoard(_spriteBatch, currentLevel.Molecule.Definition);

            _spriteBatch.DrawString(normalFont, string.Format("Score: {0}", moves), new Vector2(10, 20), Color.Red);

            if (isMovementAnimation)
            {
                _spriteBatch.Draw(GetTileTexture(atomToMove, true), atomPosition, Color.White);
            }

            _spriteBatch.End();
        }


        private void CalculateBoardTilePositions(Vector2 startPosition, BoardTileCollection board)
        {
            Vector2 mPosition = new Vector2(startPosition.X, startPosition.Y);

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    board[i, j].RenderPosition = mPosition;

                    mPosition.X += TileWidth;
                }

                mPosition.X = startPosition.X;
                mPosition.Y += TileHeight;
            }
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

        private Texture2D GetTileTexture(TileType tileType, bool isSelected)
        {
            Texture2D tile = null;

            switch (tileType)
            {
                case TileType.Wall:
                    tile = brickTexture;
                    break;
                case TileType.Empty:
                    tile = emptyTexture;
                    break;
                case TileType.Carbon:
                    tile = isSelected ? carbonSelectedTexture : carbonTexture;
                    break;
                case TileType.Oxygen:
                    tile = isSelected ? oxygenSelectedTexture : oxygenTexture;
                    break;
                case TileType.Hydrogen:
                    tile = isSelected ? hydrogenSelectedTexture : hydrogenTexture;
                    break;
                case TileType.Left:
                case TileType.Right:
                case TileType.Down:
                case TileType.Up:
                    tile = arrowTexture;
                    break;
            }

            return tile;
        }

        private void DrawBoard(SpriteBatch spriteBach, BoardTileCollection board, bool drawEmptyTiles = false)
        {
            bool drawEmpty = false;

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    Texture2D tile = GetTileTexture(board[i, j].Type, board[i, j].IsSelected);
                    drawEmpty = true;
                    float RotationAngle = 0;
                    Vector2 origin = new Vector2();

                    switch (board[i, j].Type)
                    {
                        case TileType.Wall:
                            drawEmpty = false;
                            break;
                        case TileType.Down:
                            RotationAngle = MathHelper.Pi;
                            origin.X = tile.Width;
                            origin.Y = tile.Height;
                            break;
                        case TileType.Left:
                            RotationAngle = -1 * MathHelper.Pi / 2;
                            origin.X = tile.Width;
                            origin.Y = 0;
                            break;
                        case TileType.Right:
                            RotationAngle = MathHelper.Pi / 2;
                            origin.Y = tile.Height;
                            break;
                    }

                    if (drawEmpty && drawEmptyTiles)
                        _spriteBatch.Draw(emptyTexture, board[i, j].RenderPosition, Color.White);

                    if (tile != null)
                    {
                        _spriteBatch.Draw(tile, board[i, j].RenderPosition, null, Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
