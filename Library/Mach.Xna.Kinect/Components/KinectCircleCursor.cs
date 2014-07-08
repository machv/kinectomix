using Mach.Xna;
using Mach.Xna.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.Components
{
    public class KinectCircleCursor : KinectCursor
    {
        private Texture2D _cicle;
        private AnimatedTexture _animatedCircle;
        private double _progress;
        private int _frame;
        private int _framesCount;
        private Vector2 _circlePosition;
        private ContentManager _content;

        public double Progress
        {
            get { return _progress; }
            set
            {
                if (value >= 0 && value <= 1)
                    _progress = value;

                if (value > 1)
                    _progress = 1;

                if (value < 0)
                    _progress = 0;
            }
        }

        public KinectCircleCursor(Game game, VisualKinectManager chooser) : base(game, chooser)
        {
            _animatedCircle = new AnimatedTexture(Vector2.Zero, 0, 0.5f);
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
        }

        public KinectCircleCursor(Game game, ContentManager content, VisualKinectManager chooser) : base(game, chooser)
        {
            _animatedCircle = new AnimatedTexture(Vector2.Zero, 0, 0.5f);
            _content = content;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _framesCount = 18;
            _cicle = _content.Load<Texture2D>("HandCircle");
            _animatedCircle.Load(_cicle, _framesCount, _framesCount);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _frame = 0; // Except zero
            if (HandPosition != Vector2.Zero && _progress > 0)
            {
                _circlePosition = new Vector2(HandPosition.X - 20, HandPosition.Y - 20);
                _frame = (int)(_progress * _framesCount);
                if (_frame > _framesCount)
                    _frame = _framesCount - 1;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (HandPosition != Vector2.Zero && _frame > 0)
            {
                _spriteBatch.Begin();
                _animatedCircle.DrawFrame(_spriteBatch, _frame, _circlePosition);
                _spriteBatch.End();
            }
        }
    }
}
