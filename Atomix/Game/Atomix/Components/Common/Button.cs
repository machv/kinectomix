using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atomix
{
    public class Button : DrawableGameComponent
    {
        private Texture2D _empty;
        private Vector2 _position;
        protected IInputProvider _inputProvider;
        protected Rectangle _boundingRectangle;

        /// <summary>
        /// Gets or sets rendering position of this button.
        /// </summary>
        /// <returns>Top position to render.</returns>
        public Vector2 Position
        {
            get { return _position; }
            set {
                _position = value;
                _boundingRectangle = new Rectangle((int)value.X, (int)value.Y, Width, Height);
            }
        }

        public IInputProvider InputProvider
        {
            get { return _inputProvider; }
            set { _inputProvider = value; }
        }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Content { get; set; }

        public int BorderThickness { get; set; }

        public Color BorderColor { get; set; }
        public Color Background { get; set; }

        public Color ActiveBackground { get; set; }

        public Color Foreground { get; set; }

        public SpriteFont Font { get; set; }

        public object Tag { get; set; }

        public event EventHandler<EventArgs> Selected;

        protected internal void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        SpriteBatch _spriteBatch;

        public Button(Game game)
            : base(game)
        {
            Background = Color.Gray;
            ActiveBackground = Color.Silver;
            Foreground = Color.White;
            BorderColor = Color.Black;
            BorderThickness = 2;
            Width = 160;
            Height = 70;
            Content = string.Empty;
        }

        public Button(Game game, string content)
            : this(game)
        {
            Content = content;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _empty = Game.Content.Load<Texture2D>("Empty");

            base.LoadContent();
        }

        private IInputState _previousInputState;
        private IInputState _currentInputState;

        private Color currentBackground;

        public override void Update(GameTime gameTime)
        {
            if (_inputProvider == null)
                throw new Exception("No input provider is set.");

            _previousInputState = _currentInputState;
            _currentInputState = _inputProvider.GetState();
            bool isOver = _currentInputState.X >= Position.X &&
                    _currentInputState.Y >= Position.Y &&
                    _currentInputState.X <= Position.X + Width &&
                    _currentInputState.Y <= Position.Y + Height;

            currentBackground = isOver ? ActiveBackground : Background;

            if (_previousInputState != null && _previousInputState.IsSelected != _currentInputState.IsSelected && isOver)
            {
                OnSelected();

            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (Font == null)
                throw new Exception("Font is not set.");

            Rectangle innerDimensions = new Rectangle(
                (int)Position.X + BorderThickness,
                (int)Position.Y + BorderThickness,
                Width - 2 * BorderThickness,
                Height - 2 * BorderThickness);

            _spriteBatch.Draw(_empty, _boundingRectangle, BorderColor);

            _spriteBatch.Draw(_empty, innerDimensions, currentBackground);


            Vector2 textSize = Font.MeasureString(Content);
            Vector2 textPosition = new Vector2(_boundingRectangle.Center.X, _boundingRectangle.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            _spriteBatch.DrawString(Font, Content, textPosition, Foreground);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
