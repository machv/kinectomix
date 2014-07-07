using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Kinectomix.Logic;
using Atomix.Components;
using Kinectomix.Xna.Input;
using Atomix.Components.Kinect;
using System;
using AtomixData;
using Mach.Xna.Input;
using Mach.Xna.ScreenManagement;
using Mach.Xna.Kinect.Components;
using Mach.Xna.Kinect;

namespace Atomix
{
    /// <summary>
    /// This is the main class for the Atomix game.
    /// </summary>
    public class AtomixGame : Game
    {
        public const string HighscoreFile = "atomix.highscore";

        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        ScreenManager _gameScreenManager;
        VisualKinectManager _kinectChooser;
        VideoStreamComponent _videoStream;
        SkeletonRenderer _skeletonRenderer;
        Gestures _gestures;
        IInputProvider _input;
        static GameState _state;
        Vector2 _kinectDebugOffset;
        float _scale = 1;
        public static GameState State { get { return _state; } }
        KinectCircleCursor _cursor;

        public VisualKinectManager KinectChooser
        {
            get { return _kinectChooser; }
        }

        public KinectCircleCursor Cursor { get { return _cursor; } }

        private int _fullScreenWidth = 1280;
        private int _fullScreenHeight = 720;
        private int _windowWidth = 1280;
        private int _windowHeight = 720;
        private bool _isFullScreen = false;
        private KeyboardState _previousKeyboardState;

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

        private void UpdateScreenDimensions()
        {
            if (_isFullScreen)
            {
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if ((mode.Width == _fullScreenWidth) && (mode.Height == _fullScreenHeight))
                    {
                        graphics.PreferredBackBufferWidth = _fullScreenWidth;
                        graphics.PreferredBackBufferHeight = _fullScreenHeight;
                        graphics.IsFullScreen = true;
                        graphics.ApplyChanges();

                        break;
                    }
                }
            }
            else
            {
                if (_windowWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    graphics.PreferredBackBufferWidth = _windowWidth;
                    graphics.PreferredBackBufferHeight = _windowHeight;
                    graphics.IsFullScreen = false;
                    graphics.ApplyChanges();
                }
            }
        }

        public AtomixGame()
        {
            graphics = new GraphicsDeviceManager(this);

            _fullScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _fullScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            UpdateScreenDimensions();

            Content.RootDirectory = "Content";

            _state = new GameState();

            Exiting += Game_Exiting;
        }

         /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            _kinectChooser = new VisualKinectManager(this, true, true);
            _gestures = new Gestures(this, _kinectChooser.Skeletons, "Content/Gestures/");
            _skeletonRenderer = new SkeletonRenderer(this, _kinectChooser, _kinectDebugOffset, _scale);
            _cursor = new KinectCircleCursor(this, _kinectChooser) { HideMouseCursorWhenHandTracked = true };
            _videoStream = new VideoStreamComponent(this, _kinectChooser) { StreamType = VideoStream.Depth };
            var background = new Background(this, "Background");
            var frameRate = new FrameRateInfo(this);
            var clippedEdgeVisualiser = new ClippedEdgesVisualiser(this, _kinectChooser.Skeletons);
            //_cursor.HandTracker = new ConvexityClosedHandTracker(_kinectChooser);
            //_cursor.VideoStreamData = _videoStream;

            // Input
            var mouseInput = new MouseInputProvider();
            var kinectInput = new KinectInputProvider(_cursor);
            var multipleInput = new MutipleInputProvider();
            multipleInput.AddProvider(kinectInput);
            multipleInput.AddProvider(mouseInput);
            _input = multipleInput;

            _gameScreenManager = new ScreenManager(this, _input);

            UpdateScale(_scale);

            Components.Add(background);
            Components.Add(frameRate);
            Components.Add(_gameScreenManager);
            Components.Add(_kinectChooser);
            //Components.Add(_gestures);
            Components.Add(_videoStream);
            Components.Add(_skeletonRenderer);
            Components.Add(_cursor);
            Components.Add(clippedEdgeVisualiser);

            base.Initialize();
        }

        protected void UpdateScale(float scale)
        {
            _scale = scale;
            _kinectDebugOffset = new Vector2(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 * _scale, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 * _scale);

            _skeletonRenderer.Scale = _scale;
            _skeletonRenderer.RenderOffset = _kinectDebugOffset;

            _cursor.Scale = _scale;
            _cursor.RenderOffset = _kinectDebugOffset;

            _videoStream.RenderingScale = _scale;
            _videoStream.RenderingOffset = _kinectDebugOffset;
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
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Q) == true && _previousKeyboardState.IsKeyDown(Keys.Q) == false)
                UpdateScale(_scale - 0.1f);

            if (state.IsKeyDown(Keys.A) == true && _previousKeyboardState.IsKeyDown(Keys.A) == false)
                UpdateScale(_scale + 0.1f);

            if (state.IsKeyDown(Keys.D))
            {
                Components.Remove(_videoStream);
                Components.Remove(_skeletonRenderer);
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

        private Texture2D CreateColorTexture(Color color, int width = 1, int height = 1)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);

            // Create a color array for the pixels
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(color.ToVector3());
            }

            // Set the color data for the texture
            texture.SetData(colors);

            return texture;
        }

        private void Game_Exiting(object sender, EventArgs e)
        {
            if (_state != null && _state.Highscore != null)
                _state.Highscore.Save();
        }
    }
}
