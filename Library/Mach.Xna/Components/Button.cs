using Mach.Xna.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mach.Xna.Components
{
    /// <summary>
    /// Basic button implementation for use in XNA framework.
    /// </summary>
    public class Button : ButtonBase
    {
        private Texture2D _empty;
        private Color _currentBackground;
        private string _content;
        private int _borderThickness;
        private Color _borderColor;
        private Color _background;
        private Color _activeBackground;
        private Color _foreground;
        private Color _disabledBackground;
        private SpriteFont _font;


        /// <summary>
        /// Gets or sets caption displayed on this button.
        /// </summary>
        /// <returns>Current caption.</returns>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        /// <summary>
        /// Gets or sets the border thickness of a button.
        /// </summary>
        /// <returns>Current thickness of border.</returns>
        public int BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }
        }
        /// <summary>
        /// Gets or sets color of button's border.
        /// </summary>
        /// <returns>Current color of the border.</returns>
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }
        /// <summary>
        /// Gets or sets color of button's background when button is not hovered.
        /// </summary>
        /// <returns>Background color of button.</returns>
        public Color Background
        {
            get { return _background; }
            set { _background = value; }
        }
        /// <summary>
        /// Gets or sets color of the button's background when button is hovered.
        /// </summary>
        /// <returns>Background color when hovered.</returns>
        public Color ActiveBackground
        {
            get { return _activeBackground; }
            set { _activeBackground = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="Color"/> of the background when button is not in enabled state.
        /// </summary>
        /// <value>
        /// The <see cref="Color"/> of the background when button is not in enabled state.
        /// </value>
        public Color DisabledBackground
        {
            get { return _disabledBackground; }
            set { _disabledBackground = value; }
        }
        /// <summary>
        /// Gets or sets color of the text in the button.
        /// </summary>
        /// <returns>Color of the text.</returns>
        public Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }
        /// <summary>
        /// Gets or sets font used for rendering content of the button.
        /// </summary>
        /// <returns>Current font.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public Button(Game game)
            : base(game)
        {
            _background = Color.Gray;
            _activeBackground = Color.Silver;
            _disabledBackground = Color.LightGray;
            _foreground = Color.White;
            _borderColor = Color.Black;
            _borderThickness = 2;
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
        /// Initializes a button component.
        /// </summary>
        public override void Initialize()
        {
            if (_font == null)
            {
                // Loads default font from library resources.
                ContentManager content = new ResourceContentManager(Game.Services, Resources.ResourceManager);
                _font = content.Load<SpriteFont>("NormalFont");
            }

            base.Initialize();
        }

        /// <summary>
        /// Loads required content for rendering.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _empty = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _empty.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// Checks for hover over button or any interaction from user via <see cref="IInputProvider"/> interface.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (_isEnabled)
            {
                _currentBackground = _isFocused ? ActiveBackground : Background;
            }
            else
            {
                // When button is not active we use disabled background
                _currentBackground = _disabledBackground;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this button on the screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_isVisible)
            {
                _spriteBatch.Begin();

                if (_font == null)
                    throw new Exception("Font is not set.");

                Rectangle innerDimensions = new Rectangle(
                    (int)Position.X + BorderThickness,
                    (int)Position.Y + BorderThickness,
                    Width - 2 * BorderThickness,
                    Height - 2 * BorderThickness);

                _spriteBatch.Draw(_empty, _boundingRectangle, BorderColor);
                _spriteBatch.Draw(_empty, innerDimensions, _currentBackground);

                _spriteBatch.End();

                RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };

                //Set up the spritebatch to draw using scissoring (for text cropping)
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                                  null, null, _rasterizerState);

                Vector2 textSize = Font.MeasureString(_content);
                Vector2 textPosition = new Vector2(_boundingRectangle.Center.X, _boundingRectangle.Center.Y) - textSize / 2f;
                textPosition.X = (int)textPosition.X;
                textPosition.Y = (int)textPosition.Y + (textSize.Y - Font.LineSpacing);

                //textPosition.Y = _boundingRectangle.Bottom - Font.LineSpacing;
                //textPosition.Y = _boundingRectangle.Bottom - textSize.Y;

                if (textPosition.X < _boundingRectangle.X) // overflow reset to zero
                    textPosition.X = _boundingRectangle.X + _borderThickness;

                Rectangle currentRect = _spriteBatch.GraphicsDevice.ScissorRectangle;
                Rectangle clippingRectangle = _boundingRectangle;
                clippingRectangle.Width -= 2 * _borderThickness;

                if (currentRect.Contains(clippingRectangle))
                {
                    _spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;
                }

                _spriteBatch.DrawString(Font, _content, textPosition, Foreground);

                _spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
