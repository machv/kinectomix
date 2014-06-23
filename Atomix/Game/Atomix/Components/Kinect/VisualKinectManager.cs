using Kinectomix.Components.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atomix
{
    /// <summary>
    /// Handles Kinect sensor initialization.
    /// </summary>
    public class VisualKinectManager : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _iconTexture;
        private Texture2D _backgroundTexture;
        private bool _showConnectKinectPrompt;
        private bool _drawConnectKinectAlert;
        private Vector2 _iconPosition;
        private Vector2 _textPosition;
        private Vector2 _backgroundPosition;
        private float _iconScale;
        private string _description;
        private bool _drawIcon;
        private short _ticks;
        private KinectManager _manager;

        public bool ShowConnectKinectPrompt
        {
            get { return _showConnectKinectPrompt; }
            set { _showConnectKinectPrompt = value; }
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

        VisualKinectManager(Game game)
            : base(game)
        {
            _iconScale = 0.4f;
            _showConnectKinectPrompt = true;
        }

        public VisualKinectManager(Game game, KinectManager kinectManager)
           : this(game)
        {
            _manager = kinectManager;
        }

        /// <summary>
        /// Creates new instance of <see cref="VisualKinectManager"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public VisualKinectManager(Game game, bool startColorStream, bool startDepthStream)
            : this(game)
        {
            _manager = new KinectManager(startColorStream, startDepthStream);
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
            _font = Game.Content.Load<SpriteFont>("Fonts/KinectStatus");
            _iconTexture = Game.Content.Load<Texture2D>("Images/KinectIcon");
            _backgroundTexture = Game.Content.Load<Texture2D>("Images/KinectBackground");

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
                // Background
                _backgroundPosition = new Vector2();
                _backgroundPosition.X = Game.GraphicsDevice.Viewport.Width / 2 - _backgroundTexture.Width / 2;
                _backgroundPosition.Y = 0;

                // Icon
                _iconPosition = new Vector2();
                _iconPosition.X = Game.GraphicsDevice.Viewport.Width / 2 - _iconTexture.Width * _iconScale / 2;
                _iconPosition.Y = 20;

                // Text
                _description = _manager.LastStatus == KinectStatus.Undefined ? "please connect sensor" : _manager.GetStatusDescription(_manager.LastStatus);
                Vector2 textSize = _font.MeasureString(_description);
                _textPosition = new Vector2();
                _textPosition.X = Game.GraphicsDevice.Viewport.Width / 2 - textSize.X / 2;
                _textPosition.Y = _iconPosition.Y + _iconTexture.Height * _iconScale + 10;

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
            if (Sensor == null || _manager.LastStatus != KinectStatus.Connected)
            {
                if (_drawConnectKinectAlert)
                {
                    _spriteBatch.Begin();

                    _spriteBatch.Draw(_backgroundTexture, _backgroundPosition, Color.White);
                    _spriteBatch.DrawString(_font, _description, _textPosition, Color.Black);
                    if (_drawIcon == true)
                        _spriteBatch.Draw(_iconTexture, _iconPosition, null, Color.White, 0, Vector2.Zero, _iconScale, SpriteEffects.None, 0);

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
