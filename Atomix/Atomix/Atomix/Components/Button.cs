using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class Button
    {
        Texture2D _empty;
        Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public string Content { get; set; }

        public int BorderThickness { get; set; }

        public Color BorderColor { get; set; }
        public Color Background { get; set; }

        public Color Foreground { get; set; }

        public SpriteFont Font { get; set; }

        public event EventHandler<EventArgs> Selected;

        protected internal void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        SpriteBatch _spriteBatch;

        public Button(SpriteBatch spriteBatch)
        {
            Background = Color.Gray;
            Foreground = Color.White;
            BorderColor = Color.Black;
            BorderThickness = 2;
            Width = 160;
            Height = 70;
            _spriteBatch = spriteBatch;
        }

        public Button(SpriteBatch spriteBatch, string content)
            : this(spriteBatch)
        {
            Content = content;
        }

        public void LoadContent(ContentManager content)
        {
            _empty = content.Load<Texture2D>("Empty");
        }

        IInputState lastState;
        IInputState currState;

        public void Update(GameTime gameTime, IInputProvider input)
        {
            lastState = currState;
            currState = input.GetState();

            if (lastState != null && !lastState.IsSelected && currState.IsSelected)
            {
                if (currState.X >= Position.X &&
                    currState.Y >= Position.Y &&
                    currState.X <= Position.X + Width &&
                    currState.Y <= Position.Y + Height)
                {
                    OnSelected();
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (Font == null)
                throw new Exception("Font is not set.");

            Rectangle buttonDimensions = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Width,
                Height);

            _spriteBatch.Draw(_empty, buttonDimensions, Background);

            _spriteBatch.Draw(
                _empty,
                new Rectangle(buttonDimensions.Left, buttonDimensions.Top, buttonDimensions.Width, BorderThickness),
                BorderColor);
            _spriteBatch.Draw(
                _empty,
                new Rectangle(buttonDimensions.Left, buttonDimensions.Top, BorderThickness, buttonDimensions.Height),
                BorderColor);
            _spriteBatch.Draw(
                _empty,
                new Rectangle(buttonDimensions.Right - BorderThickness, buttonDimensions.Top, BorderThickness, buttonDimensions.Height),
                BorderColor);
            _spriteBatch.Draw(
                _empty,
                new Rectangle(buttonDimensions.Left, buttonDimensions.Bottom - BorderThickness, buttonDimensions.Width, BorderThickness),
                BorderColor);


            Vector2 textSize = Font.MeasureString(Content);
            Vector2 textPosition = new Vector2(buttonDimensions.Center.X, buttonDimensions.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            _spriteBatch.DrawString(Font, Content, textPosition, Foreground);

        }

        protected void UnloadContent(Game game)
        {
            //game.Content.Unload();

        }
    }
}
