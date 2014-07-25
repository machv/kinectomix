using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Mach.Xna.Input.Extensions;
using Mach.Xna.ScreenManagement;
using Mach.Xna.Components;
using Mach.Xna.Kinect.Components;
using Mach.Xna.Kinect.Gestures;
using Mach.Kinectomix.Logic;
using Mach.Kinectomix.ViewModel;
using Mach.Kinect.Gestures;
using Mach.Xna;

namespace Mach.Kinectomix.Screens
{
    /// <summary>
    /// Screen for current game level.
    /// </summary>
    public class LevelScreen : GameScreen
    {
        private const int TileWidth = 60;
        private const int TileHeight = 60;
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
        private SoundEffect _applause;
        private Vector2 boardPosition;
        private SpriteFont _normalFont;
        private SpriteFont _splashFont;
        private SpriteFont _levelFont;
        private SpriteFont _timeFont;
        private Point activeAtomIndex = new Point(-1, -1);
        private ContentManager _content;
        private KinectButton _levelsButton;
        private KinectButton _repeatButton;
        private KinectButton _nextButton;
        private SwipeRecognizer _swipeGestures;
        private Level _levelDefinition;
        private LevelViewModel _level;
        private SpriteBatch _spriteBatch;
        private KinectCircleCursor _cursor;
        private SpriteButton _pauseButton;
        private LevelHighscore _highscore;
        private Button _levelNameButton;

        private int _leftBoxEndX = 400;
        private int _leftMargin = 55;
        private int _topOffsetMain = 120;
        private Texture2D _backgroundTexture;

        // Animation stuff
        protected bool isMovementAnimation = false;
        protected Vector2 atomPosition;
        protected Vector2 finalPosition;
        private Point destination;
        private string atomToMove;
        private MoveDirection moveDirection;
        private float atomSpeed = 400f;
        private float _renderAtomScale = 1;

        // Scoring
        private int _moves;
        private DateTime _gameStarted;
        private TimeSpan _gameDuration;
        private DateTime _lastDate;

        private bool _isLevelFinished = false;

        // Message boxes
        BoardTileViewModel currentlyHoveredTile = null;
        DateTime lastHoveredTileTime = DateTime.MinValue;
        private KeyboardState _previousKeyboardState;
        private KinectMessageBox _pauseMessageBox;
        private KinectMessageBox _finishedMessageBox;
        private bool _isPaused = false;
        private KinectButton[] _finishButtons;
        private KinectButton[] _pauseButtons;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelScreen"/> class.
        /// </summary>
        /// <param name="currentLevel">The level to play.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        public LevelScreen(Level currentLevel, SpriteBatch spriteBatch)
        {
            _swipeGestures = new SwipeRecognizer();
            _levelDefinition = currentLevel;
            _spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Occurs when this screen is activated.
        /// </summary>
        public override void Activated()
        {
            _gameStarted = DateTime.Now;

            base.Activated();
        }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        public override void Initialize()
        {
            _activeTileOpacity = 0.0f;
            _activeTileOpacityDirection = 1;
            _level = LevelViewModel.FromModel(_levelDefinition, ScreenManager.GraphicsDevice);
            _highscore = KinectomixGame.State.GetCurrentLevelHighscore();

            KinectCursor cursor = (ScreenManager.Game as KinectomixGame).Cursor;

            if (cursor is KinectCircleCursor)
                _cursor = cursor as KinectCircleCursor;

            string levelName = string.IsNullOrEmpty(_level.Name) == false ? _level.Name : Resources.LevelScreenResources.DefaultLevelName;
            _levelNameButton = new Button(ScreenManager.Game, levelName);

            _pauseButton = new KinectSpriteButton(ScreenManager.Game, _cursor);
            _pauseButton.Selected += Pause_Selected;

            _pauseMessageBox = new KinectMessageBox(ScreenManager.Game, ScreenManager.InputProvider, cursor);
            _pauseMessageBox.Changed += pause_Changed;

            _finishedMessageBox = new KinectMessageBox(ScreenManager.Game, ScreenManager.InputProvider, cursor);
            _finishedMessageBox.Changed += _finishedMessageBox_Changed;

            _finishButtons = new KinectButton[] {
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.Levels) { Tag = MessageBoxResult.Custom1 },
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.PlayAgain) { Tag = MessageBoxResult.Custom2 },
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.NextLevel) { Tag = MessageBoxResult.Custom3 },
            };

