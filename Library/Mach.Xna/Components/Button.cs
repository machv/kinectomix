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
    public class Button : DrawableGameComponent
    {
        private Texture2D _empty;
        private IInputState _previousInputState;
        private IInputState _currentInputState;
        private Color _currentBackground;
        private Vector2 _position;
        private int _width;
        private int _height;
        private string _content;
        private int _borderThickness;
        private Color _borderColor;
        private Color _background;
        private Color _activeBackground;
        private Color _foreground;
        private Color _disabledBackground;
        private SpriteFont _font;
        private object _tag;
        private bool _isEnabled;
        private bool _isVisible;
        private bool _isFocused;

        /// <summary>
        /// Currently used <see cref="IInputProvider"/> for the input from user.
        /// </summary>
        protected IInputProvider _inputProvider;
        /// <summary>
        /// Visible bounding rectangle of the button.
        /// </summary>
        protected Rectangle _boundingRectangle;
        /// <summary>
        /// <see cref="SpriteBatch"/> used for rendering.
        /// </summary>
        protected SpriteBatch _spriteBatch;

        /// <summary>
        /// Gets or sets a value indicating whether button is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this button is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
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
        /// Gets or sets an arbitrary object value that can be used to store custom information about this button.
        /// </summary>
        /// <returns></returns>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
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
        /// Gets or sets if this button is active and accepts input.
        /// </summary>
        /// <returns>True if is active and accepts input.</returns>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                if (value == false)
                {
                    // Reset states
                    _currentInputState = null;
                    _previousInputState = null;
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether the button is focused.
        /// </summary>
        /// <value>
        /// <c>true</c> if the button is focused; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocused
        {
            get { return _isFocused; }
        }
        /// <summary>
        /// Occurs when a <see cref="Button"/> is selected (eg. clicked).
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Initializes a new instance of <see cref="Button"/>.
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
            _width = 190;
            _height = 70;
            _content = string.Empty;
            _isEnabled = true;
            _isVisible = true;
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
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

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
            _empty = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _empty.SetData(new Color[] { Color.White });

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

            if (_isEnabled)
            {
                IInputState inputState = _inputProvider.GetState();
                _isFocused = _boundingRectangle.Contains(inputState.X, inputState.Y);

                _currentBackground = _isFocused ? ActiveBackground : Background;

                if (_isFocused)
                {
                    _previousInputState = _currentInputState;
                    _currentInputState = inputState;

                    if (_previousInputState != null && _previousInputState.IsStateActive == false && _currentInputState.IsStateActive == true)
                    {
                        OnSelected();
                    }
                }
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
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows to fire selection of this button when selecting is processed manually.
        /// </summary>
        public void Select()
        {
            OnSelected();
        }

        /// <summary>
        /// Fires <see cref="Selected"/> event.
        /// </summary>
        protected void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }
    }
}
