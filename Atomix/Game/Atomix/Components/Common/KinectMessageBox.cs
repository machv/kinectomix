using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mach.Xna.Input;
using Mach.Xna.Components;
using Mach.Xna.Kinect.Components;

namespace Atomix.Components.Common
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class KinectMessageBox : DrawableGameComponent
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
        private KinectCursor _cursor;
        private KinectButton _buttonOk;
        private KinectButton _buttonCancel;
        private KinectButton _buttonYes;
        private KinectButton _buttonNo;
        private int _buttonWidth;
        private int _buttonHeight;
        private Button[] _renderedButtons;
        private IInputProvider _inputProvider;

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

        /// Occurs when a result inside <see cref="KinectMessageBox"/> is selected.
        public event EventHandler<MessageBoxEventArgs> Changed;

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectMessageBox"/> component.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="cursor">Kinect cursor used in game.</param>
        public KinectMessageBox(Game game, IInputProvider inputProvider, KinectCursor cursor)
            : base(game)
        {
            _inputProvider = inputProvider;
            _cursor = cursor;
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
            _buttonOk = new KinectButton(Game, _cursor, "OK") { Tag = MessageBoxResult.OK, Width = _buttonWidth, Height = _buttonHeight, InputProvider = _inputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonCancel = new KinectButton(Game, _cursor, "Cancel") { Tag = MessageBoxResult.Cancel, Width = _buttonWidth, Height = _buttonHeight, InputProvider = _inputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonYes = new KinectButton(Game, _cursor, "Yes") { Tag = MessageBoxResult.Yes, Width = _buttonWidth, Height = _buttonHeight, InputProvider = _inputProvider, Background = Color.DarkGray, BorderColor = Color.White };
            _buttonNo = new KinectButton(Game, _cursor, "No") { Tag = MessageBoxResult.No, Width = _buttonWidth, Height = _buttonHeight, InputProvider = _inputProvider, Background = Color.DarkGray, BorderColor = Color.White };

            _buttonOk.Selected += _button_Selected;
            _buttonCancel.Selected += _button_Selected;
            _buttonYes.Selected += _button_Selected;
            _buttonNo.Selected += _button_Selected;

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
                    int x = GraphicsDevice.Viewport.Bounds.Width / 2 - (_renderedButtons.Length * (_buttonWidth + _buttonSpace)) / 2;
                    int y = GraphicsDevice.Viewport.Bounds.Height / 2 - _height / 8 + _buttonHeight / 2;

                    foreach (Button button in _renderedButtons)
                    {
                        button.Position = new Vector2(x, y);
                        button.Update(gameTime);

                        x += _buttonWidth + _buttonSpace;
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
            if (text == null)
                throw new ArgumentNullException("text");

            _isVisible = true;
            _text = text;
            _buttons = buttons;

            switch (_buttons)
            {
                case MessageBoxButtons.OK:
                    _renderedButtons = new Button[] { _buttonOk };
                    break;
                case MessageBoxButtons.OKCancel:
                    _renderedButtons = new Button[] { _buttonOk, _buttonCancel };
                    break;
                case MessageBoxButtons.YesNo:
                    _renderedButtons = new Button[] { _buttonYes, _buttonNo };
                    break;
                case MessageBoxButtons.YesNoCancel:
                    _renderedButtons = new Button[] { _buttonYes, _buttonNo, _buttonCancel };
                    break;
            }
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
            _isVisible = false;
        }

        /// <summary>
        /// Handles button selected event and eventually raises <see cref="Changed"/> event.
        /// </summary>
        /// <param name="sender">Which button raised this event.</param>
        /// <param name="e">Not used parameters.</param>
        private void _button_Selected(object sender, EventArgs e)
        {
            if ((sender is Button) == false)
                return;

            Button button = sender as Button;
            MessageBoxResult result = (MessageBoxResult)button.Tag;

            Hide();
            OnChanged(result);
        }
    }
}
