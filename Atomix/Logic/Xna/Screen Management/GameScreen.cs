using Microsoft.Xna.Framework;
using System;

namespace Kinectomix.Xna.ScreenManagement
{
    public abstract class GameScreen : IDisposable
    {
        private GameComponentCollection _components;
        private ScreenManager _screenManager;

        /// <summary>
        /// Gets components collection.
        /// </summary>
        /// <returns></returns>
        public GameComponentCollection Components
        {
            get { return _components; }
        }
        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return _screenManager; }
            internal set { _screenManager = value; }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="GameScreen"/> class.
        /// </summary>
        public GameScreen()
        {
            _components = new GameComponentCollection();
        }

        /// <summary>
        /// Prepare game screen.
        /// </summary>
        public virtual void Initialize()
        {
            foreach (IGameComponent component in _components)
            {
                component.Initialize();
            }

            if (_screenManager.Game.GraphicsDevice != null)
            {
                LoadContent();
            }
        }

        /// <summary>
        /// Update logic for the game screen.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="isActive">True if this screen is now visible.</param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (IGameComponent component in _components)
            {
                if (component is IUpdateable)
                    (component as IUpdateable).Update(gameTime);
            }
        }

        /// <summary>
        /// Draw the game screen.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        /// <param name="isActive">True if this screen is now visible.</param>
        public virtual void Draw(GameTime gameTime)
        {
            foreach (IGameComponent component in _components)
            {
                if (component is IDrawable)
                    (component as IDrawable).Draw(gameTime);
            }
        }

        /// <summary>
        /// Load graphics content for the game screen.
        /// </summary>
        protected virtual void LoadContent() { }

        /// <summary>
        /// Unload content for the game screen.
        /// </summary>
        protected virtual void UnloadContent() { }

        public virtual void Activated() { }

        public virtual void Deactivated() { }

        /// <summary>
        /// Releases used resources used by the DrawableGameComponent and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            UnloadContent();
        }
    }
}