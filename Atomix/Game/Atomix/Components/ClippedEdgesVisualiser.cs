using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace Atomix.Components
{
    /// <summary>
    /// This is a game component that visualises clipped edges of currently tracked Skeleton from Kinect sensor.
    /// </summary>
    public class ClippedEdgesVisualiser : DrawableGameComponent
    {
        private const float TopRotation = (float)Math.PI;
        private const float RightRotation = (float)Math.PI * 3 / 2;
        private const float LeftRotation = (float)Math.PI / 2;
        private const float BottomRotation = 0;

        private int _edgeWidth;
        private Texture2D _edgeTexture;
        private Skeletons _skeletons;
        private SpriteBatch _spriteBatch;
        private Rectangle _verticalRectangle;
        private Rectangle _horizontalRectangle;
        private Vector2 _topEdgeOrigin;
        private Vector2 _bottomEdgeOrigin;
        private Vector2 _leftEdgeOrigin;
        private Vector2 _rightEdgeOrigin;
        private Vector2 _topEdgePosition;
        private Vector2 _bottomEdgePosition;
        private Vector2 _leftEdgePosition;
        private Vector2 _rightEdgePosition;

        /// <summary>
        /// Gets or sets width of edge displayed when part of skeleton is clipped.
        /// </summary>
        /// <returns></returns>
        public int EdgeWidth
        {
            get { return _edgeWidth; }
            set { _edgeWidth = value; }
        }

        /// <summary>
        /// Creates new instance of clipped edge visualiser.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="skeletons"></param>
        public ClippedEdgesVisualiser(Game game, Skeletons skeletons)
            : base(game)
        {
            _skeletons = skeletons;
        }

        /// <summary>
        /// Initializes game component.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Loads and prepares required content for visualise clipped edges.
        /// </summary>
        protected override void LoadContent()
        {
            _edgeTexture = Game.Content.Load<Texture2D>("Images/Edge");

            InitializeEdges();

            base.LoadContent();
        }

        /// <summary>
        /// Draws the clipped edges of tracked skeleton.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (_skeletons.TrackedSkeleton != null)
            {
                if (_skeletons.TrackedSkeleton.ClippedEdges.HasFlag(FrameEdges.Top))
                {
                    DrawEdge(_horizontalRectangle, _topEdgePosition, _topEdgeOrigin, TopRotation);
                }

                if (_skeletons.TrackedSkeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
                {
                    DrawEdge(_horizontalRectangle, _bottomEdgePosition, _bottomEdgeOrigin, BottomRotation);
                }

                if (_skeletons.TrackedSkeleton.ClippedEdges.HasFlag(FrameEdges.Left))
                {
                    DrawEdge(_verticalRectangle, _leftEdgePosition, _leftEdgeOrigin, LeftRotation);
                }

                if (_skeletons.TrackedSkeleton.ClippedEdges.HasFlag(FrameEdges.Right))
                {
                    DrawEdge(_verticalRectangle, _rightEdgePosition, _rightEdgeOrigin, RightRotation);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawEdge(Rectangle rectangle, Vector2 position, Vector2 origin, float rotation)
        {
            _spriteBatch.Draw(_edgeTexture, position, rectangle, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        private void InitializeEdges()
        {
            _edgeWidth = _edgeTexture.Width;

            _verticalRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Height, _edgeWidth);
            _horizontalRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Bounds.Width, _edgeWidth);

            _topEdgeOrigin = new Vector2(_horizontalRectangle.Width, _horizontalRectangle.Height);
            _bottomEdgeOrigin = Vector2.Zero;
            _leftEdgeOrigin = new Vector2(0, _verticalRectangle.Height);
            _rightEdgeOrigin = new Vector2(_verticalRectangle.Width, 0);

            _topEdgePosition = Vector2.Zero;
            _bottomEdgePosition = new Vector2(0, GraphicsDevice.Viewport.Bounds.Height - _edgeWidth);
            _leftEdgePosition = Vector2.Zero;
            _rightEdgePosition = new Vector2(GraphicsDevice.Viewport.Bounds.Width - _edgeWidth, 0);
        }
    }
}
