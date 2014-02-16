using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components
{
    public class Background : DrawableGameComponent
    {
        SpriteBatch _spriteBatch;
        ContentManager _content;
        Texture2D _background;

        public Background(Game game)
            : base(game)
        {
            _spriteBatch =  new SpriteBatch(Game.GraphicsDevice); ;
            _content = game.Content;
        }

        protected override void LoadContent()
        {
            _background = _content.Load<Texture2D>("Background");
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
