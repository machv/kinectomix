using Mach.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Handles Kinect sensor initialization.
    /// </summary>
    public class VisualKinectManager : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _iconTexture;
        private Vector2 _iconPosition;
        private float _iconScale;
        private Vector2 _connectedIconPosition;
        private float _connectedIconScale;
        private Color _connectedIconColor;
        private Texture2D _connectedKinectTexture;
        private Texture2D _backgroundTexture;
        private bool _showConnectKinectPrompt;
        private bool _showConnectedKinectIcon;
        private bool _drawConnectKinectAlert;
        private Vector2 _textPosition;
        private string _description;
        private bool _drawIcon;
        private short _ticks;
        private KinectManager _manager;
        private ContentManager _content;
        private Color _foreground;
        private Vector2 _renderPosition;

        /// <summary>
        /// Gets or sets a whether connect kinect prompt will be rendered.
        /// </summary>
        /// <value>
        /// <c>true</c> if connect prompt will be rendered; otherwise, <c>false</c>.
        /// </value>
        public bool ShowConnectKinectPrompt
        {
            get { return _showConnectKinectPrompt; }
            set { _showConnectKinectPrompt = value; }
        }
        /// <summary>
        /// Gets or sets the background texture.
        /// </summary>
        /// <value>
        /// The background texture.
        /// </value>
        public Texture2D Background
        {
            get { return _backgroundTexture; }
            set { _backgroundTexture = value; }
        }
        /// <summary>
        /// Gets or sets the foreground color of the text.
        /// </summary>
        /// <value>
        /// The foreground color of the text.
        /// </value>
        public Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }
        /// <summary>
        /// Gets or sets the position where rendering of this component will start.
        /// </summary>
        /// <value>
        /// The position where rendering of this component will start.
        /// </value>
        public Vector2 RenderPosition
        {
            get { return _renderPosition; }
            set { _renderPosition = value; }
        }
        /// <summary>
        /// Gets or sets whether will be shown icon if Kinect sensor is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if icon representing connected Kinect will be rendered; otherwise, <c>false</c>.
        /// </value>
        public bool ShowConnectedKinectIcon
        {
            get { return _showConnectedKinectIcon; }
            set { _showConnectedKinectIcon = value; }
        }
        /// <summary>
        /// Gets or sets the position of connected Kinect Icon.
        /// </summary>
        /// <value>
        /// The position of connected Kinect Icon.
        /// </value>
        public Vector2 ConnectedKinectIconPosition
        {
            get { return _connectedIconPosition; }
            set { _connectedIconPosition = value; }
        }
        /// <summary>
        /// Gets or sets the scale of connected Kinect Icon.
        /// </summary>
        /// <value>
        /// The scale of connected Kinect Icon.
        /// </value>
        public float ConnectedKinectIconScale
        {
            get { return _connectedIconScale; }
            set { _connectedIconScale = value; }
        }
        /// <summary>
        /// Gets or sets the color of the connected Kinect icon.
        /// </summary>
        /// <value>
        /// The color of the connected Kinect icon.
        /// </value>
        public Color ConnectedKinectIconColor
        {
            get { return _connectedIconColor; }
            set { _connectedIconColor = value; }
        }
        /// <summary>
        /// Gets Kinect manager.
        /// </summary>
        /// <returns>Kinect manager.</returns>
        public KinectManager Manager
        {
            get { return _manager; }
        }
        /// <summary>
        /// Gets skeletons returned from the Kinect Sensor.
        /// </summary>
        /// <returns>Skeletons tracked by the Kinect sensor.</returns>
        public Skeletons Skeletons
        {
            get { return _manager.Skeletons; }
        }
        /// <summary>
        /// Gets selected Kinect sensor.
        /// </summary>
        /// <returns>Selected Kinect sensor.</returns>
        public KinectSensor Sensor
        {
            get { return _manager.Sensor; }
        }
        /// <summary>
        /// Gets the last known status of the <see cref="KinectSensor"/>.
        /// </summary>
        ///<returns>Last known status of the <see cref="KinectSensor"/>.</returns>
        public KinectStatus LastStatus
        {
            get { return _manager.LastStatus; }
        }

        /// <summary>
        /// Gets or sets <see cref="SpriteFont"/> that is used for rendering status information.
        /// </summary>
        /// <returns>Font used for rendering status information.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        static VisualKinectManager()
        {
            Localization.VisualKinectManagerResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        VisualKinectManager(Game game)
            : base(game)
        {
            _iconScale = 0.4f;
            _connectedIconColor = Color.Green;
            _connectedIconScale = 0.2f;
            _foreground = Color.Black;
            _renderPosition = Vector2.Zero;

            _showConnectKinectPrompt = true;
            _showConnectedKinectIcon = true;
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Manager handling connected sensor.</param>
        public VisualKinectManager(Game game, KinectManager kinectManager)
           : this(game)
        {
            _manager = kinectManager;
            _content = new ResourceContentManager(game.Services, Sprites.ResourceManager);
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="startColorStream">If true, color stream will be started.</param>
        /// /// <param name="startDepthStream">If true, depth stream will be started.</param>
        public VisualKinectManager(Game game, bool startColorStream, bool startDepthStream)
            : this(game)
        {
            _manager = new KinectManager(startColorStream, startDepthStream);
            _content = new ResourceContentManager(game.Services, Sprites.ResourceManager);
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager" />.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="content">ContentManager containing required assets.</param>
        /// <param name="startColorStream">If set to <c>true</c> color stream will be started.</param>
        /// <param name="startDepthStream">If set to <c>true</c> depth stream will be started.</param>
        public VisualKinectManager(Game game, ContentManager content, bool startColorStream, bool startDepthStream)
            : this(game)
        {
            _manager = new KinectManager(startColorStream, startDepthStream);
            _content = content;
        }

        /// <summary>
        /// Initializes required objects for this component.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            _font = _content.Load<SpriteFont>("KinectStatusFont");
            _iconTexture = _content.Load<Texture2D>("KinectIcon");
            _backgroundTexture = _content.Load<Texture2D>("KinectBackground");
            _connectedKinectTexture = _content.Load<Texture2D>("ConnectedKinect");

            base.LoadContent();
        }

        /// <summary>
        /// Updates <see cref="VisualKinectManager"/> for rendering phase.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_manager != null)
                _manager.ProcessUpdate();


            if (_showConnectKinectPrompt == true || (_manager.LastStatus != KinectStatus.Undefined && _manager.LastStatus != KinectStatus.Disconnected))
            {
                int width = 200;
                if (_backgroundTexture != null)
                {
                    width = _backgroundTexture.Width;
                }

                // Icon
                _iconPosition = new Vector2();
                _iconPosition.X = _renderPosition.X + width / 2 - (_iconTexture.Width * _iconScale) / 2;
                _iconPosition.Y = _renderPosition.Y + 20;

                // Text
                _description = _manager.LastStatus == KinectStatus.Undefined ? Localization.VisualKinectManagerResources.PleaseConnectSensor : _manager.GetStatusDescription(_manager.LastStatus);
                Vector2 textSize = _font.MeasureString(_description);
                _textPosition = new Vector2();
                _textPosition.X = _renderPosition.X + width / 2 - textSize.X / 2;
                _textPosition.Y = _renderPosition.Y + _iconPosition.Y + _iconTexture.Height * _iconScale + 10;

                _drawConnectKinectAlert = true;
            }
            else
            {
                _drawConnectKinectAlert = false;
            }

            _ticks++;
            if (_ticks % 40 == 0)
            {
                _ticks = 0;
                _drawIcon = !_drawIcon;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the current state of the <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_manager.Sensor == null || _manager.LastStatus != KinectStatus.Connected)
            {
                if (_drawConnectKinectAlert)
                {
                    _spriteBatch.Begin();

                    if (_backgroundTexture != null)
                    {
                        _spriteBatch.Draw(_backgroundTexture, _renderPosition, Color.White);
                    }

                    _spriteBatch.DrawString(_font, _description, _textPosition, _foreground);

                    if (_drawIcon == true)
                    {
                        _spriteBatch.Draw(_iconTexture, _iconPosition, null, Color.White, 0, Vector2.Zero, _iconScale, SpriteEffects.None, 0);
                    }

                    _spriteBatch.End();
                }
            }

            if (_manager.Sensor != null && _manager.Sensor.Status == KinectStatus.Connected)
            {
                if (_showConnectedKinectIcon == true)
                {
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(_connectedKinectTexture, _connectedIconPosition, null, _connectedIconColor, 0, Vector2.Zero, _connectedIconScale, SpriteEffects.None, 0);
                    _spriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method ensures that the <see cref="KinectSensor"/> is stopped.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (_manager != null)
                _manager.Dispose();
        }
    }
}
