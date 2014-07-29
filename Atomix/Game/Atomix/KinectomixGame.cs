using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Mach.Xna.Input;
using Mach.Xna.ScreenManagement;
using Mach.Xna.Kinect.Components;
using Mach.Kinectomix.Logic;
using Mach.Kinectomix.Screens;
using Mach.Kinectomix.Components;

namespace Mach.Kinectomix
{
    /// <summary>
    /// This is the main class for the Kinectomix game.
    /// </summary>
    public class KinectomixGame : Game
    {
        public const string HighscoreFile = "kinectomix.highscore";
        public static Color BrickColor = Color.Gold; 

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ScreenManager _gameScreenManager;
        private VisualKinectManager _visualKinectManager;
        private VideoStreamComponent _videoStream;
        private SkeletonRenderer _skeletonRenderer;
        private Gestures _gestures;
        private IInputProvider _input;
        private static GameState _state;
        private Vector2 _kinectDebugOffset;
        private float _scale = 0.5f;
        private KinectCircleCursor _cursor;
        private int _fullScreenWidth = 1280;
        private int _fullScreenHeight = 720;
        private int _windowWidth = 1280;
        private int _windowHeight = 720;
        private bool _isFullScreen = false;
        private KeyboardState _previousKeyboardState;

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public static GameState State
        {
            get { return _state; }
        }
        /// <summary>
        /// Gets the kinect chooser.
        /// </summary>
        /// <value>
        /// The kinect chooser.
        /// </value>
        public VisualKinectManager VisualKinectManager
        {
            get { return _visualKinectManager; }
        }
        /// <summary>
        /// Gets the gestures component.
        /// </summary>
        /// <value>
        /// The gestures component.
        /// </value>
        public Gestures Gestures
        {
            get { return _gestures; }
        }
        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <value>
        /// The cursor.
        /// </value>
        public KinectCircleCursor Cursor
        {
            get { return _cursor; }
        }
        /// <summary>
        /// Gets or sets if game is used full screen of windowed mode.
        /// </summary>
        /// <returns>True if full screen mode is used.</returns>
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                if (_isFullScreen != value)
                {
                    _isFullScreen = value;

                    UpdateScreenDimensions();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectomixGame"/> class.
        /// </summary>
        public KinectomixGame()
        {
            _graphics = new GraphicsDeviceManager(this);

//            _fullScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
//            _fullScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            UpdateScreenDimensions();

            Content.RootDirectory = "Content";

            _state = new GameState();

            Resources.StartScreenResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
            Resources.LevelScreenResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
            Resources.LevelsScreenResources.Culture = System.Globalization.CultureInfo.CurrentCulture;

            Exiting += Game_Exiting;
        }

        /// <summary>
        /// Initializes shared components for all screens.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            _visualKinectManager = new VisualKinectManager(this, true, true);
            _gestures = new Gestures(this, _visualKinectManager.Manager, "Content/Gestures/");
            _skeletonRenderer = new SkeletonRenderer(this, _visualKinectManager, _kinectDebugOffset, _scale);

            _cursor = new KinectCircleCursor(this, _visualKinectManager.Manager)
            {
                HideSystemCursorWhenHandTracked = true,
                CursorPositionsBufferLength = 5,
            };

            _videoStream = new VideoStreamComponent(this, _visualKinectManager)
            {
                StreamType = VideoStream.Color,
                RenderingScale = _scale,
                RenderingOffset = _kinectDebugOffset,
            };

            var videoWhenNoSkeleton = new VideoWhenNoSkeleton(this, _visualKinectManager.Manager, _videoStream, _skeletonRenderer);
            var background = new Background(this, "Backgrounds/Start");
            var clippedEdgeVisualiser = new ClippedEdgesVisualiser(this, _visualKinectManager.Manager);
            //_cursor.HandStateTracker = new ConvexityClosedHandTracker(_visualKinectManager); // { VideoStreamData = _videoStream };

            // Input
            var mouseInput = new MouseInputProvider();
            var kinectInput = new KinectInputProvider(_cursor);
            var multipleInput = new MutipleInputProvider();
            multipleInput.AddProvider(kinectInput);
            multipleInput.AddProvider(mouseInput);
            _input = multipleInput;

            _gameScreenManager = new ScreenManager(this, _input);

            UpdateScale(210f / 480f);

            Components.Add(background);
            Components.Add(_gameScreenManager);
            Components.Add(_visualKinectManager);
            Components.Add(_gestures);
            Components.Add(videoWhenNoSkeleton);
            Components.Add(_cursor);
            Components.Add(clippedEdgeVisualiser);

            base.Initialize();
        }

