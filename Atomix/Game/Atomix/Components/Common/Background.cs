using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Atomix.Components
{
    /// <summary>
    /// Component that renders background image over all available screen area.
    /// </summary>
    public class Background : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private ContentManager _content;
        private Texture2D _background;
        private string _backgroundTextureName;

        /// <summary>
        /// Creates new instance of game background with defined background texture.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public Background(Game game, string backgroundTextureName)
            : base(game)
        {
            _spriteBatch =  new SpriteBatch(Game.GraphicsDevice);
            _content = game.Content;
            _backgroundTextureName = backgroundTextureName;
        }

        /// <summary>
        /// Loads required content for rendering.
        /// </summary>
        protected override void LoadContent()
        {
            _background = _content.Load<Texture2D>(_backgroundTextureName);
        }

        /// <summary>
        /// Draws background image over whole screen.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
