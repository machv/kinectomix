using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atomix
{
    public class SkeletonRenderer : DrawableGameComponent
    {
        private KinectChooser _chooser;
        private Skeletons _skeletons;
        Vector2 _offset;
        float _scale;

        public SkeletonRenderer(Game game, KinectChooser chooser, Skeletons skeletons, Vector2 offset, float scale)
            : base(game)
        {
            _chooser = chooser;
            _skeletons = skeletons;

            _mapMethod = SkeletonToColorMap;
            _offset = offset;
            _scale = scale;
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector2 RenderOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// The SpriteBatch used for rendering.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// This method initializes necessary objects.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        private void DrawSkeleton(Vector2 resolutions)
        {
            if (_chooser.Sensor == null)
                return;

            bool isSeated = _chooser.Sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated;

            Skeleton skeleton = _skeletons.TrackedSkeleton;

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

                    spriteBatch.Draw(
                        this.jointTexture,
                        this._mapMethod(j.Position) + _offset,
                        null,
                        jointColor,
                        0.0f,
                        this.jointOrigin,
                        1.0f,
                        SpriteEffects.None,
                        0.0f);
                }
            }
        }

        /// <summary>
        /// A delegate method explaining how to map a SkeletonPoint from one space to another.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>The Vector2 representing the target location.</returns>
        public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

        /// <summary>
        /// The origin (center) location of the joint texture.
        /// </summary>
        private Vector2 jointOrigin;

        /// <summary>
        /// The joint texture.
        /// </summary>
        private Texture2D jointTexture;

        /// <summary>
        /// The origin (center) location of the bone texture.
        /// </summary>
        private Vector2 boneOrigin;

        /// <summary>
        /// The bone texture.
        /// </summary>
        private Texture2D boneTexture;

        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap _mapMethod;

        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            Vector2 start = this._mapMethod(joints[startJoint].Position);
            Vector2 end = this._mapMethod(joints[endJoint].Position);
            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / this.boneTexture.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.LightGreen;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            spriteBatch.Draw(this.boneTexture, start + _offset, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        }

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
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

        Skeleton skeleton;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the current state of the KinectChooser.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the spritebatch is null, call initialize
            if (this.spriteBatch == null)
            {
                this.Initialize();
            }

            spriteBatch.Begin();

            DrawSkeleton(new Vector2(640, 480));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.jointTexture = Game.Content.Load<Texture2D>("Images/Joint");
            this.jointOrigin = new Vector2(this.jointTexture.Width / _scale, this.jointTexture.Height / _scale);

            this.boneTexture = Game.Content.Load<Texture2D>("Images/Bone");
            this.boneOrigin = new Vector2(0.5f, 0.0f);
        }

        /// <summary>
        /// This method ensures that the KinectSensor is stopped before exiting.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
    }

}
