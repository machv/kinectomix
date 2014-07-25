using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mach.Xna.Input;
using System.Linq;

namespace Mach.Xna.Components
{
    public abstract class MessageBoxBase : DrawableGameComponent
    {
        private SpriteFont _font;
        private bool _isVisible;
        private bool _isFullscreen;
        private int _height;
        private int _buttonSpace;
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
        private int _buttonWidth;
        private int _buttonHeight;
        private Button[] _renderedButtons;
        private IInputProvider _inputProvider;

        protected Button _buttonOk;
        protected Button _buttonCancel;
        protected Button _buttonYes;
        protected Button _buttonNo;

        /// <summary>
        /// Gets or sets font used for rendering texts in message box.
        /// </summary>
        /// <returns>Font used in message box.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;

                _buttonOk.Font = _font;
                _buttonCancel.Font = _font;
                _buttonYes.Font = _font;
                _buttonNo.Font = _font;
            }
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
        /// <summary>
        /// Gets or sets space between rendered buttons on the message box.
        /// </summary>
        /// <returns>Current space between buttons.</returns>
        public int ButtonSpace
        {
            get { return _buttonSpace; }
            set { _buttonSpace = value; }
        }
        public int ButtonsWidth
        {
            get { return _buttonWidth; }
            set { _buttonWidth = value; }
        }
        public int ButtonsHeight
        {
            get { return _buttonHeight; }
            set { _buttonHeight = value; }
        }
        public IInputProvider InputProvider
        {
            get { return _inputProvider; }
        }

        /// Occurs when a result inside <see cref="KinectMessageBox"/> is selected.
        public event EventHandler<MessageBoxEventArgs> Changed;

        protected MessageBoxBase(Game game, IInputProvider inputProvider)
            : base(game)
        {
            _inputProvider = inputProvider;

            _borderWidth = 4;
            _height = 250;
            _buttonWidth = 130;
            _buttonHeight = 80;
            _buttonSpace = 50;
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

            _buttonOk.Initialize();
            _buttonCancel.Initialize();
            _buttonYes.Initialize();
            _buttonNo.Initialize();

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

                if (_renderedButtons != null)
                {
                    int buttonsWidth = _renderedButtons.Select(b => b.Width).Sum() + _buttonSpace * _renderedButtons.Length;
                    int x = GraphicsDevice.Viewport.Bounds.Width / 2 - buttonsWidth / 2;
                    int y = GraphicsDevice.Viewport.Bounds.Height / 2 - _height / 8 + _buttonHeight / 2;

                    foreach (Button button in _renderedButtons)
                    {
                        button.Position = new Vector2(x, y);
                        button.Update(gameTime);

                        x += button.Width + _buttonSpace;
                    }
                }
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

                if (_renderedButtons != null)
                {
                    foreach (Button button in _renderedButtons)
                        button.Draw(gameTime);
                }
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
            _buttons = buttons;

            Button[] buttonsToDisplay = null;

            switch (_buttons)
            {
                case MessageBoxButtons.OK:
                    buttonsToDisplay = new Button[] { _buttonOk };
                    break;
                case MessageBoxButtons.OKCancel:
                    buttonsToDisplay = new Button[] { _buttonOk, _buttonCancel };
                    break;
                case MessageBoxButtons.YesNo:
                    buttonsToDisplay = new Button[] { _buttonYes, _buttonNo };
                    break;
                case MessageBoxButtons.YesNoCancel:
                    buttonsToDisplay = new Button[] { _buttonYes, _buttonNo, _buttonCancel };
                    break;
            }

            Show(text, buttonsToDisplay);
        }

        /// <summary>
        /// Displays a message box with specified text and custom defined buttons. Buttons have to be initialized and in Tag property has to have value from <see cref="MessageBoxResult"/>.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">Array of <see cref="Button"/> containing custom initialized buttons to be rendered in the message box.</param>
        public void Show(string text, Button[] buttons)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (buttons == null)
                throw new ArgumentNullException("buttons");

            foreach (Button button in buttons)
            {
                button.Selected += button_Selected;

                // If no input provider is set use default.
                if (button.InputProvider == null)
                    button.InputProvider = _inputProvider;
            }

            _text = text;
            _renderedButtons = buttons;
            _isVisible = true;
        }

        /// <summary>
        /// Displays a message box with specified text.
        /// </summary>
        /// <param name="text"></param>
        public void Show(string text)
        {
            Show(text, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Hides a message box.
        /// </summary>
        public void Hide()
        {
            foreach (Button button in _renderedButtons)
            {
                button.Selected -= button_Selected;
            }

            _isVisible = false;
        }

        /// <summary>
        /// Handles button selected event and eventually raises <see cref="Changed"/> event.
        /// </summary>
        /// <param name="sender">Which button raised this event.</param>
        /// <param name="e">Not used parameters.</param>
        private void button_Selected(object sender, EventArgs e)
        {
            if ((sender is Button) == false)
                return;

            Button button = sender as Button;
            MessageBoxResult result = MessageBoxResult.Cancel;

            if (button.Tag is MessageBoxResult)
            {
                result = (MessageBoxResult)button.Tag;
            }

            Hide();

            OnChanged(result);
        }
    }
}
