using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atomix.Components
{
    /// <summary>
    /// Component that displays current frame rate of the game.
    /// </summary>
    public class FrameRateInfo : DrawableGameComponent
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets font for rendering FPS information.
        /// </summary>
        /// <returns>Current font.</returns>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Creates new instance of frame rate component.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public FrameRateInfo(Game game)
            : base(game)
        {
            _content = new ContentManager(game.Services);
            _content.RootDirectory = "Content";
        }

        /// <summary>
        /// Loads required content for rendering.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = _content.Load<SpriteFont>("Fonts/Normal");
        }

        /// <summary>
        /// Unloads any used content in this component.
        /// </summary>
        protected override void UnloadContent()
        {
            _content.Unload();
        }

        /// <summary>
        /// Calculates current FPS.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Draws current FPS.
        /// </summary>
        /// <param name="gameTime">Snapshot of game timing.</param>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("FPS: {0}", frameRate);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, fps, new Vector2(21, 53), Color.Black);
            _spriteBatch.DrawString(_font, fps, new Vector2(20, 52), Color.Red);

            _spriteBatch.End();
        }
    }
}
