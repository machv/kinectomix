using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;


namespace KinectFirstSteps
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AtomixGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectSensor kinect;
        Texture2D colorVideo;
        Texture2D skeletonTexture;

        Skeleton[] skeletonData;
        Skeleton skeleton;


        public AtomixGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // nastavíme mapper
            //mapMethod = SkeletonToColorMap;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeKinectSensor();

            Components.Add(new Components.SkeletonRenderer(this, kinect, SkeletonToColorMap));

            base.Initialize();
        }

        private void InitializeKinectSensor()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault();

            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinect.ColorFrameReady += kinect_ColorFrameReady;
            //kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            //kinect.AllFramesReady += kinect_AllFramesReady;
            kinect.SkeletonStream.Enable();

            kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default; // tracking of top body 

            kinect.Start();

            kinect.ElevationAngle = 0;

            if (skeletonTexture == null)
            {
                skeletonTexture = new Texture2D(graphics.GraphicsDevice, 30, 30);
                Color[] data = new Color[30 * 30];
                for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
                skeletonTexture.SetData(data);
            }
        }

        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorVideoFrame = e.OpenColorImageFrame())
            {
                if (colorVideoFrame != null)
                {
                    // Create array for pixel data and copy it from the image frame
                    Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                    colorVideoFrame.CopyPixelDataTo(pixelData);

                    //Convert RGBA to BGRA
                    Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                    for (int i = 0; i < pixelData.Length; i += 4)
                    {
                        bgraPixelData[i] = pixelData[i + 2];
                        bgraPixelData[i + 1] = pixelData[i + 1];
                        bgraPixelData[i + 2] = pixelData[i];
                        bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                    }

                    colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                    colorVideo.SetData(bgraPixelData);
                }
            }
        }

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //
            // Skeleton Frame
            //
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    //Copy the skeleton data to our array
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                    if (skeletonData != null)
                    {
                        foreach (Skeleton skel in skeletonData)
                        {
                            if (skel.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                skeleton = skel;
                            }
                        }
                    }


                }
            }


            using (ColorImageFrame colorVideoFrame = e.OpenColorImageFrame())
            {
                if (colorVideoFrame != null)
                {
                    // Create array for pixel data and copy it from the image frame
                    Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                    colorVideoFrame.CopyPixelDataTo(pixelData);

                    //Convert RGBA to BGRA
                    Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                    for (int i = 0; i < pixelData.Length; i += 4)
                    {
                        bgraPixelData[i] = pixelData[i + 2];
                        bgraPixelData[i + 1] = pixelData[i + 1];
                        bgraPixelData[i + 2] = pixelData[i];
                        bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                    }


                    colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                    colorVideo.SetData(bgraPixelData);
                }
            }
        }

        //private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        //{
        //    if (skeleton != null)
        //    {
        //        if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
        //            return;

        //        // Draw Bones
        //        DrawBone(skeleton.Joints, JointType.Head, JointType.ShoulderCenter);
        //        DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
        //        DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
        //        DrawBone(skeleton.Joints, JointType.ShoulderCenter, JointType.Spine);
        //        DrawBone(skeleton.Joints, JointType.Spine, JointType.HipCenter);
        //        DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipLeft);
        //        DrawBone(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

        //        DrawBone(skeleton.Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
        //        DrawBone(skeleton.Joints, JointType.ElbowLeft, JointType.WristLeft);
        //        DrawBone(skeleton.Joints, JointType.WristLeft, JointType.HandLeft);

        //        DrawBone(skeleton.Joints, JointType.ShoulderRight, JointType.ElbowRight);
        //        DrawBone(skeleton.Joints, JointType.ElbowRight, JointType.WristRight);
        //        DrawBone(skeleton.Joints, JointType.WristRight, JointType.HandRight);

        //        DrawBone(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
        //        DrawBone(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
        //        DrawBone(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

        //        DrawBone(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
        //        DrawBone(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
        //        DrawBone(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

        //        // Now draw the joints
        //        foreach (Joint j in skeleton.Joints)
        //        {
        //            Color jointColor = Color.Green;
        //            if (j.TrackingState != JointTrackingState.Tracked)
        //            {
        //                jointColor = Color.Yellow;
        //            }

        //            spriteBatch.Draw(
        //                this.jointTexture,
        //                this.mapMethod(j.Position),
        //                null,
        //                jointColor,
        //                0.0f,
        //                this.jointOrigin,
        //                1.0f,
        //                SpriteEffects.None,
        //                0.0f);
        //        }

        //        //foreach (Joint joint in skeleton.Joints)
        //        //{
        //        //    //if (joint.JointType != JointType.HandLeft) continue;

        //        //    Color color = (joint.JointType == JointType.HandLeft || joint.JointType == JointType.HandRight) ? Color.Blue : Color.Red;

        //        //    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)), (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)));
        //        //    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), color);
        //        //}
        //    }
        //}

        ///// <summary>
        ///// A delegate method explaining how to map a SkeletonPoint from one space to another.
        ///// </summary>
        ///// <param name="point">The SkeletonPoint to map.</param>
        ///// <returns>The Vector2 representing the target location.</returns>
        //public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

        ///// <summary>
        ///// The origin (center) location of the joint texture.
        ///// </summary>
        //private Vector2 jointOrigin;

        ///// <summary>
        ///// The joint texture.
        ///// </summary>
        //private Texture2D jointTexture;

        ///// <summary>
        ///// The origin (center) location of the bone texture.
        ///// </summary>
        //private Vector2 boneOrigin;

        ///// <summary>
        ///// The bone texture.
        ///// </summary>
        //private Texture2D boneTexture;

        ///// <summary>
        ///// This is the map method called when mapping from
        ///// skeleton space to the target space.
        ///// </summary>
        //private readonly SkeletonPointMap mapMethod;

        //private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        //{
        //    Vector2 start = this.mapMethod(joints[startJoint].Position);
        //    Vector2 end = this.mapMethod(joints[endJoint].Position);
        //    Vector2 diff = end - start;
        //    Vector2 scale = new Vector2(1.0f, diff.Length() / this.boneTexture.Height);

        //    float angle = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

        //    Color color = Color.LightGreen;
        //    if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
        //        joints[endJoint].TrackingState != JointTrackingState.Tracked)
        //    {
        //        color = Color.Gray;
        //    }

        //    spriteBatch.Draw(this.boneTexture, start, null, color, angle, this.boneOrigin, scale, SpriteEffects.None, 1.0f);
        //}

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        private Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != kinect) && (null != kinect.ColorStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(point, kinect.ColorStream.Format);
                return new Vector2(colorPt.X, colorPt.Y);
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //this.jointTexture = Content.Load<Texture2D>("Joint");
            //this.jointOrigin = new Vector2(this.jointTexture.Width / 2, this.jointTexture.Height / 2);

            //this.boneTexture = Content.Load<Texture2D>("Bone");
            //this.boneOrigin = new Vector2(0.5f, 0.0f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            if (kinect != null)
            {
                try
                {
                    using (SkeletonFrame skeletonFrame = kinect.SkeletonStream.OpenNextFrame(1))
                    {
                        if (skeletonFrame != null)
                        {
                            if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                            {
                                this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                            }
                            //Copy the skeleton data to our array
                            skeletonFrame.CopySkeletonDataTo(this.skeletonData);

                            if (skeletonData != null)
                            {
                                foreach (Skeleton skel in skeletonData)
                                {
                                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                                    {
                                        skeleton = skel;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Report an error message
                }
            }
        

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (colorVideo != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(colorVideo, new Rectangle(0, 0, 640, 480), Color.White);
                //DrawSkeleton(spriteBatch, new Vector2(640, 480), colorVideo);
                spriteBatch.End();
            }

            if (skeletonTexture != null)
            {
                spriteBatch.Begin();
                //DrawSkeleton(spriteBatch, new Vector2(640, 480), skeletonTexture);
                //spriteBatch.Draw(skeletonTexture, new Rectangle(0, 0, 640, 480), Color.White);


                //spriteBatch.Draw(skeletonTexture, new Rectangle(50, 50, 10, 10), Color.Red);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
