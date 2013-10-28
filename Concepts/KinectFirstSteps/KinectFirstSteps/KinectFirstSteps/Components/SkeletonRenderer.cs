using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFirstSteps.Components
{
    public class SkeletonRenderer : DrawableGameComponent
    {
        private KinectSensor _kinect;
        private Skeletons _skeletons;

        public SkeletonRenderer(Game game, Microsoft.Kinect.KinectSensor sensor, Skeletons skeletons, SkeletonPointMap map)
            : base(game)
        {
            _kinect = sensor;
            _skeletons = skeletons;

            mapMethod = map;
        }

        /// <summary>
        /// The SpriteBatch used for rendering.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Gets the selected KinectSensor.
        /// </summary>
        public KinectSensor Sensor { get; private set; }

        /// <summary>
        /// This method initializes necessary objects.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        private void DrawSkeleton(Vector2 resolution)
        {
            Skeleton skeleton = _skeletons.TrackedSkeleton;

            if (skeleton != null)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    return;

                // Draw Bones
                DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
                DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
                DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
                DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
                DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
                DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
                DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

                DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
                DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
                DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

                DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

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
                        this.mapMethod(j.Position),
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
        private readonly SkeletonPointMap mapMethod;

        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            Vector2 start = this.mapMethod(joints[startJoint].Position);
            Vector2 end = this.mapMethod(joints[endJoint].Position);
            Vector2 diff = end - start;
            Vector2 scale = new Vector2(1.0f, diff.Length() / this.boneTexture.Height);

            float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

            Color color = Color.LightGreen;
            if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                joints[endJoint].TrackingState != JointTrackingState.Tracked)
            {
                color = Color.Gray;
            }

            spriteBatch.Draw(this.boneTexture, start, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        }

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        private Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != _kinect) && (null != _kinect.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = _kinect.CoordinateMapper.MapSkeletonPointToColorPoint(point, _kinect.ColorStream.Format);
                return new Vector2(colorPt.X, colorPt.Y);
            }

            return Vector2.Zero;
        }

        Skeleton skeleton;

        public override void Update(GameTime gameTime)
        {
            //if (_skeletons.Items != null)
            //{
            //    foreach (Skeleton skel in _skeletons.Items)
            //    {
            //        if (skel.TrackingState == SkeletonTrackingState.Tracked)
            //        {
            //            skeleton = skel;
            //        }
            //    }
            //}

            //if (_kinect != null)
            //{
            //    try
            //    {
            //        using (SkeletonFrame skeletonFrame = _kinect.SkeletonStream.OpenNextFrame(0))
            //        {
            //            if (skeletonFrame != null)
            //            {
            //                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                            
            //                //Copy the skeleton data to our array
            //                skeletonFrame.CopySkeletonDataTo(skeletonData);

            //                if (skeletonData != null)
            //                {
            //                    foreach (Skeleton skel in skeletonData)
            //                    {
            //                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
            //                        {
            //                            skeleton = skel;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //Report an error message
            //    }
            //}

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

            this.jointTexture = Game.Content.Load<Texture2D>("Joint");
            this.jointOrigin = new Vector2(this.jointTexture.Width / 2, this.jointTexture.Height / 2);

            this.boneTexture = Game.Content.Load<Texture2D>("Bone");
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
