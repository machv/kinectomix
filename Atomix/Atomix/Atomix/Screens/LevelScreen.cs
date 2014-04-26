using Atomix.Components;
using Atomix.ViewModel;
using AtomixData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class LevelScreen : GameScreen
    {
        LevelViewModel currentLevel;
        SpriteBatch spriteBatch;

        public LevelScreen(Level currentLevel, SpriteBatch spriteBatch)
        {
            this.currentLevel = LevelFactory.ToViewModel(currentLevel);
            this.spriteBatch = spriteBatch;

            gameStarted = DateTime.Now;
        }

        Texture2D wallTexture;
        Texture2D emptyTexture;
        Texture2D carbonTexture;
        Texture2D carbonSelectedTexture;
        Texture2D hydrogenTexture;
        Texture2D hydrogenSelectedTexture;
        Texture2D oxygenTexture;
        Texture2D oxygenSelectedTexture;
        Texture2D arrowTexture;
        Texture2D idleTexture;
        MouseState mouseState;
        MouseState lastMouseState;
        SoundEffect applause;
        Vector2 boardPosition;
        SpriteFont normalFont;
        SpriteFont splashFont;
        Point activeAtomIndex = new Point(-1, -1);

        int TileWidth = 60;
        int TileHeight = 60;

        // Animation stuff
        protected bool isMovementAnimation = false;
        protected Vector2 atomPosition;
        protected Vector2 finalPosition;
        Point destination;
        string atomToMove;
        Direction moveDirection;
        float atomSpeed = 400f;

        // Scoring
        int moves;
        DateTime gameStarted;
        TimeSpan gameDuration;

        bool isLevelFinished = false;

        ContentManager _content;

        Button _levelsButton;
        Button _repeatButton;
        Button _nextButton;

        public override void LoadContent()
        {
            boardPosition = new Vector2(20, 60);

            CalculateBoardTilePositions(boardPosition, currentLevel.Board);

            int offset = (int)boardPosition.X + currentLevel.Board.ColumnsCount * TileWidth + TileWidth;
            int moleculeWidth = currentLevel.Molecule.ColumnsCount * TileWidth;
            int moleculeHeight = currentLevel.Molecule.RowsCount * TileHeight;
            int posX = (ScreenManager.GraphicsDevice.Viewport.Bounds.Width - offset) / 2 - moleculeWidth / 2;
            int posY = ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - moleculeHeight / 2;

            CalculateBoardTilePositions(new Vector2(offset + posX, posY), currentLevel.Molecule);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            wallTexture = _content.Load<Texture2D>("Board/Wall");
            emptyTexture = _content.Load<Texture2D>("Board/Empty");
            carbonTexture = _content.Load<Texture2D>("Board/Carbon");
            carbonSelectedTexture = _content.Load<Texture2D>("Board/CarbonSelected");
            hydrogenTexture = _content.Load<Texture2D>("Board/Hydrogen");
            hydrogenSelectedTexture = _content.Load<Texture2D>("Board/HydrogenSelected");
            oxygenTexture = _content.Load<Texture2D>("Board/Oxygen");
            oxygenSelectedTexture = _content.Load<Texture2D>("Board/OxygenSelected");
            arrowTexture = _content.Load<Texture2D>("Board/Up");
            applause = _content.Load<SoundEffect>("Sounds/Applause");
            normalFont = _content.Load<SpriteFont>("Fonts/Normal");
            splashFont = _content.Load<SpriteFont>("Fonts/Splash");
            idleTexture = _content.Load<Texture2D>("Idle");

            _levelsButton = new Button(spriteBatch, "levels");
            _levelsButton.Font = normalFont;
            _levelsButton.LoadContent(ScreenManager.Content);
            _levelsButton.Selected += _levelsButton_Selected;

            _repeatButton = new Button(spriteBatch, "play again");
            _repeatButton.Font = normalFont;
            _repeatButton.LoadContent(ScreenManager.Content);
            _repeatButton.Selected += _repeatButton_Selected;

            _nextButton = new Button(spriteBatch, "continue");
            _nextButton.Font = normalFont;
            _nextButton.LoadContent(ScreenManager.Content);
            _nextButton.Selected += _nextButton_Selected;
        }

        void _nextButton_Selected(object sender, EventArgs e)
        {
            GameScreen gameScreen = null;

            // Load next level
            LevelDefinition newLevelInfo = AtomixGame.State.SwitchToNextLevel();

            if (newLevelInfo == null)
            {
                // We are on last level -> go to main screen
                gameScreen = new StartScreen(spriteBatch);
            }
            else
            {
                Level newLevel = _content.Load<AtomixData.Level>("Levels/" + newLevelInfo.AssetName);
                gameScreen = new LevelScreen(newLevel, spriteBatch);
            }

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        void _repeatButton_Selected(object sender, EventArgs e)
        {
            GameScreen gameScreen = null;

            // Load current level again
            LevelDefinition newLevelInfo = AtomixGame.State.GetCurrentLevel();
            Level newLevel = _content.Load<AtomixData.Level>("Levels/" + newLevelInfo.AssetName);
            gameScreen = new LevelScreen(newLevel, spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        void _levelsButton_Selected(object sender, EventArgs e)
        {
            GameScreen gameScreen = new LevelsScreen(spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        //bool lastHandClosedState;

        public override void Update(GameTime gameTime)
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

            KinectCursor cursor = ((AtomixGame)this.ScreenManager.Game).Cursor;

            if (isLevelFinished)
            {
                _repeatButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2 - _repeatButton.Width - 30, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _repeatButton.Update(gameTime, ScreenManager.InputProvider);

                _levelsButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _levelsButton.Update(gameTime, ScreenManager.InputProvider);

                _nextButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2 + _nextButton.Width + 30, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _nextButton.Update(gameTime, ScreenManager.InputProvider);

                return;
            }

            gameDuration = DateTime.Now - gameStarted;

            if (isMovementAnimation)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 destinationPosition = currentLevel.Board[destination.X, destination.Y].RenderPosition;

                if (atomPosition.X == destinationPosition.X)
                {
                    int direction = moveDirection == Direction.Up ? -1 : 1;
                    atomPosition.Y += direction * atomSpeed * elapsed;
                }

                if (atomPosition.Y == destinationPosition.Y)
                {
                    int direction = moveDirection == Direction.Left ? -1 : 1;
                    atomPosition.X += direction * atomSpeed * elapsed;
                }

                Rectangle tile = new Rectangle((int)destinationPosition.X, (int)destinationPosition.Y, TileWidth / 4, TileHeight / 4);
                if (tile.Contains((int)atomPosition.X, (int)atomPosition.Y))
                {
                    isMovementAnimation = false;
                    currentLevel.Board[destination.X, destination.Y].Asset = atomToMove;

                    // Animation finished -> check victory
                    bool isFinished = CheckFinish();
                    if (isFinished)
                    {
                        applause.Play();
                        isLevelFinished = true;
                    }
                }

                return;
            }

            if (clickOccurred || cursor.IsHandClosed)
            {
                Point mousePosition;
                if (clickOccurred)
                {
                    mousePosition = new Point(mouseState.X, mouseState.Y);
                }
                else
                {
                    mousePosition = new Point((int)cursor.HandPosition.X, (int)cursor.HandPosition.Y);
                }

                // Find nearest point
                Vector2 mPosition = new Vector2(boardPosition.X, boardPosition.Y);

                // Reset active atom to none
                activeAtomIndex = new Point(-1, -1);

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
                                activeAtomIndex = new Point(i, j);

                                //zjistit jakymi smery se muze pohnout
                                if (currentLevel.CanGoUp(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Up;
                                    currentLevel.Board[i - 1, j].Asset = "Up";
                                }
                                if (currentLevel.CanGoDown(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Down;
                                    currentLevel.Board[i + 1, j].Asset = "Down";
                                }
                                if (currentLevel.CanGoLeft(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Left;
                                    currentLevel.Board[i, j - 1].Asset = "Left";
                                }
                                if (currentLevel.CanGoRight(i, j))
                                {
                                    currentLevel.Board[i, j].Movements |= Direction.Right;
                                    currentLevel.Board[i, j + 1].Asset = "Right";
                                }
                            }
                            else if (currentLevel.Board[i, j].Asset == "Right" ||
                                     currentLevel.Board[i, j].Asset == "Left" ||
                                     currentLevel.Board[i, j].Asset == "Up" ||
                                     currentLevel.Board[i, j].Asset == "Down")
                            {
                                Point coordinates = new Point(i, j);
                                Point newCoordinates = NewPosition(coordinates, currentLevel.Board[i, j].Movements);
                                Point atomCoordinates = GetAtomPosition(coordinates, currentLevel.Board[i, j].Movements);

                                BoardTileViewModel atom = currentLevel.Board[atomCoordinates.X, atomCoordinates.Y]; // remember atom

                                // start animation
                                isMovementAnimation = true;
                                atomPosition = atom.RenderPosition;
                                destination = newCoordinates;
                                //atomToMove = atom.Type;
                                moveDirection = currentLevel.Board[i, j].Movements;

                                //instead of just switching do the animation
                                currentLevel.Board[atomCoordinates.X, atomCoordinates.Y] = currentLevel.Board[newCoordinates.X, newCoordinates.Y]; //switch empty place to the atom
                                currentLevel.Board[newCoordinates.X, newCoordinates.Y] = atom; // new field will be atom
                                currentLevel.Board[newCoordinates.X, newCoordinates.Y].IsEmpty = true;// = TileType.Empty; // but now will be rendered as empty

                                CalculateBoardTilePositions(boardPosition, currentLevel.Board);

                                moves += 1;

                                //BoardTile atom = currentLevel.Board[atomCoordinates.X, atomCoordinates.Y];
                                //currentLevel.Board[atomCoordinates.X, atomCoordinates.Y] = currentLevel.Board[newCoordinates.X, newCoordinates.Y];
                                //currentLevel.Board[newCoordinates.X, newCoordinates.Y] = atom;

                                ClearBoard();
                            }
                        }

                        mPosition.X += TileWidth;
                    }

                    mPosition.X = boardPosition.X;
                    mPosition.Y += TileHeight;
                }
            }

            if (!isMovementAnimation && activeAtomIndex.X != -1 && activeAtomIndex.Y != -1)
            {
                // We have selected atom which is not moved -> Detect gestures

                // Gestures will be recognized only when hand is closed
                if (cursor.IsHandClosed)
                {
                    /// Detect Right/Left

                    // expect not
                    isToRightGesture = false;
                    isToLeftGesture = false;

                    if (Math.Abs(lastHandPosition.Y - cursor.HandPosition.Y) < 10)
                    {
                        gestureAccumulatedDistanceX += lastHandPosition.X - cursor.HandPosition.X;
                    }
                    else
                    {
                        // Reset acumulated info
                        gestureAccumulatedDistanceX = 0;
                    }

                    if (gestureAccumulatedDistanceX > gestureThreshold)
                    {
                        isToRightGesture = true;
                    }

                    if (gestureAccumulatedDistanceX < gestureThreshold * -1)
                    {
                        isToLeftGesture = true;
                    }

                    // Detect Top

                    // Detect Down
                }
            }
        }

        bool isToLeftGesture = false;
        bool isToRightGesture = false;
        float gestureAccumulatedDistanceX = 0;
        float gestureAxeTolerance = 10;
        float gestureThreshold = 20;
        Direction gestureDirection;
        Vector2 lastHandPosition;

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            DrawBoard(spriteBatch, currentLevel.Board, true);
            DrawBoard(spriteBatch, currentLevel.Molecule);

            spriteBatch.DrawString(normalFont, string.Format("Score: {0}", moves), new Vector2(20, 20), Color.Red);

            spriteBatch.DrawString(normalFont, string.Format("Time: {0}", gameDuration.ToString(@"mm\:ss")), new Vector2(200, 20), Color.Red);

            if (isMovementAnimation)
            {
                spriteBatch.Draw(GetTileTexture(atomToMove, true), new Rectangle((int)atomPosition.X, (int)atomPosition.Y, TileWidth, TileHeight), Color.White);
            }

            if (isLevelFinished)
            {
                spriteBatch.Draw(idleTexture, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Bounds.Width, ScreenManager.GraphicsDevice.Viewport.Bounds.Height), Color.White);
                spriteBatch.Draw(wallTexture, new Rectangle(0, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 100, ScreenManager.GraphicsDevice.Viewport.Bounds.Width, 230), Color.Brown);

                string name = "level completed";
                Vector2 size = splashFont.MeasureString(name);

                spriteBatch.DrawString(splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 100), Color.White);

                _repeatButton.Draw(gameTime);
                _levelsButton.Draw(gameTime);
                _nextButton.Draw(gameTime);
            }

            spriteBatch.End();
        }


        private void CalculateBoardTilePositions(Vector2 startPosition, BoardCollection<BoardTileViewModel> board)
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

                    for (int x = 0; x < currentLevel.Molecule.RowsCount; x++)
                    {
                        for (int y = 0; y < currentLevel.Molecule.ColumnsCount; y++)
                        {
                            // If we have empty tile in definition it is like a wildcard that matches everything
                            if (currentLevel.Molecule[x, y].IsEmpty)
                                continue;

                            if (currentLevel.Board[i + x, j + y].Asset != currentLevel.Molecule[x, y].Asset)
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

        private Point GetAtomPosition(Point directionTilePosition, Direction direction)
        {
            Point atomPosition = directionTilePosition;

            switch (direction)
            {
                case Direction.Right:
                    atomPosition.Y -= 1;
                    break;
                case Direction.Left:
                    atomPosition.Y += 1;
                    break;
                case Direction.Down:
                    atomPosition.X -= 1;
                    break;
                case Direction.Up:
                    atomPosition.X += 1;
                    break;
            }

            return atomPosition;
        }

        private Point NewPosition(Point coordinates, Direction direction)
        {
            Point newCoordinates = coordinates;

            switch (direction)
            {
                case Direction.Right:
                    while (newCoordinates.Y < currentLevel.Board.ColumnsCount)
                    {
                        if (!currentLevel.Board[newCoordinates.X, newCoordinates.Y].IsEmpty)
                            break;

                        newCoordinates.Y++;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case Direction.Left:
                    while (newCoordinates.Y >= 0)
                    {
                        if (!currentLevel.Board[newCoordinates.X, newCoordinates.Y].IsEmpty)
                            break;

                        newCoordinates.Y--;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case Direction.Down:
                    while (newCoordinates.X < currentLevel.Board.RowsCount)
                    {
                        if (!currentLevel.Board[newCoordinates.X, newCoordinates.Y].IsEmpty)
                            break;

                        newCoordinates.X++;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case Direction.Up:
                    while (newCoordinates.X >= 0)
                    {
                        if (!currentLevel.Board[newCoordinates.X, newCoordinates.Y].IsEmpty)
                            break;

                        newCoordinates.X--;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
            }

            return newCoordinates;
        }

        private void ClearBoard()
        {
            for (int x = 0; x < currentLevel.Board.RowsCount; x++)
            {
                for (int y = 0; y < currentLevel.Board.ColumnsCount; y++)
                {
                    //switch (currentLevel.Board[x, y].Type)
                    //{
                    //    case TileType.Down:
                    //    case TileType.Up:
                    //    case TileType.Right:
                    //    case TileType.Left:
                    //        currentLevel.Board[x, y].Type = TileType.Empty;
                    //        break;
                    //}

                    currentLevel.Board[x, y].IsSelected = false;
                    currentLevel.Board[x, y].Movements = Direction.None;
                }
            }
        }

        private Texture2D GetTileTexture(string tileType, bool isSelected)
        {
            Texture2D tile = null;

            switch (tileType)
            {
                case "Wall":
                    tile = wallTexture;
                    break;
                case "Empty":
                    tile = emptyTexture;
                    break;
                case "Carbon":
                    tile = isSelected ? carbonSelectedTexture : carbonTexture;
                    break;
                case "Oxygen":
                    tile = isSelected ? oxygenSelectedTexture : oxygenTexture;
                    break;
                case "Hydrogen":
                    tile = isSelected ? hydrogenSelectedTexture : hydrogenTexture;
                    break;
                case "Left":
                case "Right":
                case "Down":
                case "Up":
                    tile = arrowTexture;
                    break;
            }

            return tile;
        }

        private void DrawBoard(SpriteBatch spriteBach, BoardCollection<BoardTileViewModel> board, bool drawEmptyTiles = false)
        {
            bool drawEmpty = false;

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    Texture2D tile = GetTileTexture(board[i, j].Asset, board[i, j].IsSelected);
                    drawEmpty = true;
                    float RotationAngle = 0;
                    Vector2 origin = new Vector2();

                    switch (board[i, j].Movements)
                    {
                        //case TileType.Wall:
                        //    drawEmpty = false;
                        //    break;
                        case Direction.Down:
                            RotationAngle = MathHelper.Pi;
                            origin.X = tile.Width;
                            origin.Y = tile.Height;
                            break;
                        case Direction.Left:
                            RotationAngle = -1 * MathHelper.Pi / 2;
                            origin.X = tile.Width;
                            origin.Y = 0;
                            break;
                        case Direction.Right:
                            RotationAngle = MathHelper.Pi / 2;
                            origin.Y = tile.Height;
                            break;
                    }

                    if (drawEmpty && drawEmptyTiles)
                        spriteBatch.Draw(emptyTexture, new Rectangle((int)board[i, j].RenderPosition.X, (int)board[i, j].RenderPosition.Y, TileWidth, TileHeight), Color.White);

                    if (tile != null)
                    {
                        spriteBatch.Draw(tile, new Rectangle((int)board[i, j].RenderPosition.X, (int)board[i, j].RenderPosition.Y, TileWidth, TileHeight), null, Color.White, RotationAngle, origin, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
