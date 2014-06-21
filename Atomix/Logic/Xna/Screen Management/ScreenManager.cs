using Kinectomix.Xna.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Kinectomix.Xna.ScreenManagement
{
    /// <summary>
    /// Provides basic screen management to the XNA framework. Inspired by the Game State Management demo by Microsoft, http://xbox.create.msdn.com/en-US/education/catalog/sample/game_state_management.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        private List<GameScreen> _screens;
        private GameScreen _activeScreen;
        private GameScreen _previousScreen;
        private ContentManager _content;
        private IInputProvider _input;

        /// <summary>
        /// Gets or sets the XNA <see cref="ContentManager"/> used in the game.
        /// </summary>
        /// <returns>Current <see cref="ContentManager"/>.</returns>
        public ContentManager Content
        {
            get { return _content; }
            set { _content = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="IInputProvider"/> that handles input in the game.
        /// </summary>
        /// <returns>Current <see cref="IInputProvider"/> that handles input in the game.</returns>
        public IInputProvider InputProvider
        {
            get { return _input; }
            set { _input = value; }
        }
        public GameScreen PreviousScreen
        {
            get { return _previousScreen; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenManager"/> class.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="input"><see cref="IInputProvider"/> that will be registered for the screens.</param>
        public ScreenManager(Game game, IInputProvider input)
            : base(game)
        {
            _screens = new List<GameScreen>();
            _content = game.Content;
            _input = input;
        }

        /// <summary>
        /// Registers selected <see cref="GameScreen"/> with this game screen manager and intializes that <see cref="GameScreen"/>.
        /// </summary>
        /// <param name="screen"><see cref="GameScreen"/> to add.</param>
        public void Add(GameScreen screen)
        {
            if (screen == null)
                throw new ArgumentNullException("Game screen cannot be null.");

            if (screen.ScreenManager != null)
                throw new ArgumentException("This game screen is already associated with any screen manager.");

            screen.ScreenManager = this;
            screen.Initialize();

            _screens.Add(screen);
        }

        /// <summary>
        /// Removes the <see cref="GameScreen"/> from this game screen manager.
        /// </summary>
        /// <param name="screen"><see cref="GameScreen"/> to remove.</param>
        public void Remove(GameScreen screen)
        {
            if (_activeScreen == screen)
                _activeScreen = null;

            _screens.Remove(screen);

            screen.ScreenManager = null;
        }

        /// <summary>
        /// Sets selected <see cref="GameScreen"/> as active which causes that this <see cref="GameScreen"/> will be visible.
        /// </summary>
        /// <param name="screen"></param>
        public void Activate(GameScreen screen)
        {
            if (!_screens.Contains(screen))
                throw new ArgumentException("Requested screen is not added into the screen manager yet.");

            _previousScreen = _activeScreen;
            _activeScreen = screen;

            if (_previousScreen != null)
                _previousScreen.Deactivated();

            _activeScreen.Activated();
        }

        /// <summary>
        /// Triggers updating on the currently active screen.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (_activeScreen != null)
                _activeScreen.Update(gameTime);
        }

        /// <summary>
        /// Triggers drawing on the currently active screen.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            if (_activeScreen != null)
                _activeScreen.Draw(gameTime);
        }

        /// <summary>
        /// Triggers unload content on all registered screens.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GameScreen screen in _screens)
            {
                screen.Dispose();
            }
        }
    }
}
