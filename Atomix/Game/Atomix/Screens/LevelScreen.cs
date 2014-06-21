using Atomix.Components;
using Atomix.Components.Common;
using Atomix.Components.Kinect;
using Atomix.ViewModel;
using AtomixData;
using Kinectomix.Logic;
using Kinectomix.Logic.Game;
using Kinectomix.Xna.Components;
using Kinectomix.Xna.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Atomix
{
    /// <summary>
    /// Screen for current game level.
    /// </summary>
    public class LevelScreen : GameScreen
    {
        private TimeSpan _minimalHoverDuration = TimeSpan.FromSeconds(2);
        private int _activeTileOpacityDirection;
        private float _activeTileOpacity;
        private Texture2D wallTexture;
        private Texture2D emptyTexture;
        private Texture2D arrowTexture;
        private Texture2D activeTexture;
        private Texture2D idleTexture;
        private MouseState mouseState;
        private MouseState lastMouseState;
        private SoundEffect applause;
        private Vector2 boardPosition;
        private SpriteFont _normalFont;
        private SpriteFont _splashFont;
        private SpriteFont _levelFont;
        private Point activeAtomIndex = new Point(-1, -1);

        private int TileWidth = 64;
        private int TileHeight = 64;

        // Animation stuff
        protected bool isMovementAnimation = false;
        protected Vector2 atomPosition;
        protected Vector2 finalPosition;
        private Point destination;
        private string atomToMove;
        private MoveDirection moveDirection;
        private float atomSpeed = 400f;

        // Scoring
        private int moves;
        private DateTime gameStarted;
        private TimeSpan gameDuration;

        private bool isLevelFinished = false;

        private ContentManager _content;

        private Button _levelsButton;
        private Button _repeatButton;
        private Button _nextButton;
        private SwipeGesturesRecognizer _swipeGestures;
        private Level levelDefinition;
        private LevelViewModel level;
        private SpriteBatch spriteBatch;
        private Highscore highScore;
        private string _log = "";
        private KinectCircleCursor _cursor;

        public LevelScreen(Level currentLevel, SpriteBatch spriteBatch)
        {
            _swipeGestures = new SwipeGesturesRecognizer();
            levelDefinition = currentLevel;
            this.spriteBatch = spriteBatch;

            gameStarted = DateTime.Now;
        }

        public override void Initialize()
        {
            KinectCursor cursor = (ScreenManager.Game as AtomixGame).Cursor;

            if (cursor is KinectCircleCursor)
                _cursor = cursor as KinectCircleCursor;

            _pauseMessageBox = new KinectMessageBox(ScreenManager.Game, ScreenManager.InputProvider, cursor);
            _pauseMessageBox.Changed += pause_Changed;

            level = LevelViewModel.FromModel(levelDefinition, ScreenManager.GraphicsDevice);
            highScore = new Highscore(AtomixGame.HighscoreFile);

            _activeTileOpacity = 0.0f;
            _activeTileOpacityDirection = 1;

            Components.Add(_pauseMessageBox);

            _levelsButton = new Button(ScreenManager.Game, "levels");
            _levelsButton.InputProvider = ScreenManager.InputProvider;
            _levelsButton.IsVisible = false;

            _repeatButton = new Button(ScreenManager.Game, "play again");
            _repeatButton.InputProvider = ScreenManager.InputProvider;
            _repeatButton.IsVisible = false;

            _nextButton = new Button(ScreenManager.Game, "continue");
            _nextButton.InputProvider = ScreenManager.InputProvider;
            _nextButton.IsVisible = false;

            Components.Add(_levelsButton);
            Components.Add(_repeatButton);
            Components.Add(_nextButton);

            base.Initialize();
        }

        private void pause_Changed(object sender, MessageBoxEventArgs e)
        {
            _pauseMessageBox.Hide();
        }

        public override void LoadContent()
        {
            int boardHeight = level.Board.RowsCount * TileHeight;

            int startX = ScreenManager.GraphicsDevice.Viewport.Bounds.Width - (level.Board.ColumnsCount * TileWidth) - 1 * TileWidth;
            int startY = ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - boardHeight / 2;

            boardPosition = new Vector2(startX, startY);

            CalculateBoardTilePositions(boardPosition, level.Board);

            //int offset = (int)boardPosition.X + level.Board.ColumnsCount * TileWidth + TileWidth;
            int offset = 30;
            int moleculeWidth = level.Molecule.ColumnsCount * TileWidth;
            int moleculeHeight = level.Molecule.RowsCount * TileHeight;
            //int posX = (ScreenManager.GraphicsDevice.Viewport.Bounds.Width - offset) / 2 - moleculeWidth / 2;
            int posX = (offset); // / 2 - moleculeWidth / 2;
            int posY = ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - moleculeHeight / 2;

            CalculateBoardTilePositions(new Vector2(offset + posX, posY), level.Molecule);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            wallTexture = _content.Load<Texture2D>("Board/Wall");
            emptyTexture = _content.Load<Texture2D>("Board/Empty");
            arrowTexture = _content.Load<Texture2D>("Board/Arrow");
            activeTexture = _content.Load<Texture2D>("Board/Active");
            applause = _content.Load<SoundEffect>("Sounds/Applause");
            _normalFont = _content.Load<SpriteFont>("Fonts/Normal");
            _splashFont = _content.Load<SpriteFont>("Fonts/Splash");
            _levelFont = _content.Load<SpriteFont>("Fonts/LevelName");
            idleTexture = _content.Load<Texture2D>("Idle");

            
            _levelsButton.Font = _normalFont;
            _levelsButton.Selected += _levelsButton_Selected;

            _repeatButton.Font = _normalFont;
            _repeatButton.Selected += _repeatButton_Selected;

            _nextButton.Font = _normalFont;
            _nextButton.Selected += _nextButton_Selected;

            _pauseMessageBox.Font = _normalFont;

            base.LoadContent();
        }

        void _nextButton_Selected(object sender, EventArgs e)
        {
            GameScreen gameScreen = null;

            // Load next level
            Level newLevel = AtomixGame.State.SwitchToNextLevel();

            if (newLevel == null)
            {
                // We are on last level -> go to main screen
                gameScreen = new StartScreen(spriteBatch);
            }
            else
            {
                gameScreen = new LevelScreen(newLevel, spriteBatch);
            }

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        void _repeatButton_Selected(object sender, EventArgs e)
        {
            GameScreen gameScreen = null;

            // Load current level again
            Level newLevel = AtomixGame.State.GetCurrentLevel();
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

            base.UnloadContent();
        }

        BoardTileViewModel currentlyHoveredTile = null;
        DateTime lastHoveredTileTime = DateTime.MinValue;
        private KeyboardState _previousKeyboardState;
        private KinectMessageBox _pauseMessageBox;

        private void PauseGame()
        {
            _pauseMessageBox.Show("game paused");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape) == true && _previousKeyboardState.IsKeyDown(Keys.Escape) == false)
                PauseGame();

            _previousKeyboardState = state;

            bool clickOccurred = false;
            bool isGestureDetected = false;

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

            GesturesState gesturesState = Gestures.GetState();
            if (gesturesState.RecognizedGestures != null && gesturesState.RecognizedGestures.Count() > 0)
            //if (gesturesState.IsGestureRecognized(GestureType.RightHandUp))
            {
                clickOccurred = true;
                isGestureDetected = true;
                _log = "Gestures: " + gesturesState.RecognizedGestures.Count().ToString() + " / " + gesturesState.RecognizedGestures.ToArray()[0].Gesture.Name;
            }

            KinectCursor cursor = ((AtomixGame)ScreenManager.Game).Cursor;

            if (isLevelFinished)
            {
                _repeatButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2 - _repeatButton.Width - 30, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _repeatButton.IsVisible = true;
                _levelsButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _levelsButton.IsVisible = true;
                _nextButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelsButton.Width / 2 + _nextButton.Width + 30, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 40);
                _nextButton.IsVisible = true;
                return;
            }

            gameDuration = DateTime.Now - gameStarted;

            if (isMovementAnimation)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 destinationPosition = level.Board[destination.X, destination.Y].RenderPosition;

                if (atomPosition.X == destinationPosition.X)
                {
                    int direction = moveDirection == MoveDirection.Up ? -1 : 1;
                    atomPosition.Y += direction * atomSpeed * elapsed;
                }

                if (atomPosition.Y == destinationPosition.Y)
                {
                    int direction = moveDirection == MoveDirection.Left ? -1 : 1;
                    atomPosition.X += direction * atomSpeed * elapsed;
                }

                Rectangle tile = new Rectangle((int)destinationPosition.X, (int)destinationPosition.Y, TileWidth / 4, TileHeight / 4);
                if (tile.Contains((int)atomPosition.X, (int)atomPosition.Y))
                {
                    isMovementAnimation = false;
                    level.Board[destination.X, destination.Y].Asset = atomToMove;
                    level.Board[destination.X, destination.Y].IsEmpty = false;

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

            // Detect hover
            Point mousePosition = cursor.IsHandTracked ?
                new Point((int)cursor.HandPosition.X, (int)cursor.HandPosition.Y) :
                new Point(mouseState.X, mouseState.Y);

            // Clear selection
            foreach (BoardTileViewModel tile in level.Board.Where(t => t != null && t.IsHovered == true))
            {
                tile.IsHovered = false;
            }

            BoardTileViewModel newHoveredTileBefore = null;
            DateTime lastHoveredTileTimeBefore = lastHoveredTileTime;
            int k = -1;
            int l = -1;

            for (int i = 0; i < level.Board.RowsCount; i++)
            {
                for (int j = 0; j < level.Board.ColumnsCount; j++)
                {
                    if (level.Board[i, j] != null)
                    {
                        if (level.Board[i, j].RenderRectangle.Contains(mousePosition))
                        {
                            if (cursor.IsHandTracked)
                            {
                                // If we are tracking cursor via Kinect, we use some affinity
                                if (level.Board[i, j].IsFixed == false)
                                {
                                    newHoveredTileBefore = level.Board[i, j];
                                    k = i;
                                    l = j;
                                }
                                else
                                {
                                    int m, n;
                                    BoardTileViewModel tile = GetNeigbourMoleculeTile(level.Board, i, j, out m, out n);

                                    if (tile != null)
                                    {
                                        newHoveredTileBefore = tile;
                                        k = m;
                                        l = n;
                                    }
                                }
                            }
                            else
                            {
                                // In normal way (mouse) we use direct positions
                                newHoveredTileBefore = level.Board[i, j];
                                k = i;
                                l = j;
                            }
                        }
                    }
                }
            }

            if (newHoveredTileBefore != currentlyHoveredTile)
            {
                lastHoveredTileTime = DateTime.Now;
                currentlyHoveredTile = newHoveredTileBefore;

                if (cursor.IsHandTracked) // Only when is tracked hand via Kinect clear selected tiles.
                {
                    ClearBoard();
                }

                if (_cursor != null)
                    _cursor.Progress = 0;
            }

            if (currentlyHoveredTile != null)
            {
                currentlyHoveredTile.IsHovered = true;

                if (currentlyHoveredTile.IsFixed == false)
                {
                    TimeSpan elapsedTime = DateTime.Now - lastHoveredTileTime;

                    if (_cursor != null)
                    {
                        _cursor.Progress = elapsedTime.TotalMilliseconds / _minimalHoverDuration.TotalMilliseconds;
                    }

                    if (elapsedTime > _minimalHoverDuration)
                    {
                        ClearBoard();

                        currentlyHoveredTile.IsSelected = true;

                        activeAtomIndex = new Point(k, l);
                        PrepareAvailableTileMovements(level.Board, k, l);

                        if (_cursor != null)
                            _cursor.Progress = 0;

                        // prepare for gesture
                        _swipeGestures.Start(cursor.HandRealPosition, 0.05);
                        _isGestureCandidate = true;
                    }
                }
            }

            // Detect clicks
            if (clickOccurred || isGestureDetected || cursor.IsHandClosed)
            {
                Point activityPosition;
                if (clickOccurred && !isGestureDetected)
                {
                    activityPosition = new Point(mouseState.X, mouseState.Y);
                }
                else
                {
                    activityPosition = new Point((int)cursor.HandPosition.X, (int)cursor.HandPosition.Y);
                }

                // Find nearest point
                Vector2 mPosition = new Vector2(boardPosition.X, boardPosition.Y);

                // Reset active atom to none
                activeAtomIndex = new Point(-1, -1);

                for (int i = 0; i < level.Board.RowsCount; i++)
                {
                    for (int j = 0; j < level.Board.ColumnsCount; j++)
                    {
                        if (level.Board[i, j] != null)
                        {
                            Rectangle tile = new Rectangle((int)mPosition.X, (int)mPosition.Y, TileWidth, TileHeight);
                            if (clickOccurred && tile.Contains(activityPosition))
                            {
                                if (level.Board[i, j].IsFixed == false)
                                {
                                    ClearBoard();

                                    level.Board[i, j].IsSelected = true;
                                    activeAtomIndex = new Point(i, j);

                                    PrepareAvailableTileMovements(level.Board, i, j);
                                }
                                else if (level.Board[i, j].Asset == "Right" ||
                                         level.Board[i, j].Asset == "Left" ||
                                         level.Board[i, j].Asset == "Up" ||
                                         level.Board[i, j].Asset == "Down")
                                {
                                    MoveDirection direction = GetDirectionFromAsset(level.Board[i, j].Asset);
                                    Point coordinates = new Point(i, j);
                                    Point atomCoordinates = GetAtomPosition(coordinates, direction);

                                    ProcessTileMove(atomCoordinates, direction);

                                    ClearBoard();
                                }
                            }
                        }

                        mPosition.X += TileWidth;
                    }

                    mPosition.X = boardPosition.X;
                    mPosition.Y += TileHeight;
                }
            }

            if (_isGestureCandidate)
            {
                SwipeGesture recognizedGesture;
                _isGestureCandidate = _swipeGestures.ProcessPosition(cursor.HandRealPosition, out recognizedGesture);
                if (recognizedGesture != null)
                {
                    MoveDirection direction = SwipeToMoveDirection(recognizedGesture.Direction);

                    ProcessTileMove(activeAtomIndex, direction);

                    ClearBoard();

                    _log = "Detected swipe " + recognizedGesture.Direction.ToString();
                }
            }

            if (!isMovementAnimation && activeAtomIndex.X != -1 && activeAtomIndex.Y != -1)
            {
                // We have selected atom which is not moved 

                // update glowing
                _activeTileOpacity += 0.02f * _activeTileOpacityDirection;
                if (_activeTileOpacity > 1.0)
                    _activeTileOpacityDirection = -1;
                if (_activeTileOpacity < 0.5)
                    _activeTileOpacityDirection = 1;

                level.Board[activeAtomIndex.X, activeAtomIndex.Y].Opacity = _activeTileOpacity;


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

        private void ProcessTileMove(Point atomCoordinates, MoveDirection direction)
        {
            Point newCoordinates = GetNewAtomPosition(atomCoordinates, direction);

            BoardTileViewModel atom = level.Board[atomCoordinates.X, atomCoordinates.Y]; // remember atom

            // start animation
            isMovementAnimation = true;
            atomPosition = atom.RenderPosition;
            destination = newCoordinates;
            atomToMove = atom.Asset;
            moveDirection = direction;

            // instead of just switching do the animation
            level.Board[atomCoordinates.X, atomCoordinates.Y] = level.Board[newCoordinates.X, newCoordinates.Y]; //switch empty place to the atom
            level.Board[newCoordinates.X, newCoordinates.Y] = atom; // new field will be atom
            level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty = true; // but now will be rendered as empty
            level.Board[newCoordinates.X, newCoordinates.Y].Asset = "Empty";// but now will be rendered as empty

            CalculateBoardTilePositions(boardPosition, level.Board);

            moves += 1;
        }

        private MoveDirection SwipeToMoveDirection(SwipeDirection swipeDirection)
        {
            switch (swipeDirection)
            {
                case SwipeDirection.Left:
                    return MoveDirection.Left;
                case SwipeDirection.Right:
                    return MoveDirection.Right;
                case SwipeDirection.Up:
                    return MoveDirection.Up;
                case SwipeDirection.Down:
                    return MoveDirection.Down;
            }

            return MoveDirection.None;
        }

        private void PrepareAvailableTileMovements(TilesCollection<BoardTileViewModel> board, int i, int j)
        {
            board[i, j].Movements = MoveDirection.None;

            //zjistit jakymi smery se muze pohnout
            if (level.CanGoUp(i, j))
            {
                board[i, j].Movements |= MoveDirection.Up;
                board[i - 1, j].Asset = "Up";
            }
            if (level.CanGoDown(i, j))
            {
                board[i, j].Movements |= MoveDirection.Down;
                board[i + 1, j].Asset = "Down";
            }
            if (level.CanGoLeft(i, j))
            {
                board[i, j].Movements |= MoveDirection.Left;
                board[i, j - 1].Asset = "Left";
            }
            if (level.CanGoRight(i, j))
            {
                board[i, j].Movements |= MoveDirection.Right;
                board[i, j + 1].Asset = "Right";
            }
        }

        private BoardTileViewModel GetNeigbourMoleculeTile(TilesCollection<BoardTileViewModel> board, int x, int y, out int k, out int l)
        {
            BoardTileViewModel tile = null;
            k = -1;
            l = -1;

            for (int i = -1; i <= 1; i++) // erows
            {
                for (int j = -1; j <= 1; j++) // columns
                {
                    if (x + i > 0 && x + i < board.RowsCount &&
                        y + j > 0 && y + j < board.ColumnsCount &&
                        board[x + i, y + j] != null)
                    {
                        if (board[x + i, y + j].IsFixed == false)
                        {
                            tile = board[x + i, y + j];
                            k = x + i;
                            l = y + j;
                        }
                    }
                }
            }

            return tile;
        }

        private MoveDirection GetDirectionFromAsset(string asset)
        {
            switch (asset)
            {
                case "Right":
                    return MoveDirection.Right;
                case "Left":
                    return MoveDirection.Left;
                case "Up":
                    return MoveDirection.Up;
                case "Down":
                    return MoveDirection.Down;
            }

            return MoveDirection.None;
        }

        bool _isGestureCandidate = false;
        bool isToLeftGesture = false;
        bool isToRightGesture = false;
        double gestureAccumulatedDistanceX = 0;
        double gestureAxeTolerance = 10;
        double gestureThreshold = 20;
        MoveDirection gestureDirection;
        Vector2 lastHandPosition;
        Vector2 startHandPosition;
        Vector3 startHandPositionReal;
        Vector3 lastHandPositionReal;
        double lastDiff;

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            string levelName = string.IsNullOrEmpty(level.Name) == false ? level.Name : "game level";

            spriteBatch.DrawString(_levelFont, levelName, new Vector2(21, 21), Color.Black * 0.8f);
            spriteBatch.DrawString(_levelFont, levelName, new Vector2(20, 20), Color.Red);

            spriteBatch.DrawString(_normalFont, _log, new Vector2(20, 600), Color.Red);

            DrawBoard(spriteBatch, level.Board, true);
            DrawBoard(spriteBatch, level.Molecule);

            spriteBatch.DrawString(_normalFont, string.Format("Score: {0}", moves), new Vector2(21, 101), Color.Black * 0.8f);
            spriteBatch.DrawString(_normalFont, string.Format("Score: {0}", moves), new Vector2(20, 100), Color.Red);

            spriteBatch.DrawString(_normalFont, string.Format("Time: {0}", gameDuration.ToString(@"mm\:ss")), new Vector2(21, 141), Color.Black * 0.8f);
            spriteBatch.DrawString(_normalFont, string.Format("Time: {0}", gameDuration.ToString(@"mm\:ss")), new Vector2(20, 140), Color.Red);

            if (isMovementAnimation)
            {
                spriteBatch.Draw(GetTileTexture(atomToMove), new Rectangle((int)atomPosition.X, (int)atomPosition.Y, TileWidth, TileHeight), Color.White);
            }

            if (isLevelFinished)
            {
                spriteBatch.Draw(idleTexture, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Bounds.Width, ScreenManager.GraphicsDevice.Viewport.Bounds.Height), Color.White);
                spriteBatch.Draw(wallTexture, new Rectangle(0, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 100, ScreenManager.GraphicsDevice.Viewport.Bounds.Width, 230), Color.Brown);

                string name = "level completed";
                Vector2 size = _splashFont.MeasureString(name);

                spriteBatch.DrawString(_splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 100), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void CalculateBoardTilePositions(Vector2 startPosition, TilesCollection<BoardTileViewModel> board)
        {
            Vector2 currentPosition = new Vector2(startPosition.X, startPosition.Y);

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    if (board[i, j] != null)
                    {
                        board[i, j].RenderPosition = currentPosition;
                        board[i, j].RenderRectangle = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, TileWidth, TileHeight);
                    }

                    currentPosition.X += TileWidth;
                }

                currentPosition.X = startPosition.X;
                currentPosition.Y += TileHeight;
            }
        }

        private bool CheckFinish()
        {
            for (int i = 0; i < level.Board.RowsCount; i++)
            {
                for (int j = 0; j < level.Board.ColumnsCount; j++)
                {
                    // We expect match
                    bool isMatch = true;

                    for (int x = 0; x < level.Molecule.RowsCount; x++)
                    {
                        for (int y = 0; y < level.Molecule.ColumnsCount; y++)
                        {
                            // If we have empty tile in definition it is like a wildcard that matches everything
                            if (level.Molecule[x, y] == null || level.Molecule[x, y].IsEmpty)
                                continue;

                            // TODO overflow
                            if (i + x >= level.Board.RowsCount || j + y >= level.Board.ColumnsCount || level.Board[i + x, j + y] == null || level.Board[i + x, j + y].Asset != level.Molecule[x, y].Asset)
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

        private Point GetAtomPosition(Point directionTilePosition, MoveDirection direction)
        {
            Point atomPosition = directionTilePosition;

            switch (direction)
            {
                case MoveDirection.Right:
                    atomPosition.Y -= 1;
                    break;
                case MoveDirection.Left:
                    atomPosition.Y += 1;
                    break;
                case MoveDirection.Down:
                    atomPosition.X -= 1;
                    break;
                case MoveDirection.Up:
                    atomPosition.X += 1;
                    break;
            }

            return atomPosition;
        }

        private Point GetNewAtomPosition(Point coordinates, MoveDirection direction)
        {
            Point newCoordinates = coordinates;

            switch (direction)
            {
                case MoveDirection.Right:
                    newCoordinates.Y++; // We started on the atom -> move one right
                    while (newCoordinates.Y < level.Board.ColumnsCount)
                    {
                        if (level.Board[newCoordinates.X, newCoordinates.Y] == null || level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.Y++;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Left:
                    newCoordinates.Y--; // We started on the atom -> move one left
                    while (newCoordinates.Y >= 0)
                    {
                        if (level.Board[newCoordinates.X, newCoordinates.Y] == null || level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.Y--;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Down:
                    newCoordinates.X++; // We started on the atom -> move one bottom
                    while (newCoordinates.X < level.Board.RowsCount)
                    {
                        if (level.Board[newCoordinates.X, newCoordinates.Y] == null || level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.X++;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Up:
                    newCoordinates.X--; // We started on the atom -> move one up
                    while (newCoordinates.X >= 0)
                    {
                        if (level.Board[newCoordinates.X, newCoordinates.Y] == null || level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.X--;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
            }

            return newCoordinates;
        }

        //TODO -> into the ViewModel
        private void ClearBoard()
        {
            for (int x = 0; x < level.Board.RowsCount; x++)
            {
                for (int y = 0; y < level.Board.ColumnsCount; y++)
                {
                    if (level.Board[x, y] == null)
                        continue;

                    switch (level.Board[x, y].Asset)
                    {
                        case "Down":
                        case "Up":
                        case "Right":
                        case "Left":
                            level.Board[x, y].Asset = "Empty";
                            break;
                    }

                    level.Board[x, y].IsSelected = false;
                    level.Board[x, y].Movements = MoveDirection.None;
                    level.Board[x, y].Opacity = 1;
                }
            }
        }

        private Texture2D GetTileTexture(string asset)
        {
            Texture2D tileTexture = null;

            switch (asset)
            {
                case "Empty":
                    tileTexture = emptyTexture;
                    break;
                case "Left":
                case "Right":
                case "Down":
                case "Up":
                    tileTexture = arrowTexture;
                    break;
                default:
                    tileTexture = level.Assets[asset];
                    break;
            }

            return tileTexture;
        }

        private void DrawBoard(SpriteBatch spriteBach, TilesCollection<BoardTileViewModel> board, bool drawEmptyTiles = false)
        {
            bool drawEmpty = false;

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    if (board[i, j] == null)
                        continue;

                    Texture2D tile = GetTileTexture(board[i, j].AssetCode);
                    drawEmpty = true;
                    float RotationAngle = 0;
                    Vector2 origin = new Vector2();

                    switch (board[i, j].AssetCode)
                    {
                        case "Down":
                            RotationAngle = MathHelper.Pi;
                            origin.X = tile.Width;
                            origin.Y = tile.Height;
                            break;
                        case "Left":
                            RotationAngle = -1 * MathHelper.Pi / 2;
                            origin.X = tile.Width;
                            origin.Y = 0;
                            break;
                        case "Right":
                            RotationAngle = MathHelper.Pi / 2;
                            origin.Y = tile.Height;
                            break;
                    }

                    if (!drawEmptyTiles && board[i, j].IsEmpty)
                    {
                        drawEmpty = false;
                        tile = null;
                    }

                    if (drawEmpty && drawEmptyTiles)
                        spriteBatch.Draw(emptyTexture, new Rectangle((int)board[i, j].RenderPosition.X, (int)board[i, j].RenderPosition.Y, TileWidth, TileHeight), Color.White);

                    if (tile != null)
                    {
                        spriteBatch.Draw(tile, new Rectangle((int)board[i, j].RenderPosition.X, (int)board[i, j].RenderPosition.Y, TileWidth, TileHeight), null, Color.White * board[i, j].Opacity, RotationAngle, origin, SpriteEffects.None, 0f);

                        if (board[i, j].IsHovered && (board[i, j].IsFixed == false || board[i, j].IsEmpty == true))
                        {
                            spriteBach.Draw(activeTexture, new Rectangle((int)board[i, j].RenderPosition.X, (int)board[i, j].RenderPosition.Y, TileWidth, TileHeight), Color.White);
                        }
                    }
                }
            }
        }
    }
}
