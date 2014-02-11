using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
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

        public KinectSensor Sensor { get; private set; }

        /// <summary>
        /// Gets the last known status of the KinectSensor.
        /// </summary>
        public KinectStatus LastStatus { get; private set; }

        public KinectChooser(Game game)
            : base(game)
        {
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
                        sensor.SkeletonStream.Enable();
                        sensor.DepthStream.Enable();
                        sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                        sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                        sensor.SkeletonStream.EnableTrackingInNearRange = true;

                        sensor.Start();

                        //sensor.ElevationAngle = 15;
                        //sensor.ElevationAngle = -15;
                        //sensor.ElevationAngle = 0;

                        Sensor = sensor;
                    }
                    catch(Exception e) { Sensor = null; }
                }
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

            // If the background is not loaded, load it now
            //if (this.chooserBackground == null)
            //{
            //    this.LoadContent();
            //}

            // If we don't have a sensor, or the sensor we have is not connected
            // then we will display the information text
            if (this.Sensor == null || this.LastStatus != KinectStatus.Connected)
            {
                this.spriteBatch.Begin();

                // Render the background
                //this.spriteBatch.Draw(
                //    this.chooserBackground,
                //    new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2),
                //    null,
                //    Color.White,
                //    0,
                //    new Vector2(this.chooserBackground.Width / 2, this.chooserBackground.Height / 2),
                //    1,
                //    SpriteEffects.None,
                //    0);

                // Determine the text
                string txt = "Sensor not connected";
                if (this.Sensor != null)
                {
                    txt = LastStatus.ToString();
                }

                // Render the text
                Vector2 size = this.font.MeasureString(txt);
                this.spriteBatch.DrawString(
                    this.font,
                    txt,
                    new Vector2((Game.GraphicsDevice.Viewport.Width - size.X) / 2, (Game.GraphicsDevice.Viewport.Height / 2) + size.Y),
                    Color.White);
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
