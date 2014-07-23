using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Components
{
    public class SpriteButton : ButtonBase
    {
        private Texture2D _normalTexture;
        private Texture2D _focusedTexture;
        private Texture2D _disabledTexture;
        private Texture2D _currentTexture;
        private float _textureScale;
        /// <summary>
        /// Gets or sets the normal texture for the button.
        /// </summary>
        /// <value>
        /// The texture for the button.
        /// </value>
        public Texture2D Texture
        {
            get { return _normalTexture; }
            set { _normalTexture = value; }
        }
        /// <summary>
        /// Gets or sets the texture when button is hovered.
        /// </summary>
        /// <value>
        /// The texture when button is hovered.
        /// </value>
        public Texture2D Focused
        {
            get { return _focusedTexture; }
            set { _focusedTexture = value; }
        }
        /// <summary>
        /// Gets or sets the texture when button is disabled.
        /// </summary>
        /// <value>
        /// The texture when button is disabled.
        /// </value>
        public Texture2D Disabled
        {
            get { return _disabledTexture; }
            set { _disabledTexture = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteButton"/> class.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public SpriteButton(Game game) : base(game) { }

        /// <summary>
        /// Updates correct texture for the button.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            if (!_isFrozen)
            {
                if (_isEnabled)
                {
                    _currentTexture = _normalTexture;
                    if (_isFocused && _focusedTexture != null)
                    {
                        _currentTexture = _focusedTexture;
                    }
                }
                else
                {
                    _currentTexture = _disabledTexture != null ? _disabledTexture : _normalTexture;
                }

                float textureRatio = _currentTexture.Width / _currentTexture.Height;
                float buttonRatio = Width / Height;
                float scale = textureRatio < buttonRatio ? _currentTexture.Width / Width : _currentTexture.Height / Height;
                int width = (int)(scale * Height);

                _textureScale = (float)Height / _currentTexture.Height;
                _boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, Height);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this button on the screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_isVisible && _currentTexture != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_currentTexture, Position, null, Color.White, 0, Vector2.Zero, _textureScale, SpriteEffects.None, 0);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