            foreach (Button button in _finishButtons)
                button.Initialize();

            _pauseButtons = new KinectButton[] {
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.MainMenu) { Tag = MessageBoxResult.Custom1 },
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.PlayAgain) { Tag = MessageBoxResult.Custom2 },
                new KinectButton(ScreenManager.Game, _cursor, Resources.LevelScreenResources.ContinueGame) { Tag = MessageBoxResult.Custom3 },
            };

            foreach (Button button in _pauseButtons)
                button.Initialize();

            Components.Add(_levelNameButton);
            Components.Add(_pauseButton);
            Components.Add(_finishedMessageBox);
            Components.Add(_pauseMessageBox);

            _lastDate = DateTime.Now;

            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundTexture = _content.Load<Texture2D>("Backgrounds/LevelShort");

            int maximalWidth;
            int maximalHeight;
            float scale;

            // Board
            scale = 1;
            maximalWidth = 750;
            maximalHeight = 660;
            int boardWidth = _level.Board.ColumnsCount * TileHeight;
            int boardHeight = _level.Board.RowsCount * TileHeight;

            if (boardWidth > maximalWidth)
            {
                scale = (float)maximalWidth / boardWidth;
            }

            if (boardHeight > maximalHeight)
            {
                float heightScale = (float)maximalHeight / boardHeight;
                if (heightScale < scale)
                {
                    scale = heightScale;
                }
            }

            _renderAtomScale = scale;
            boardWidth = (int)(boardWidth * scale);
            boardHeight = (int)(boardHeight * scale);

            int startX = maximalWidth / 2 - boardWidth / 2;
            startX = 440 + 30 * (startX / 30);

            int startY = maximalHeight / 2 - boardHeight / 2;
            startY = 30 + 30 * (startY / 30);

            boardPosition = new Vector2(startX, startY);

            CalculateBoardTilePositions(boardPosition, _level.Board, scale);

            int moleculeWidth = _level.Molecule.ColumnsCount * TileWidth;
            int moleculeHeight = _level.Molecule.RowsCount * TileHeight;
            int maxWidth = 355;
            int maxHeight = 325;
            scale = 1;
            int offsetY = _topOffsetMain;

            if (moleculeWidth > maxWidth)
            {
                scale = (float)maxWidth / moleculeWidth;
            }

            if (moleculeHeight > maxHeight)
            {
                float heightScale = (float)maxHeight / moleculeHeight;
                if (heightScale < scale)
                {
                    scale = heightScale;
                }
            }

            moleculeWidth = (int)(moleculeWidth * scale);
            moleculeHeight = (int)(moleculeHeight * scale);

            int posX = 45 + maxWidth / 2 - moleculeWidth / 2;
            int posY = _topOffsetMain + maxHeight / 2 - moleculeHeight / 2;

            CalculateBoardTilePositions(new Vector2(posX, posY), _level.Molecule, scale);

            wallTexture = _content.Load<Texture2D>("Board/Wall");
            emptyTexture = _content.Load<Texture2D>("Board/Empty");
            arrowTexture = _content.Load<Texture2D>("Board/Arrow");
            activeTexture = _content.Load<Texture2D>("Board/Active");
            _applause = _content.Load<SoundEffect>("Sounds/Applause");
            _normalFont = _content.Load<SpriteFont>("Fonts/Normal");
            _splashFont = _content.Load<SpriteFont>("Fonts/Splash");
            _levelFont = _content.Load<SpriteFont>("Fonts/LevelName");
            _timeFont = _content.Load<SpriteFont>("Fonts/Time");
            idleTexture = _content.Load<Texture2D>("Idle");

            _levelNameButton.Font = _levelFont;
            _levelNameButton.IsEnabled = false;
            _levelNameButton.DisabledForeground = KinectomixGame.BrickColor;
            _levelNameButton.DisabledBackground = Color.Transparent;
            _levelNameButton.TextScrolling = TextScrolling.Slide;
            _levelNameButton.TextAlignment = TextAlignment.Left;
            _levelNameButton.Position = new Vector2(50, 30);
            _levelNameButton.Width = 360;
            _levelNameButton.Height = 60;
            _levelNameButton.BorderThickness = 0;

            _pauseButton.Texture = _content.Load<Texture2D>("Buttons/PauseNormal");
            _pauseButton.Focused = _content.Load<Texture2D>("Buttons/PauseFocused");
            _pauseButton.Width = 60;
            _pauseButton.Height = 60;
            _pauseButton.InputProvider = ScreenManager.InputProvider;

            _pauseMessageBox.Font = _normalFont;
            _finishedMessageBox.Font = _normalFont;

            InitializeButtons(_finishButtons);
            InitializeButtons(_pauseButtons);

            base.LoadContent();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void UnloadContent()
        {
            _content.Unload();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _pauseButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width - _pauseButton.Width - 30, 30);

            if (!_isPaused)
            {
                KeyboardState state = Keyboard.GetState();

                if (state.IsKeyDown(Keys.Escape) == true && _previousKeyboardState.IsKeyDown(Keys.Escape) == false)
                    PauseGame();

                _previousKeyboardState = state;

                bool clickOccurred = false;
                bool isGestureDetected = false;

                lastMouseState = mouseState;
                mouseState = Mouse.GetState();

                // Recognize a single click of the left mouse button
                if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                {
                    clickOccurred = true;
                }

                GesturesState gesturesState = Gestures.GetState();
                if (gesturesState.RecognizedGestures != null && gesturesState.RecognizedGestures.Count() > 0)
                //if (gesturesState.IsGestureRecognized(GestureType.RightHandUp))
                {
                    clickOccurred = true;
                    isGestureDetected = true;
                    //_log = "Gestures: " + gesturesState.RecognizedGestures.Count().ToString() + " / " + gesturesState.RecognizedGestures.ToArray()[0].Gesture.Name;
                }

                KinectCursor cursor = (ScreenManager.Game as KinectomixGame).Cursor;

                if (_isLevelFinished)
                {
                    if (_highscore == null)
                        _highscore = new LevelHighscore();

                    if (_highscore.UpdateIfBetter(_moves, _gameDuration))
                        KinectomixGame.State.SetCurrentLevelHighscore(_highscore);

                    return;
                }

                _gameDuration += DateTime.Now - _lastDate;
                _lastDate = DateTime.Now;

                if (isMovementAnimation)
                {
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Vector2 destinationPosition = _level.Board[destination.X, destination.Y].RenderPosition;

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

                    Rectangle tile = new Rectangle((int)destinationPosition.X, (int)destinationPosition.Y, (int)(TileWidth * _renderAtomScale) / 4, (int)(TileHeight * _renderAtomScale));
                    if (tile.Contains((int)atomPosition.X, (int)atomPosition.Y))
                    {
                        isMovementAnimation = false;
                        _level.Board[destination.X, destination.Y].Asset = atomToMove;
                        _level.Board[destination.X, destination.Y].IsEmpty = false;

                        // Animation finished -> check victory
                        bool isFinished = CheckFinish();
                        if (isFinished)
                        {
                            ProcessFinish();
                        }
                    }
                }
                else
                {
                    // Detect hover
                    Point mousePosition = cursor.IsHandTracked ?
                        new Point((int)cursor.Position.X, (int)cursor.Position.Y) :
                        new Point(mouseState.X, mouseState.Y);

                    // Clear selection
                    foreach (BoardTileViewModel tile in _level.Board.Where(t => t != null && t.IsHovered == true))
                    {
                        tile.IsHovered = false;
                    }

                    BoardTileViewModel newHoveredTileBefore = null;
                    DateTime lastHoveredTileTimeBefore = lastHoveredTileTime;
                    int k = -1;
                    int l = -1;

                    for (int i = 0; i < _level.Board.RowsCount; i++)
                    {
                        for (int j = 0; j < _level.Board.ColumnsCount; j++)
                        {
                            if (_level.Board[i, j] != null)
                            {
                                if (_level.Board[i, j].RenderRectangle.Contains(mousePosition))
                                {
                                    if (cursor.IsHandTracked)
                                    {
                                        // If we are tracking cursor via Kinect, we use some affinity
                                        if (_level.Board[i, j].IsFixed == false)
                                        {
                                            newHoveredTileBefore = _level.Board[i, j];
                                            k = i;
                                            l = j;
                                        }
                                        else
                                        {
                                            int m, n;
                                            BoardTileViewModel tile = GetNeigbourMoleculeTile(_level.Board, i, j, out m, out n);

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
                                        newHoveredTileBefore = _level.Board[i, j];
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

                        if (cursor.IsHandTracked && currentlyHoveredTile.IsFixed == false)
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
                                PrepareAvailableTileMovements(_level.Board, k, l);

                                if (_cursor != null)
                                    _cursor.Progress = 0;

                                // prepare for gesture
                                _swipeGestures.Start(cursor.HandPosition, 0.05);
                                _isGestureCandidate = true;
                            }
                        }
                    }

                    // Detect clicks
                    if (clickOccurred || isGestureDetected || cursor.IsHandStateActive)
                    {
                        Point activityPosition;
                        if (clickOccurred && !isGestureDetected)
                        {
                            activityPosition = new Point(mouseState.X, mouseState.Y);
                        }
                        else
                        {
                            activityPosition = new Point((int)cursor.Position.X, (int)cursor.Position.Y);
                        }

                        // Find nearest point
                        Vector2 mPosition = new Vector2(boardPosition.X, boardPosition.Y);

                        // Reset active atom to none
                        activeAtomIndex = new Point(-1, -1);

                        for (int i = 0; i < _level.Board.RowsCount; i++)
                        {
                            for (int j = 0; j < _level.Board.ColumnsCount; j++)
                            {
                                if (_level.Board[i, j] != null)
                                {
                                    Rectangle tile = new Rectangle((int)mPosition.X, (int)mPosition.Y, (int)(TileWidth * _renderAtomScale), (int)(TileHeight * _renderAtomScale));
                                    if (clickOccurred && _level.Board[i, j].RenderRectangle.Contains(activityPosition))
                                    {
                                        if (_level.Board[i, j].IsFixed == false)
                                        {
                                            ClearBoard();

                                            _level.Board[i, j].IsSelected = true;
                                            activeAtomIndex = new Point(i, j);

                                            PrepareAvailableTileMovements(_level.Board, i, j);
                                        }
                                        else if (_level.Board[i, j].Asset == "Right" ||
                                                 _level.Board[i, j].Asset == "Left" ||
                                                 _level.Board[i, j].Asset == "Up" ||
                                                 _level.Board[i, j].Asset == "Down")
                                        {
                                            MoveDirection direction = GetDirectionFromAsset(_level.Board[i, j].Asset);
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
                        _isGestureCandidate = _swipeGestures.ProcessPosition(cursor.HandPosition, out recognizedGesture);
                        if (recognizedGesture != null)
                        {
                            // Recognized swipe gesture
                            MoveDirection direction = SwipeToMoveDirection(recognizedGesture.Direction);

                            ProcessTileMove(activeAtomIndex, direction);

                            ClearBoard();
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

                        _level.Board[activeAtomIndex.X, activeAtomIndex.Y].Opacity = _activeTileOpacity;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, ScreenManager.Game.GraphicsDevice.Viewport.Bounds.Width, ScreenManager.Game.GraphicsDevice.Viewport.Bounds.Height), Color.White);

            DrawBoard(_spriteBatch, _level.Board, true);
            DrawBoard(_spriteBatch, _level.Molecule);

            string text = Resources.LevelScreenResources.Moves;
            var scoreSize = _normalFont.MeasureString(text);
            _spriteBatch.DrawStringWithShadow(_normalFont, text, new Vector2(55, 490), KinectomixGame.BrickColor);

            text = Resources.LevelScreenResources.Time;
            var timeSize = _normalFont.MeasureString(text);
            float dif = scoreSize.X - timeSize.X;

            _spriteBatch.DrawStringWithShadow(_normalFont, text, new Vector2(55, 595), KinectomixGame.BrickColor);

            _spriteBatch.DrawStringWithShadow(_timeFont, string.Format("{0}", _moves), new Vector2(55, 525), KinectomixGame.BrickColor);
            _spriteBatch.DrawStringWithShadow(_timeFont, string.Format("{0}", _gameDuration.ToString(@"mm\:ss")), new Vector2(55, 630), KinectomixGame.BrickColor);

            Vector2 textSize;
            int x;
            string textToRender;
            if (_highscore != null)
            {
                textToRender = Resources.LevelScreenResources.HighScore;
                textSize = _normalFont.MeasureString(textToRender);
                x = _leftBoxEndX - (int)textSize.X;
                _spriteBatch.DrawStringWithShadow(_normalFont, textToRender, new Vector2(x, 490), Color.Gray);

                textToRender = string.Format("{0}", _highscore.Moves);
                textSize = _timeFont.MeasureString(textToRender);
                x = _leftBoxEndX - (int)textSize.X;
                _spriteBatch.DrawStringWithShadow(_timeFont, textToRender, new Vector2(x, 525), Color.Gray);

                textToRender = string.Format("{0}", _highscore.Time.ToString(@"mm\:ss"));
                textSize = _timeFont.MeasureString(textToRender);
                x = _leftBoxEndX - (int)textSize.X;
                _spriteBatch.DrawStringWithShadow(_timeFont, textToRender, new Vector2(x, 630), Color.Gray);
            }

            if (isMovementAnimation)
            {
                _spriteBatch.Draw(GetTileTexture(atomToMove), new Rectangle((int)atomPosition.X, (int)atomPosition.Y, (int)(TileWidth * _renderAtomScale), (int)(TileHeight * _renderAtomScale)), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private void PauseGame()
        {
            _isPaused = true;
            _pauseButton.Freeze();

            _pauseMessageBox.Show(Resources.LevelScreenResources.GamePaused, _pauseButtons);
        }

        private void ProcessFinish()
        {
            _isPaused = true;
            _isLevelFinished = true;

            _applause.Play();
            _finishedMessageBox.Show(Resources.LevelScreenResources.LevelCompleted, _finishButtons);
        }

        private void Pause_Selected(object sender, EventArgs e)
        {
            PauseGame();
        }

        private void pause_Changed(object sender, MessageBoxEventArgs e)
        {
            switch (e.Result)
            {
                case MessageBoxResult.Custom1: // MainMenu
                    ShowStart();
                    break;
                case MessageBoxResult.Custom2: // PlayAgain
                    RestartCurrentLevel();
                    break;
                case MessageBoxResult.Custom3: // ContinueGame
                    _pauseMessageBox.Hide();
                    _pauseButton.Unfreeze();

                    _isPaused = false;
                    _lastDate = DateTime.Now;
                    break;
            }
        }

        private void _finishedMessageBox_Changed(object sender, MessageBoxEventArgs e)
        {
            switch (e.Result)
            {
                case MessageBoxResult.Custom1: // Levels
                    ShowLevels();

                    // Keep message box visible, user can navigate back
                    _pauseMessageBox.Show(Resources.LevelScreenResources.GamePaused, _pauseButtons);
                    break;
                case MessageBoxResult.Custom2: // PlayAgain
                    RestartCurrentLevel();
                    break;
                case MessageBoxResult.Custom3: // NextLevel
                    GoToNextLevel();
                    break;
            }
        }

        private void GoToNextLevel()
        {
            GameScreen gameScreen = null;

            // Load next level
            Level newLevel = KinectomixGame.State.SwitchToNextLevel();

            if (newLevel == null)
            {
                // We are on last level -> go to main screen
                gameScreen = new StartScreen(_spriteBatch);
            }
            else
            {
                gameScreen = new LevelScreen(newLevel, _spriteBatch);
            }

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void RestartCurrentLevel()
        {
            GameScreen gameScreen = null;

            // Load current level again
            Level newLevel = KinectomixGame.State.GetCurrentLevel();
            gameScreen = new LevelScreen(newLevel, _spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void ShowLevels()
        {
            GameScreen gameScreen = new LevelsScreen(_spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void ShowStart()
        {
            GameScreen gameScreen = new StartScreen(_spriteBatch);

            ScreenManager.Add(gameScreen);
            ScreenManager.Activate(gameScreen);
        }

        private void InitializeButtons(Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                button.Font = _normalFont;
                button.Width = 310;
                button.InputProvider = ScreenManager.InputProvider;
                button.Background = Color.Silver;
                button.Foreground = Color.White;
                button.ActiveBackground = Color.Black;
                button.ActiveForeground = Color.White;
            }
        }

        private void ProcessTileMove(Point atomCoordinates, MoveDirection direction)
        {
            Point newCoordinates = GetNewAtomPosition(atomCoordinates, direction);

            BoardTileViewModel atom = _level.Board[atomCoordinates.X, atomCoordinates.Y]; // remember atom

            // start animation
            isMovementAnimation = true;
            atomPosition = atom.RenderPosition;
            destination = newCoordinates;
            atomToMove = atom.Asset;
            moveDirection = direction;

            // instead of just switching do the animation
            _level.Board[atomCoordinates.X, atomCoordinates.Y] = _level.Board[newCoordinates.X, newCoordinates.Y]; //switch empty place to the atom
            _level.Board[newCoordinates.X, newCoordinates.Y] = atom; // new field will be atom
            _level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty = true; // but now will be rendered as empty
            _level.Board[newCoordinates.X, newCoordinates.Y].Asset = "Empty";// but now will be rendered as empty

            CalculateBoardTilePositions(boardPosition, _level.Board, _renderAtomScale);

            _moves += 1;
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
            if (_level.CanGoUp(i, j))
            {
                board[i, j].Movements |= MoveDirection.Up;
                board[i - 1, j].Asset = "Up";
            }
            if (_level.CanGoDown(i, j))
            {
                board[i, j].Movements |= MoveDirection.Down;
                board[i + 1, j].Asset = "Down";
            }
            if (_level.CanGoLeft(i, j))
            {
                board[i, j].Movements |= MoveDirection.Left;
                board[i, j - 1].Asset = "Left";
            }
            if (_level.CanGoRight(i, j))
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

            for (int i = -1; i <= 1; i++) // rows
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
        private int _levelNameWidth = 360;
        private Viewport _levelNameViewPort;
        private Viewport _defaultViewport;

        private bool CheckFinish()
        {
            for (int i = 0; i < _level.Board.RowsCount; i++)
            {
                for (int j = 0; j < _level.Board.ColumnsCount; j++)
                {
                    // We expect match
                    bool isMatch = true;

                    for (int x = 0; x < _level.Molecule.RowsCount; x++)
                    {
                        for (int y = 0; y < _level.Molecule.ColumnsCount; y++)
                        {
                            // If we have empty tile in definition it is like a wildcard that matches everything
                            if (_level.Molecule[x, y] == null || _level.Molecule[x, y].IsEmpty)
                                continue;

                            // TODO overflow
                            if (i + x >= _level.Board.RowsCount || j + y >= _level.Board.ColumnsCount || _level.Board[i + x, j + y] == null || _level.Board[i + x, j + y].Asset != _level.Molecule[x, y].Asset)
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
                    while (newCoordinates.Y < _level.Board.ColumnsCount)
                    {
                        if (_level.Board[newCoordinates.X, newCoordinates.Y] == null || _level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.Y++;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Left:
                    newCoordinates.Y--; // We started on the atom -> move one left
                    while (newCoordinates.Y >= 0)
                    {
                        if (_level.Board[newCoordinates.X, newCoordinates.Y] == null || _level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.Y--;
                    }

                    if (newCoordinates.Y != coordinates.Y) newCoordinates.Y++; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Down:
                    newCoordinates.X++; // We started on the atom -> move one bottom
                    while (newCoordinates.X < _level.Board.RowsCount)
                    {
                        if (_level.Board[newCoordinates.X, newCoordinates.Y] == null || _level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
                            break;

                        newCoordinates.X++;
                    }

                    if (newCoordinates.X != coordinates.X) newCoordinates.X--; // pretekli jsme o jedno za (na zed) tak se vratime zpet, ale jen pokud je pohyb
                    break;
                case MoveDirection.Up:
                    newCoordinates.X--; // We started on the atom -> move one up
                    while (newCoordinates.X >= 0)
                    {
                        if (_level.Board[newCoordinates.X, newCoordinates.Y] == null || _level.Board[newCoordinates.X, newCoordinates.Y].IsEmpty == false)
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
            for (int x = 0; x < _level.Board.RowsCount; x++)
            {
                for (int y = 0; y < _level.Board.ColumnsCount; y++)
                {
                    if (_level.Board[x, y] == null)
                        continue;

                    switch (_level.Board[x, y].Asset)
                    {
                        case "Down":
                        case "Up":
                        case "Right":
                        case "Left":
                            _level.Board[x, y].Asset = "Empty";
                            break;
                    }

                    _level.Board[x, y].IsSelected = false;
                    _level.Board[x, y].Movements = MoveDirection.None;
                    _level.Board[x, y].Opacity = 1;
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
                    tileTexture = _level.Assets[asset];
                    break;
            }

            return tileTexture;
        }

        private void CalculateBoardTilePositions(Vector2 startPosition, TilesCollection<BoardTileViewModel> board, float scale = 1)
        {
            Vector2 currentPosition = new Vector2(startPosition.X, startPosition.Y);

            for (int i = 0; i < board.RowsCount; i++)
            {
                for (int j = 0; j < board.ColumnsCount; j++)
                {
                    if (board[i, j] != null)
                    {
                        board[i, j].RenderPosition = currentPosition;
                        board[i, j].RenderScale = scale;
                        board[i, j].RenderRectangle = new Rectangle((int)currentPosition.X, (int)currentPosition.Y, (int)(TileWidth * scale), (int)(TileHeight * scale));
                    }

                    currentPosition.X += TileWidth * scale;
                }

                currentPosition.X = startPosition.X;
                currentPosition.Y += TileHeight * scale;
            }
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
                    {
                        //_spriteBatch.Draw(emptyTexture, board[i, j].RenderRectangle, Color.White);
                        _spriteBatch.Draw(emptyTexture, board[i, j].RenderPosition, null, Color.White, 0, Vector2.Zero, board[i, j].RenderScale, SpriteEffects.None, 0);
                    }

                    if (tile != null)
                    {
                        //_spriteBatch.Draw(tile, board[i, j].RenderRectangle, null, Color.White * board[i, j].Opacity, RotationAngle, origin, SpriteEffects.None, 0f);
                        _spriteBatch.Draw(tile, board[i, j].RenderPosition, null, Color.White * board[i, j].Opacity, RotationAngle, origin, board[i, j].RenderScale, SpriteEffects.None, 0);

                        if (board[i, j].IsHovered && (board[i, j].IsFixed == false || board[i, j].IsEmpty == true))
                        {
                            //_spriteBatch.Draw(activeTexture, board[i, j].RenderRectangle, Color.White);
                            _spriteBatch.Draw(activeTexture, board[i, j].RenderPosition, null, Color.White, 0, Vector2.Zero, board[i, j].RenderScale, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
    }
}
