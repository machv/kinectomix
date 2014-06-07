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
        private IInputProvider _inputProvider;

        /// <summary>
        /// Gets or sets rendering position of this button.
        /// </summary>
        /// <returns>Top position to render.</returns>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
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

        public Button(Game game, SpriteBatch spriteBatch)
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

            _spriteBatch = spriteBatch;
        }

        public Button(Game game, SpriteBatch spriteBatch, string content)
            : this(game, spriteBatch)
        {
            Content = content;
        }

        protected override void LoadContent()
        {
            _empty = Game.Content.Load<Texture2D>("Empty");

            base.LoadContent();
        }

        IInputState lastState;
        IInputState currState;

        private Color currentBackground;

        public override void Update(GameTime gameTime)
        {
            if (_inputProvider == null)
                throw new Exception("No input provider is set.");

            lastState = currState;
            currState = _inputProvider.GetState();
            bool isOver = currState.X >= Position.X &&
                    currState.Y >= Position.Y &&
                    currState.X <= Position.X + Width &&
                    currState.Y <= Position.Y + Height;

            currentBackground = isOver ? ActiveBackground : Background;

            if (lastState != null && lastState.IsSelected != currState.IsSelected && isOver)
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

            Rectangle buttonDimensions = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Width,
                Height);

            Rectangle innerDimensions = new Rectangle(
                (int)Position.X + BorderThickness,
                (int)Position.Y + BorderThickness,
                Width - 2 * BorderThickness,
                Height - 2 * BorderThickness);

            _spriteBatch.Draw(_empty, buttonDimensions, BorderColor);

            _spriteBatch.Draw(_empty, innerDimensions, currentBackground);


            Vector2 textSize = Font.MeasureString(Content);
            Vector2 textPosition = new Vector2(buttonDimensions.Center.X, buttonDimensions.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            _spriteBatch.DrawString(Font, Content, textPosition, Foreground);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
