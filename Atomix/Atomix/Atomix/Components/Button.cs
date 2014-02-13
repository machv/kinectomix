using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components
{
    public class Button : DrawableGameComponent
    {
        public Button(Game game) : base(game)
        {

        }

        protected override void LoadContent()
        {
            // Load textures

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // detects actions like click

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw button

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            // Remove textures 

            base.UnloadContent();
        }
    }
}
