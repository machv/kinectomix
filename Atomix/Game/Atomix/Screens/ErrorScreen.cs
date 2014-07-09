using Mach.Xna.Kinect.Components;
using Mach.Xna.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mach.Kinectomix.Screens
{
    class ErrorScreen : GameScreen
    {
        private SpriteFont _splashFont;
        private SpriteFont _normalFont;
        private SpriteBatch _spriteBatch;
        private string _errorText;
        private KinectCircleCursor _cursor;
        private KinectButton _quitButton;

        public ErrorScreen(SpriteBatch spriteBatch, string errorText)
        {
            _spriteBatch = spriteBatch;
            _errorText = errorText;
        }

        public override void Initialize()
        {
            _cursor = (ScreenManager.Game as KinectomixGame).Cursor;

            _quitButton = new KinectButton(ScreenManager.Game, _cursor, "quit game");
            _quitButton.Selected += Quit_Selected;

            Components.Add(_quitButton);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _splashFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Splash");
            _normalFont = ScreenManager.Content.Load<SpriteFont>("Fonts/Normal");

            _quitButton.Font = _normalFont;
            _quitButton.InputProvider = ScreenManager.InputProvider;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _quitButton.Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - _quitButton.Width / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 + 150 + _quitButton.Height);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 size;

            string name = "Kinectomix";
            size = _splashFont.MeasureString(name);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_splashFont, name, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2 - 220), Color.Black);

            size = _normalFont.MeasureString(_errorText);
            _spriteBatch.DrawString(_normalFont, _errorText, new Vector2(ScreenManager.GraphicsDevice.Viewport.Bounds.Width / 2 - size.X / 2, ScreenManager.GraphicsDevice.Viewport.Bounds.Height / 2), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Quit_Selected(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
