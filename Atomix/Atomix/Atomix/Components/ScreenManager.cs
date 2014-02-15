using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components
{
    public class ScreenManager : DrawableGameComponent
    {
        List<GameScreen> _screens;

        GameScreen _activeScreen;
        ContentManager _content;
        IInputProvider _input;

        public ContentManager Content { get { return _content; } }

        public IInputProvider InputProvider { get { return _input; } }

        public ScreenManager(Game game, IInputProvider input)
            : base(game)
        {
            _screens = new List<GameScreen>();
            _content = game.Content;
            _input = input;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_activeScreen != null)
                _activeScreen.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_activeScreen != null)
                _activeScreen.Draw(gameTime);
        }

        public void Add(GameScreen screen)
        {
            if (screen == null)
                throw new ArgumentNullException("Game screen cannot be null.");

            if (screen.ScreenManager != null)
                throw new ArgumentException("This game screen is already associated with any screen manager.");

            screen.ScreenManager = this;
            screen.Initialize();

            _screens.Add(screen);

            screen.LoadContent();
        }

        public void Remove(GameScreen screen)
        {
            screen.UnloadContent();

            if (_activeScreen == screen)
                _activeScreen = null;

            _screens.Remove(screen);

            screen.ScreenManager = null;
        }

        public void Activate(GameScreen screen)
        {
            if (!_screens.Contains(screen))
                throw new ArgumentException("Requested screen is not added into screen manager yet.");

            _activeScreen = screen;
        }
    }
}
