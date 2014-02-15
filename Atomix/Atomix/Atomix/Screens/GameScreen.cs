using Atomix.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public abstract class GameScreen
    {
        ScreenManager _screenManager;

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return _screenManager; }
            internal set { _screenManager = value; }
        }

        /// <summary>
        /// Update logic for the game screen.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="isActive">True if this screen is now visible.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draw the game screen.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="isActive">True if this screen is now visible.</param>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// Load graphics content for the game screen.
        /// </summary>
        public virtual void LoadContent() { }


        /// <summary>
        /// Unload content for the game screen.
        /// </summary>
        public virtual void UnloadContent() { }
    }
}