        private void UpdateScale(float scale)
        {
            _scale = scale;
            _kinectDebugOffset = new Vector2(80, 480);

            _skeletonRenderer.Scale = _scale;
            _skeletonRenderer.RenderOffset = _kinectDebugOffset;

            _cursor.Scale = _scale;
            _cursor.RenderOffset = _kinectDebugOffset;

            _videoStream.RenderingScale = _scale;
            _videoStream.RenderingOffset = _kinectDebugOffset;
            _videoStream.DestinationRectangle = new Rectangle(80, 480, 270, 210);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameScreen screen = null;
            try
            {
                GameDefinition definition = GameDefinitionFactory.Load();
                _state.Levels = definition.Levels;
                _state.DefinitionHash = definition.Hash;

                screen = new StartScreen(_spriteBatch);
            }
            catch
            {
                screen = new ErrorScreen(_spriteBatch, string.Format("Unable to load game levels."));
            }

            Highscore score = Highscore.Load(HighscoreFile);
            if (score.DefinitionHash != null && score.DefinitionHash != _state.DefinitionHash)
            {
                score = new Highscore(HighscoreFile);
            }
            score.DefinitionHash = _state.DefinitionHash;
            _state.Highscore = score;

            _visualKinectManager.Font = Content.Load<SpriteFont>("Fonts/Small");
            _visualKinectManager.Foreground = new Color(255, 43, 0);
            _visualKinectManager.Background = Content.Load<Texture2D>("KinectPromptBackground");
            _visualKinectManager.RenderPosition = new Vector2(290, 0);
            _visualKinectManager.ShowPromptKinectIcon = false;
            _visualKinectManager.PromptTextPositionOffset = new Vector2(150, 18);
            _visualKinectManager.ShowConnectedKinectIcon = false;

            _gameScreenManager.Add(screen);
            _gameScreenManager.Activate(screen);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Checks for global keyboard shortcuts.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            // Toggle full screen mode
            if (state.IsKeyDown(Keys.F11) == true && _previousKeyboardState.IsKeyDown(Keys.F11) == false)
                    IsFullScreen = !IsFullScreen;

            if (state.IsKeyDown(Keys.W) == true && _previousKeyboardState.IsKeyDown(Keys.W) == false)
                UpdateScale(_scale - 0.1f);

            if (state.IsKeyDown(Keys.S) == true && _previousKeyboardState.IsKeyDown(Keys.S) == false)
                UpdateScale(_scale + 0.1f);

            if (state.IsKeyDown(Keys.Q) == true && _previousKeyboardState.IsKeyDown(Keys.Q) == false)
                UpdateElevationAngle(+3);

            if (state.IsKeyDown(Keys.A) == true && _previousKeyboardState.IsKeyDown(Keys.A) == false)
                UpdateElevationAngle(-3);

            if (state.IsKeyDown(Keys.D) == true && _previousKeyboardState.IsKeyDown(Keys.D) == false)
            {
                if (Components.Contains(_videoStream))
                {
                    Components.Remove(_videoStream);
                    Components.Remove(_skeletonRenderer);
                }
                else
                {
                    Components.Add(_videoStream);
                    Components.Add(_skeletonRenderer);
                }
            }

            _previousKeyboardState = state;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }

        private void UpdateScreenDimensions()
        {
            if (_isFullScreen)
            {
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if ((mode.Width == _fullScreenWidth) && (mode.Height == _fullScreenHeight))
                    {
                        _graphics.PreferredBackBufferWidth = _fullScreenWidth;
                        _graphics.PreferredBackBufferHeight = _fullScreenHeight;
                        _graphics.IsFullScreen = true;
                        _graphics.ApplyChanges();

                        break;
                    }
                }
            }
            else
            {
                if (_windowWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    _graphics.PreferredBackBufferWidth = _windowWidth;
                    _graphics.PreferredBackBufferHeight = _windowHeight;
                    _graphics.IsFullScreen = false;
                    _graphics.ApplyChanges();
                }
            }
        }

        private void UpdateElevationAngle(int relativeAngle)
        {
            if (_visualKinectManager.Manager.Sensor != null)
            {
                int newAngle = _visualKinectManager.Sensor.ElevationAngle + relativeAngle;

                try
                {
                    if (newAngle >= _visualKinectManager.Sensor.MinElevationAngle && newAngle <= _visualKinectManager.Manager.Sensor.MaxElevationAngle)
                    {
                        _visualKinectManager.Sensor.ElevationAngle = newAngle;
                    }
                }
                catch
                { }
            }
        }

        private void Game_Exiting(object sender, EventArgs e)
        {
            if (_state != null && _state.Highscore != null)
                _state.Highscore.Save();
        }
    }
}
