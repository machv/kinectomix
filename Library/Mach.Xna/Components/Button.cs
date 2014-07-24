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
    public partial class Button : ButtonBase
    {
        private Texture2D _empty;
        private Color _currentBackground;
        private Color _currentForeground;
        private string _content;
        private int _borderThickness;
        private int _padding;
        private Color _borderColor;
        private Color _background;
        private Color _foreground;
        private Color _activeBackground;
        private Color _activeForeground;
        private Color _disabledBackground;
        private Color _disabledForeground;
        private SpriteFont _font;
        private TextAlignment _textAlignment;
        private Viewport _levelNameViewPort;
        private Viewport _defaultViewport;
        private TextScrolling _textScrolling;

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
        /// Gets or sets the padding inside the text box.
        /// </summary>
        /// <value>
        /// The padding inside the text box.
        /// </value>
        public int Padding
        {
            get { return _padding; }
            set { _padding = value; }
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
        /// Gets or sets color of the button's foreground color when button is hovered.
        /// </summary>
        /// <returns>Foreground color when hovered.</returns>
        public Color ActiveForeground
        {
            get { return _activeForeground; }
            set { _activeForeground = value; }
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
        /// Gets or sets the <see cref="Color"/> of the foreground when button is not in enabled state.
        /// </summary>
        /// <value>
        /// The <see cref="Color"/> of the foreground when button is not in enabled state.
        /// </value>
        public Color DisabledForeground
        {
            get { return _disabledForeground; }
            set { _disabledForeground = value; }
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
        /// Gets or sets alignment of the text inside button.
        /// </summary>
        /// <value>
        /// The alignment of the text inside button.
        /// </value>
        public TextAlignment TextAlignment
        {
            get { return _textAlignment; }
            set { _textAlignment = value; }
        }
        /// <summary>
        /// Gets the rendered width of this button.
        /// </summary>
        /// <value>
        /// The buttons's width, in pixels.
        /// </value>
        public int ActualWidth
        {
            get { return _boundingRectangle.Width; }
        }
        /// <summary>
        /// Gets the rendered height of this button.
        /// </summary>
        /// <value>
        /// The buttons's height, in pixels.
        /// </value>
        public int ActualHeight
        {
            get { return _boundingRectangle.Height; }
        }
        /// <summary>
        /// Gets or sets the type of the scrolling of containing text.
        /// </summary>
        /// <value>
        /// The type of the scrolling of containing text.
        /// </value>
        public TextScrolling TextScrolling
        {
            get { return _textScrolling; }
            set { _textScrolling = value; }
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
            _activeForeground = Color.White;
            _disabledForeground = Color.White;
            _borderColor = Color.Black;
            _borderThickness = 2;
            _padding = 2;
            _textAlignment = TextAlignment.Center;
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

            _defaultViewport = GraphicsDevice.Viewport;
            _levelNameViewPort = _defaultViewport;
            _levelNameViewPort.Width = _boundingRectangle.Width;
            _levelNameViewPort.Height = _boundingRectangle.Height;
            _levelNameViewPort.X = (int)Position.X;
            _levelNameViewPort.Y = (int)Position.Y;
        }

        /// <summary>
        /// Calculates bounding box for the button.
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            _boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width + _padding * 2 + _borderThickness * 2, Height + _padding * 2 + _borderThickness * 2);

            _levelNameViewPort.X = (int)Position.X;
            _levelNameViewPort.Y = (int)Position.Y;
            _levelNameViewPort.Width = _boundingRectangle.Width;
            _levelNameViewPort.Height = _boundingRectangle.Height;
        }

        /// <summary>
        /// Checks for hover over button or any interaction from user via <see cref="IInputProvider"/> interface.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (!_isFrozen)
            {
                if (_isEnabled)
                {
                    _currentBackground = _isFocused ? _activeBackground : _background;
                    _currentForeground = _isFocused ? _activeForeground : _foreground;
                }
                else
                {
                    // When button is not active we use disabled background
                    _currentBackground = _disabledBackground;
                    _currentForeground = _disabledForeground;
                }
            }

            base.Update(gameTime);
        }

        float _scrollDifferenceX = 0;
        bool toRight = true;
        bool dokola = true;
        TimeSpan delay = TimeSpan.FromSeconds(1);
        DateTime whenContinue;

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
                    _boundingRectangle.X + _borderThickness,
                    _boundingRectangle.Y + _borderThickness,
                    Width + 2 * _padding,
                    Height + 2 * _padding);

                // draw top border
                if (_borderThickness > 0)
                {
                    Rectangle topBorder = new Rectangle(_boundingRectangle.Left, _boundingRectangle.Top, _boundingRectangle.Width, _borderThickness);
                    Rectangle bottomBorder = new Rectangle(_boundingRectangle.Left, _boundingRectangle.Bottom - _borderThickness, _boundingRectangle.Width, _borderThickness);
                    Rectangle leftBorder = new Rectangle(_boundingRectangle.Left, _boundingRectangle.Top, _borderThickness, _boundingRectangle.Height);
                    Rectangle rightBorder = new Rectangle(_boundingRectangle.Right - _borderThickness, _boundingRectangle.Top, _borderThickness, _boundingRectangle.Height);

                    _spriteBatch.Draw(_empty, topBorder, BorderColor);
                    _spriteBatch.Draw(_empty, bottomBorder, BorderColor);
                    _spriteBatch.Draw(_empty, leftBorder, BorderColor);
                    _spriteBatch.Draw(_empty, rightBorder, BorderColor);
                }

                _spriteBatch.Draw(_empty, innerDimensions, _currentBackground);

                _spriteBatch.End();


                Viewport viewPort = _levelNameViewPort;
                Rectangle bounds = _spriteBatch.GraphicsDevice.PresentationParameters.Bounds;
                int xTrans = 0;

                if (!bounds.Contains(_levelNameViewPort.Bounds))
                {
                    xTrans = viewPort.X;
                    Rectangle inter = Rectangle.Intersect(bounds, _levelNameViewPort.Bounds);
                    viewPort.Bounds = Rectangle.Empty; // inter;
                }

                if (viewPort.Bounds != Rectangle.Empty)
                {
                    // we can use scrolling, as we are on the screen
                    _spriteBatch.Begin();

                    _spriteBatch.GraphicsDevice.Viewport = viewPort;
                    Vector2 levelNameSize = Font.MeasureString(_content);

                    Rectangle bound = new Rectangle(0, 0, _levelNameViewPort.Width, _levelNameViewPort.Height);

                    Vector2 textPositionS = new Vector2(bound.Center.X, bound.Center.Y) - levelNameSize / 2f;
                    textPositionS.X = xTrans + (int)textPositionS.X;
                    textPositionS.Y = (int)textPositionS.Y + (levelNameSize.Y - Font.LineSpacing) + _padding;

                    switch (_textAlignment)
                    {
                        case TextAlignment.Left:
                            textPositionS.X = _borderThickness + _padding;
                            break;
                        case TextAlignment.Center:
                            // Center is default.
                            break;
                        case TextAlignment.Right:
                            textPositionS.X = _boundingRectangle.Width - _borderThickness - _padding - levelNameSize.X;
                            break;
                    }

                    if (levelNameSize.X > _levelNameViewPort.Width)
                    {
                        switch (_textScrolling)
                        {
                            case TextScrolling.None:
                                _scrollDifferenceX = 0;
                                break;
                            case TextScrolling.Loop:
                                if (levelNameSize.X + _scrollDifferenceX > 0)
                                {
                                    _scrollDifferenceX -= (float)(gameTime.ElapsedGameTime.TotalSeconds * 40);
                                }
                                else
                                {
                                    _scrollDifferenceX = _levelNameViewPort.Width;
                                }
                                break;
                            case TextScrolling.Slide:
                                if (whenContinue < DateTime.Now)
                                {
                                    if (toRight)
                                    {
                                        // scroll
                                        if (levelNameSize.X + _scrollDifferenceX > _levelNameViewPort.Width)
                                        {
                                            _scrollDifferenceX -= (float)(gameTime.ElapsedGameTime.TotalSeconds * 40);
                                        }
                                        else
                                        {
                                            toRight = false;
                                            _scrollDifferenceX += (float)(gameTime.ElapsedGameTime.TotalSeconds * 40);
                                        }
                                    }

                                    if (!toRight)
                                    {
                                        if (_scrollDifferenceX < 0)
                                        {
                                            _scrollDifferenceX += (float)(gameTime.ElapsedGameTime.TotalSeconds * 40);
                                        }
                                        else
                                        {
                                            toRight = true;
                                            whenContinue = DateTime.Now + delay;
                                        }
                                    }
                                }
                                break;
                        }

                        textPositionS.X = _scrollDifferenceX;
                    }

                    _spriteBatch.DrawString(Font, _content, textPositionS, _currentForeground);
                    _spriteBatch.End();
                    _spriteBatch.GraphicsDevice.Viewport = _defaultViewport;
                }
            }

            base.Draw(gameTime);
        }
    }
}
