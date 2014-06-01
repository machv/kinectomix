using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Kinectomix.Logic;
using Atomix.Components;
using Atomix.Input;

namespace Atomix
{
    /// <summary>
    /// This is the main class for the Atomix game.
    /// </summary>
    public class AtomixGame : Game
    {
        public const string HighscoreFile = "atomix.highscore";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager _gameScreenManager;
        KinectChooser _kinectChooser;
        VideoStreamComponent _videoStream;
        SkeletonRenderer _skeletonRenderer;
        Gestures _gestures;
        IInputProvider _input;
        static GameState _state;
        Vector2 _kinectDebugOffset;
        float _scale = 1;
        public static GameState State { get { return _state; } }
        KinectCursor _cursor;

        public KinectCursor Cursor { get { return _cursor; } }

        private int _fullScreenWidth = 1280;
        private int _fullScreenHeight = 720;
        private int _windowWidth = 1280;
        private int _windowHeight = 720;
        private bool _inFullScreen = false;
        /// <summary>
        /// Toggle between fullscreen and windowed mode
        /// </summary>
        private void UpdateScreenDimensions()
        {
            // This sets the display resolution or window size to the desired size
            // If windowed, it also forces a 4:3 ratio for height and adds 110 for header/footer
            if (_inFullScreen)
            {
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check our requested FullScreenWidth and Height against each supported display mode and set if valid
                    if ((mode.Width == _fullScreenWidth) && (mode.Height == _fullScreenHeight))
                    {
                        graphics.PreferredBackBufferWidth = _fullScreenWidth;
                        graphics.PreferredBackBufferHeight = _fullScreenHeight;
                        graphics.IsFullScreen = true;
                        graphics.ApplyChanges();
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

            _kinectDebugOffset = new Vector2(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 / _scale, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 / _scale);

            _kinectChooser = new KinectChooser(this);
            _gestures = new Gestures(this, _kinectChooser.Skeletons, "Content/Gestures/");
            _skeletonRenderer = new SkeletonRenderer(this, _kinectChooser, _kinectChooser.Skeletons, _kinectDebugOffset, _scale);
            _cursor = new KinectCursor(this, _kinectChooser, _kinectChooser.Skeletons, _kinectDebugOffset, _scale);
            _videoStream = new VideoStreamComponent(this, _kinectChooser, graphics, _kinectDebugOffset, _scale) { Type = VideoType.Depth };
            var background = new Background(this);
            var frameRate = new FrameRateInfo(this);
            _cursor.VideoStreamData = _videoStream;

            // Input
            var mouseInput = new MouseInputProvider();
            var kinectInput = new KinectInputProvider(_cursor);
            var multipleInput = new MutipleInputProvider();
            multipleInput.AddProvider(kinectInput);
            multipleInput.AddProvider(mouseInput);
            _input = multipleInput;

            _gameScreenManager = new ScreenManager(this, _input);

            Components.Add(background);
            Components.Add(frameRate);
            Components.Add(_gameScreenManager);
            Components.Add(_kinectChooser);
            Components.Add(_gestures);
            Components.Add(_videoStream);
            Components.Add(_skeletonRenderer);
            Components.Add(_cursor);

            base.Initialize();
        }

        protected void UpdateScale(float scale)
        {
            _scale = scale;
            _kinectDebugOffset = new Vector2(GraphicsDevice.Viewport.Bounds.Width - 20 - 640 / _scale, GraphicsDevice.Viewport.Bounds.Height - 20 - 480 / _scale);

            _skeletonRenderer.Scale = _scale;
            _skeletonRenderer.RenderOffset = _kinectDebugOffset;

            _cursor.Scale = _scale;
            _cursor.RenderOffset = _kinectDebugOffset;

            _videoStream.Scale = _scale;
            _videoStream.RenderOffset = _kinectDebugOffset;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameScreen screen = new StartScreen(spriteBatch);
            _gameScreenManager.Add(screen);
            _gameScreenManager.Activate(screen);

            _state.Levels = Content.Load<LevelDefinition[]>("Levels");
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
            if (state.IsKeyDown(Keys.Q))
                UpdateScale(_scale - 0.005f);

            if (state.IsKeyDown(Keys.A))
                UpdateScale(_scale + 0.005f);

            // Fullscreen on/off toggle
            if (state.IsKeyDown(Keys.F11))
            {
                // If not down last update, key has just been pressed.
                if (state.IsKeyDown(Keys.F11))
                {
                    _inFullScreen = !_inFullScreen;
                    UpdateScreenDimensions();
                }
            }

            if (state.IsKeyDown(Keys.D))
            {
                Components.Remove(_videoStream);
                Components.Remove(_skeletonRenderer);
            }

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
    }
}
