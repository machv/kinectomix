using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Atomix.Components.Common
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MessageBox : DrawableGameComponent
    {
        private int _borderWidth = 4;
        private int _height = 250;

        private SpriteBatch _spriteBatch;
        private Texture2D _empty;
        private bool _isVisible;
        private bool _isFullscreen;
        private MessageBoxButtons _buttons;
        private SpriteFont _font;
        private string _message;

        /// <summary>
        /// Gets or sets font used for rendering texts in message box.
        /// </summary>
        /// <returns>Font used in message box.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
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

        public event EventHandler<MessageBoxEventArgs> Changed;

        public MessageBox(Game game)
            : base(game)
        {

        }

        private void OnChanged(MessageBoxResult result)
        {
            if (Changed != null)
            {
                Changed(this, new MessageBoxEventArgs(result));
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _empty = Game.Content.Load<Texture2D>("Empty");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_font == null)
                throw new Exception("No font specified for rendering.");

            if (_isVisible)
            {
                _spriteBatch.Begin();

                if (_isFullscreen)
                {
                    Rectangle background = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
                    _spriteBatch.Draw(_empty, background, Color.LightGray);
                }

                Rectangle outerBox = new Rectangle(0, (GraphicsDevice.Viewport.Bounds.Height - _height) / 2, GraphicsDevice.Viewport.Bounds.Width, _height);

                _spriteBatch.Draw(_empty, outerBox, Color.Silver);

                Rectangle innerBox = new Rectangle(0, _borderWidth + (GraphicsDevice.Viewport.Bounds.Height - _height) / 2, GraphicsDevice.Viewport.Bounds.Width, _height - 2 * _borderWidth);

                _spriteBatch.Draw(_empty, innerBox, Color.Gray);

                Vector2 size = _font.MeasureString(_message);

                _spriteBatch.DrawString(_font, _message, new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, GraphicsDevice.Viewport.Bounds.Height / 2 - _height / 2 + size.Y / 2), Color.White);

                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public void Show(string message, MessageBoxButtons buttons)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _isVisible = true;
            _message = message;
            _buttons = buttons;
        }

        public void Show(string message)
        {
            Show(message, MessageBoxButtons.OK);
        }
    }
}
