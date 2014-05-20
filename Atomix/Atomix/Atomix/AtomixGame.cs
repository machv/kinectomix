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
using Kinectomix.Logic;
using System.Xml;
using Microsoft.Kinect;
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
        KinectChooser _KinectChooser;
        VideoStreamComponent _videoStream;
        SkeletonRenderer _skeletonRenderer;
        Skeletons _skeletons = new Skeletons();
        IInputProvider _input;
        static GameState _state;
        Vector2 _kinectDebugOffset;
        float _scale = 1;
        public static GameState State { get { return _state; } }
        KinectCursor _cursor;

        public KinectCursor Cursor { get { return _cursor; } }

        public AtomixGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.IsFullScreen = true;

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

            _KinectChooser = new KinectChooser(this);
            _skeletonRenderer = new SkeletonRenderer(this, _KinectChooser, _skeletons, _kinectDebugOffset, _scale);
            _cursor = new KinectCursor(this, _KinectChooser, _skeletons, _kinectDebugOffset, _scale);
            _videoStream = new VideoStreamComponent(this, _KinectChooser, graphics, _kinectDebugOffset, _scale) { Type = VideoType.Depth };
            var background = new Background(this);
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
            Components.Add(_gameScreenManager);
            Components.Add(_KinectChooser);
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
            if (state.IsKeyDown(Keys.Add))
                UpdateScale(_scale - 0.005f);

            if (state.IsKeyDown(Keys.Subtract))
                UpdateScale(_scale + 0.005f);

            if (_KinectChooser.Sensor != null && _KinectChooser.Sensor.IsRunning && _KinectChooser.SkeletonData != null)
                _skeletons.Items = _KinectChooser.SkeletonData;

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
