using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atomix.Components.Common
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MessageBox : DrawableGameComponent
    {
        private SpriteFont _font;
        private bool _isVisible;
        private bool _isFullscreen;
        private int _height;
        private int _borderWidth;
        private SpriteBatch _spriteBatch;
        private Texture2D _empty;
        private MessageBoxButtons _buttons;
        private string _text;
        private Rectangle _backgroundBox;
        private Rectangle _outerBox;
        private Rectangle _innerBox;
        private Vector2 _fontSize;
        private Vector2 _textPosition;

        /// <summary>
        /// Gets or sets font used for rendering texts in message box.
        /// </summary>
        /// <returns>Font used in message box.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }
        /// <summary>
        /// Gets if this message box is currently visible.
        /// </summary>
        /// <returns>True if message box is visible.</returns>
        public bool IsVisible
        {
            get { return _isVisible; }
        }
        /// <summary>
        /// Gets or sets if this message box should be in fullscreen background.
        /// </summary>
        /// <returns>True if fullscreen backgrnound will be used.</returns>
        public bool IsFullscreen
        {
            get { return _isFullscreen; }
            set { _isFullscreen = value; }
        }
        /// <summary>
        /// Gets or sets message box height.
        /// </summary>
        /// <returns>Height of message box.</returns>
        public int Height
        {
            get { return _height; }
            set
            {
                if (value > 0)
                    _height = value;
            }
        }
        /// <summary>
        /// Gets or sets width of the message box's border.
        /// </summary>
        /// <returns>Width of the border.</returns>
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                if (value > 0)
                    _borderWidth = value;
            }
        }

        /// Occurs when a result inside <see cref="MessageBox"/> is selected.
        public event EventHandler<MessageBoxEventArgs> Changed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> component.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public MessageBox(Game game)
            : base(game)
        {
            _borderWidth = 4;
            _height = 250;
        }

        /// <summary>
        /// Fires <see cref="Changed"/> event.
        /// </summary>
        private void OnChanged(MessageBoxResult result)
        {
            if (Changed != null)
            {
                Changed(this, new MessageBoxEventArgs(result));
            }
        }

        /// <summary>
        /// Initializes a message box component.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Loads content required for a message box.
        /// </summary>
        protected override void LoadContent()
        {
            _empty = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _empty.SetData(new Color[] { Color.White });

            base.LoadContent();
        }

        /// <summary>
        /// Updates a message box before rendering.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (_font == null)
                throw new Exception("No font specified for rendering.");

            if (_isVisible)
            {
                _backgroundBox = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
                _outerBox = new Rectangle(0, (GraphicsDevice.Viewport.Bounds.Height - _height) / 2, GraphicsDevice.Viewport.Bounds.Width, _height);
                _innerBox = new Rectangle(0, _borderWidth + (GraphicsDevice.Viewport.Bounds.Height - _height) / 2, GraphicsDevice.Viewport.Bounds.Width, _height - 2 * _borderWidth);
                _fontSize = _font.MeasureString(_text);
                _textPosition = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - _fontSize.X / 2, GraphicsDevice.Viewport.Bounds.Height / 2 - _height / 2 + _fontSize.Y / 2);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws a message box on the screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_isVisible)
            {
                _spriteBatch.Begin();

                if (_isFullscreen)
                    _spriteBatch.Draw(_empty, _backgroundBox, Color.LightGray);

                _spriteBatch.Draw(_empty, _outerBox, Color.Silver);
                _spriteBatch.Draw(_empty, _innerBox, Color.Gray);
                _spriteBatch.DrawString(_font, _text, _textPosition, Color.White);

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Displays a message box with specified text and buttons.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the <see cref="MessageBoxButtons"/> values that specifies which buttons to display in the message box.</param>
        public void Show(string text, MessageBoxButtons buttons)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            _isVisible = true;
            _text = text;
            _buttons = buttons;
        }

        /// <summary>
        /// Displays a message box with specified text.
        /// </summary>
        /// <param name="text"></param>
        public void Show(string text)
        {
            Show(text, MessageBoxButtons.OK);
        }
    }
}
