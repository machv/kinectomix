using Mach.Xna.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mach.Xna.Components
{
    /// <summary>
    /// Base class for button implementation for use in XNA framework.
    /// </summary>
    public abstract class ButtonBase : DrawableGameComponent
    {
        private IInputState _previousInputState;
        private IInputState _currentInputState;
        private Vector2 _position;
        private object _tag;
        private int _width;
        private int _height;

        /// <summary>
        /// <c>true</c> if this button is frozen and does not respond to any action.
        /// </summary>
        protected bool _isFrozen;

        /// <summary>
        /// <c>true</c> if this button is rendered on the screen.
        /// </summary>
        protected bool _isVisible;
        /// <summary>
        /// <c>true</c> if this button is enabled and can handle user input.
        /// </summary>
        protected bool _isEnabled;
        /// <summary>
        /// <c>true</c> if this button is focused by user.
        /// </summary>
        protected bool _isFocused;
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
        public virtual bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        /// <summary>
        /// Gets or sets rendering position of this button.
        /// </summary>
        /// <returns>Top position to render.</returns>
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                SetBoundingRectangle();
            }
        }
        /// <summary>
        /// Gets or sets width of this button.
        /// </summary>
        /// <returns>Current width of the button.</returns>
        public virtual int Width
        {
            get { return _width; }
            set
            {
                if (value > 0)
                {
                    _width = value;

                    SetBoundingRectangle();
                }
            }
        }
        /// <summary>
        /// Gets or sets height of this button.
        /// </summary>
        /// <returns>Current height of the button.</returns>
        public virtual int Height
        {
            get { return _height; }
            set
            {
                if (value > 0)
                {
                    _height = value;

                    SetBoundingRectangle();
                }
            }
        }
        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information about this button.
        /// </summary>
        /// <returns></returns>
        public virtual object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Gets or sets input provider for accepting interactions from user.
        /// </summary>
        /// <returns>Current registered input provider for this button.</returns>
        public virtual IInputProvider InputProvider
        {
            get { return _inputProvider; }
            set { _inputProvider = value; }
        }
        /// <summary>
        /// Gets or sets if this button is active and accepts input.
        /// </summary>
        /// <returns>True if is active and accepts input.</returns>
        public virtual bool IsEnabled
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
        public virtual bool IsFocused
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
        public ButtonBase(Game game)
            : base(game)
        {
            _width = 190;
            _height = 70;
            _isEnabled = true;
            _isVisible = true;
        }

        /// <summary>
        /// Initializes a button component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// Checks for hover over button or any interaction from user via <see cref="IInputProvider"/> interface.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (_isEnabled && !_isFrozen)
            {
                if (_inputProvider == null)
                    throw new InvalidOperationException("No input provider is set.");

                IInputState inputState = _inputProvider.GetState();
                _isFocused = _boundingRectangle.Contains(inputState.X, inputState.Y);

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

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows to fire selection of this button when selecting is processed manually.
        /// </summary>
        public void Select()
        {
            OnSelected();
        }

        /// <summary>
        /// Freezes this button so it does not respond to user input and does not update itself.
        /// </summary>
        public virtual void Freeze()
        {
            _isFrozen = true;
        }

        /// <summary>
        /// Unfreezes this button so it can update itself.
        /// </summary>
        public virtual void Unfreeze()
        {
            _isFrozen = false;
        }

        /// <summary>
        /// Fires <see cref="Selected"/> event.
        /// </summary>
        protected void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        private void SetBoundingRectangle()
        {
            _boundingRectangle = new Rectangle((int)_position.X, (int)_position.Y, _width, _height);
        }
    }
}
