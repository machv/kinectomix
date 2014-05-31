﻿using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Atomix
{
    /// <summary>
    /// Kinect status for user.
    /// </summary>
    public class KinectChooser : DrawableGameComponent
    {
        /// <summary>
        /// The SpriteBatch used for rendering.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The font for rendering the state text.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// Kinect Icon texture.
        /// </summary>
        private Texture2D icon;

        public Skeletons Skeletons { get; private set; }

        public KinectSensor Sensor { get; private set; }

        /// <summary>
        /// Gets the last known status of the KinectSensor.
        /// </summary>
        public KinectStatus LastStatus { get; private set; }

        public KinectChooser(Game game)
            : base(game)
        {
            Skeletons = new Skeletons();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            DiscoverSensor();

        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // If the status is not connected, try to stop it
            if (e.Status != KinectStatus.Connected)
            {
                e.Sensor.Stop();
            }

            this.LastStatus = e.Status;
            this.DiscoverSensor();
        }

        public InteractionStream Interactions { get; private set; }

        private void DiscoverSensor()
        {
            KinectSensor sensor = KinectSensor.KinectSensors.FirstOrDefault();
            if (sensor != null)
            {
                LastStatus = sensor.Status;

                if (sensor.Status == KinectStatus.Connected)
                {
                    try
                    {
                        // http://msdn.microsoft.com/en-us/library/jj131024.aspx + http://msdn.microsoft.com/en-us/library/microsoft.kinect.transformsmoothparameters_properties.aspx for default values
                        TransformSmoothParameters parameters = new TransformSmoothParameters();
                        parameters.Smoothing = 0.5f;
                        parameters.Correction = 0.5f;
                        parameters.Prediction = 0.4f;
                        parameters.JitterRadius = 1.0f;
                        parameters.MaxDeviationRadius = 0.5f;

                        parameters.Smoothing = 0.7f;
                        parameters.Correction = 0.3f;

                        sensor.SkeletonStream.Enable(parameters);

                        sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                        sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                        //EnableSeatedMode(sensor);

                        KinectInteractionClient ic = new KinectInteractionClient();
                        Interactions = new Microsoft.Kinect.Toolkit.Interaction.InteractionStream(sensor, ic);

                        sensor.Start();

                        //sensor.ElevationAngle = 10;
                        //sensor.ElevationAngle = -5;
                        //sensor.ElevationAngle = 0;

                        Sensor = sensor;
                    }
                    catch
                    {
                        Sensor = null;
                    }
                }
            }
        }

        private void EnableSeatedMode(KinectSensor sensor)
        {
            if (sensor != null && sensor.DepthStream != null && sensor.SkeletonStream != null)
            {   
                sensor.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
                sensor.SkeletonStream.EnableTrackingInNearRange = true; // enable returning skeletons while depth is in Near Range
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use seated tracking
            }
        }

        /// <summary>
        /// This method initializes necessary objects.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public Skeleton[] SkeletonData { get; set; }
        public long SkeletonTimestamp { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (Sensor != null && Sensor.SkeletonStream.IsEnabled)
            {
                using (SkeletonFrame skeletonFrame = Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    if (skeletonFrame != null)
                    {
                        SkeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];

                        // Copy the skeleton data to our array
                        skeletonFrame.CopySkeletonDataTo(SkeletonData);

                        SkeletonTimestamp = skeletonFrame.Timestamp;

                        Skeletons.SetSkeletonData(SkeletonData);
                    }
                }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the current state of the KinectChooser.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the spritebatch is null, call initialize
            if (spriteBatch == null)
            {
                Initialize();
            }

            // If we don't have a sensor, or the sensor we have is not connected
            // then we will display the information text
            if (this.Sensor == null || this.LastStatus != KinectStatus.Connected)
            {
                this.spriteBatch.Begin();

                float scale = 0.5f;
                Vector2 position = new Vector2();
                position.X = Game.GraphicsDevice.Viewport.Width - icon.Width - 20 + (icon.Width * scale / 2); // Centered
                position.Y = 20;

                spriteBatch.Draw(icon, position, null, Color.White, 0, new Vector2(), scale, SpriteEffects.None, 0);

                position.X -= icon.Width * scale / 2;
                position.Y += icon.Height * scale + 10;

                // Determine the text
                string txt = "please connect sensor";
                Vector2 size = font.MeasureString(txt);
                if (this.Sensor != null)
                {
                    txt = LastStatus.ToString();
                }

                // Render the text
                this.spriteBatch.DrawString(
                    this.font,
                    txt,
                    position,
                    Color.Black);

                this.spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            //this.chooserBackground = Game.Content.Load<Texture2D>("ChooserBackground");
            font = Game.Content.Load<SpriteFont>("Fonts/Normal");
            icon = Game.Content.Load<Texture2D>("Images/KinectIcon");
        }

        /// <summary>
        /// This method ensures that the KinectSensor is stopped before exiting.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            // Always stop the sensor when closing down
            if (this.Sensor != null)
            {
                this.Sensor.Stop();
            }
        }
    }
}
