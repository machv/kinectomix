using Mach.Xna.Kinect;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Visualises tracked skeletons by the Kinect Sensor.
    /// </summary>
    public class SkeletonRenderer : DrawableGameComponent
    {
        private Vector2 _jointOrigin; // The origin (center) location of the joint texture.
        private Vector2 _boneOrigin; // The origin (center) location of the bone texture.
        private Texture2D _jointTexture;
        private Texture2D _boneTexture;
        private SpriteBatch _spriteBatch;
        private VisualKinectManager _chooser;
        private Vector2 _offset;
        private float _scale;
        private readonly SkeletonPointMap _pointMapping;
        private ContentManager _content;

        /// <summary>
        /// A delegate method explaining how to map a SkeletonPoint from one space to another.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>The Vector2 representing the target location.</returns>
        public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

        /// <summary>
        /// Gets or sets scale ratio to alter rendering of skeletons.
        /// </summary>
        /// <returns>Current scale ratio.</returns>
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// Gets or sets offset where will be skeletons rendered on the screen.
        /// </summary>
        /// <returns>Current offset.</returns>
        public Vector2 RenderOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Gets the map method called when mapping from skeleton space to the target space is needed.
        /// </summary>
        /// <returns>Current map method.</returns>
        public SkeletonPointMap PointMapping
        {
            get { return _pointMapping; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SkeletonRenderer"/>.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> that the game component should be attached to.</param>
        /// <param name="chooser">The <see cref="VisualKinectManager"/> that manages Kinect sensor.</param>
        /// <param name="offset">The offset where rendering of the skeletons will start.</param>
        public SkeletonRenderer(Game game, VisualKinectManager chooser, Vector2 offset)
            : this(game, chooser, offset, 1)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SkeletonRenderer"/>.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> that the game component should be attached to.</param>
        /// <param name="chooser">The <see cref="VisualKinectManager"/> that manages Kinect sensor.</param>
        /// <param name="offset">The offset where rendering of the skeletons will start.</param>
        /// <param name="scale">The scale ratio for rendering skeletons.</param>
        public SkeletonRenderer(Game game, VisualKinectManager chooser, Vector2 offset, float scale)
            : base(game)
        {
            _chooser = chooser;
            _pointMapping = SkeletonToColorMap;
            _offset = offset;
            _scale = scale;
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SkeletonRenderer"/>.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> that the game component should be attached to.</param>
        /// <param name="chooser">The <see cref="VisualKinectManager"/> that manages Kinect sensor.</param>
        /// <param name="offset">The offset where rendering of the skeletons will start.</param>
        /// <param name="scale">The scale ratio for rendering skeletons.</param>
        /// <param name="pointMapping">The <see cref="SkeletonPointMap"/> that is called when mapping from skeleton space to the target space is needed.</param>
        public SkeletonRenderer(Game game, VisualKinectManager chooser, Vector2 offset, float scale, SkeletonPointMap pointMapping)
            : base(game)
        {
            _chooser = chooser;
            _pointMapping = pointMapping;
            _offset = offset;
            _scale = scale;
            _content = new ResourceContentManager(game.Services, Resources.ResourceManager);
        }

        /// <summary>
        /// This method initializes necessary objects.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// This method loads the textures for skeleton.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _jointTexture = _content.Load<Texture2D>("Joint");
            _jointOrigin = new Vector2(_jointTexture.Width * _scale, _jointTexture.Height * _scale);

            _boneTexture = _content.Load<Texture2D>("Bone");
            _boneOrigin = new Vector2(0.5f, 0.0f);
        }

        /// <summary>
        /// This method renders the currently tracked skeletons.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            DrawSkeleton(new Vector2(640, 480));

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSkeleton(Vector2 resolutions)
        {
            if (_chooser.Sensor == null)
                return;

            bool isSeated = _chooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated;

            Skeleton skeleton = _chooser.Skeletons.TrackedSkeleton;

            if (skeleton != null)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    return;

                // Draw Bones
                DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
                DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);

                if (!isSeated)
                {
                    DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
                }

                DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
                DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
                DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
                DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

                DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
                DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
                DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

                if (!isSeated)
                {
                    DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                    DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                    DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                    DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                    DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                    DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);
                }

                // Now draw the joints
                foreach (Joint j in skeleton.Joints)
                {
                    Color jointColor = Color.Green;
                    if (j.TrackingState != JointTrackingState.Tracked)
                    {
                        jointColor = Color.Yellow;
                    }

                    _spriteBatch.Draw(
                        _jointTexture,
                        _pointMapping(j.Position) + _offset,
                        null,
                        jointColor,
                        0.0f,
                        _jointOrigin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                }
            }
        }

        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            Vector2 start = _pointMapping(joints[startJoint].Position);
            Vector2 end = _pointMapping(joints[endJoint].Position);
            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / this._boneTexture.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.LightGreen;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            _spriteBatch.Draw(_boneTexture, start + _offset, null, color, angle, _boneOrigin, scale, SpriteEffects.None, 1.0f);
        }

        private Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != _chooser) && (null != _chooser.Sensor.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = _chooser.Sensor.CoordinateMapper.MapSkeletonPointToColorPoint(point, _chooser.Sensor.ColorStream.Format);
                return new Vector2(colorPt.X / _scale, colorPt.Y / _scale);
            }

            return Vector2.Zero;
        }
    }
}
