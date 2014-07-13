using Mach.Xna;
using Mach.Xna.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Cursor for Kinect sensor with circle progress animation.
    /// </summary>
    public class KinectCircleCursor : KinectCursor
    {
        private Texture2D _cicle;
        private AnimatedTexture _animatedCircle;
        private double _progress;
        private int _frame;
        private int _framesCount;
        private Vector2 _circlePosition;
        private ContentManager _content;

        /// <summary>
        /// Gets or sets progress of the circle animation, accepted values are in [0, 1] range.
        /// </summary>
        /// <returns>Progress of the circle animation.</returns>
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

        /// <summary>
        /// Creates new instance of <see cref="KinectCircleCursor"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Manager handling connected sensor.</param>
        public KinectCircleCursor(Game game, VisualKinectManager chooser) : base(game, chooser)
        {
            _animatedCircle = new AnimatedTexture(Vector2.Zero, 0, 0.5f);
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
        }

        /// <summary>
        /// Creates new instance of <see cref="KinectCircleCursor"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        /// <param name="kinectManager">Manager handling connected sensor.</param>
        /// <param name="content">ContentManager containing required assets.</param>
        public KinectCircleCursor(Game game, ContentManager content, VisualKinectManager chooser) : base(game, chooser)
        {
            _animatedCircle = new AnimatedTexture(Vector2.Zero, 0, 0.5f);
            _content = content;
        }

        /// <summary>
        /// Loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _framesCount = 18;
            _cicle = _content.Load<Texture2D>("HandCircle");
            _animatedCircle.Load(_cicle, _framesCount, _framesCount);
        }

        /// <summary>
        /// Updates position of the cursor.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _frame = 0; // Except zero
            if (CursorPosition != Vector2.Zero && _progress > 0)
            {
                _circlePosition = new Vector2(CursorPosition.X - 20, CursorPosition.Y - 20);
                _frame = (int)(_progress * _framesCount);
                if (_frame > _framesCount)
                    _frame = _framesCount - 1;
            }
        }

        /// <summary>
        /// Draws current position of the cursor.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (CursorPosition != Vector2.Zero && _frame > 0)
            {
                _spriteBatch.Begin();
                _animatedCircle.DrawFrame(_spriteBatch, _frame, _circlePosition);
                _spriteBatch.End();
            }
        }
    }
}
