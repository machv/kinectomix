using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public interface IGameScreen
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);

        void LoadContent();

        void UnloadContent();
    }
}
