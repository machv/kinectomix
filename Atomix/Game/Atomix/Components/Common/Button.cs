using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atomix
{
    /// <summary>
    /// Basic button implementation for use in XNA game.
    /// </summary>
    public class Button : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _empty;
        private Vector2 _position;
        private int _width;
        private int _height;
        private string _content;

        private IInputState _previousInputState;
        private IInputState _currentInputState;
        private Color _currentBackground;

        protected IInputProvider _inputProvider;
        protected Rectangle _boundingRectangle;

        /// <summary>
        /// Gets or sets rendering position of this button.
        /// </summary>
        /// <returns>Top position to render.</returns>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _boundingRectangle = new Rectangle((int)value.X, (int)value.Y, _width, _height);
            }
        }
        /// <summary>
        /// Gets or sets input provider for accepting interactions from user.
        /// </summary>
        /// <returns>Current registered input provider for this button.</returns>
        public IInputProvider InputProvider
        {
            get { return _inputProvider; }
            set { _inputProvider = value; }
        }
        /// <summary>
        /// Gets or sets width of this button.
        /// </summary>
        /// <returns>Current width of the button.</returns>
        public int Width
        {
            get { return _width; }
            set
            {
                if (value > 0)
                    _width = value;
            }
        }
        /// <summary>
        /// Gets or sets height of this button.
        /// </summary>
        /// <returns>Current height of the button.</returns>
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
        /// Gets or sets caption displayed on this button.
        /// </summary>
        /// <returns>Current caption.</returns>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public int BorderThickness { get; set; }

        public Color BorderColor { get; set; }
        public Color Background { get; set; }

        public Color ActiveBackground { get; set; }

        public Color Foreground { get; set; }

        public SpriteFont Font { get; set; }

        public object Tag { get; set; }

        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Fires <see cref="Selected"/> event.
        /// </summary>
        protected void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }


        /// <summary>
        /// Constructs new instance of <see cref="Button"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public Button(Game game)
            : base(game)
        {
            Background = Color.Gray;
            ActiveBackground = Color.Silver;
            Foreground = Color.White;
            BorderColor = Color.Black;
            BorderThickness = 2;
            _width = 160;
            _height = 70;
            _content = string.Empty;
        }

        /// <summary>
        /// Constructs new instance of <see cref="Button"/> with text caption.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="content">Caption text for the button.</param>
        public Button(Game game, string content)
            : this(game)
        {
            _content = content;
        }

        /// <summary>
        /// Loads required content for rendering.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _empty = Game.Content.Load<Texture2D>("Empty");

            base.LoadContent();
        }

        /// <summary>
        /// Checks for hover over button or any interaction from user via <see cref="IInputProvider"/> interface.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (_inputProvider == null)
                throw new Exception("No input provider is set.");

            _previousInputState = _currentInputState;
            _currentInputState = _inputProvider.GetState();

            bool isOver = _boundingRectangle.Contains(_currentInputState.X, _currentInputState.Y);

            _currentBackground = isOver ? ActiveBackground : Background;

            if (_previousInputState != null && _previousInputState.IsSelected != _currentInputState.IsSelected && isOver)
            {
                OnSelected();

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this button on the screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (Font == null)
                throw new Exception("Font is not set.");

            Rectangle innerDimensions = new Rectangle(
                (int)Position.X + BorderThickness,
                (int)Position.Y + BorderThickness,
                _width - 2 * BorderThickness,
                _height - 2 * BorderThickness);

            _spriteBatch.Draw(_empty, _boundingRectangle, BorderColor);

            _spriteBatch.Draw(_empty, innerDimensions, _currentBackground);


            Vector2 textSize = Font.MeasureString(_content);
            Vector2 textPosition = new Vector2(_boundingRectangle.Center.X, _boundingRectangle.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            _spriteBatch.DrawString(Font, _content, textPosition, Foreground);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
