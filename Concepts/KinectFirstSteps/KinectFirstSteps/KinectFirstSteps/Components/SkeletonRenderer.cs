//using Microsoft.Kinect;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace KinectFirstSteps.Components
//{
//    public class SkeletonRenderer : DrawableGameComponent 
//    {
//        public SkeletonRenderer(Game game, Microsoft.Kinect.KinectSensor sensor)
//            : base(game)
//        {
//        }

//        /// <summary>
//        /// The SpriteBatch used for rendering.
//        /// </summary>
//        private SpriteBatch spriteBatch;

//        /// <summary>
//        /// Gets the selected KinectSensor.
//        /// </summary>
//        public KinectSensor Sensor { get; private set; }

//        /// <summary>
//        /// This method initializes necessary objects.
//        /// </summary>
//        public override void Initialize()
//        {
//            base.Initialize();

//            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
//        }

//        /// <summary>
//        /// This method renders the current state of the KinectChooser.
//        /// </summary>
//        /// <param name="gameTime">The elapsed game time.</param>
//        public override void Draw(GameTime gameTime)
//        {
//            // If the spritebatch is null, call initialize
//            if (this.spriteBatch == null)
//            {
//                this.Initialize();
//            }

//            // If we don't have a sensor, or the sensor we have is not connected
//            // then we will display the information text
//            if (this.Sensor == null || this.LastStatus != KinectStatus.Connected)
//            {
//                this.spriteBatch.Begin();

//                // Render the background
//                this.spriteBatch.Draw(
//                    this.chooserBackground,
//                    new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2),
//                    null,
//                    Color.White,
//                    0,
//                    new Vector2(this.chooserBackground.Width / 2, this.chooserBackground.Height / 2),
//                    1,
//                    SpriteEffects.None,
//                    0);

//                // Determine the text
//                string txt = "Required";
//                if (this.Sensor != null)
//                {
//                    txt = this.statusMap[this.LastStatus];
//                }

//                // Render the text
//                Vector2 size = this.font.MeasureString(txt);
//                this.spriteBatch.DrawString(
//                    this.font,
//                    txt,
//                    new Vector2((Game.GraphicsDevice.Viewport.Width - size.X) / 2, (Game.GraphicsDevice.Viewport.Height / 2) + size.Y),
//                    Color.White);
//                this.spriteBatch.End();
//            }

//            base.Draw(gameTime);
//        }

//        /// <summary>
//        /// This method loads the textures and fonts.
//        /// </summary>
//        protected override void LoadContent()
//        {
//            base.LoadContent();

//            this.chooserBackground = Game.Content.Load<Texture2D>("ChooserBackground");
//            this.font = Game.Content.Load<SpriteFont>("Segoe16");
//        }

//        /// <summary>
//        /// This method ensures that the KinectSensor is stopped before exiting.
//        /// </summary>
//        protected override void UnloadContent()
//        {
//            base.UnloadContent();

//            // Always stop the sensor when closing down
//            if (this.Sensor != null)
//            {
//                this.Sensor.Stop();
//            }
//        }
//    }
//}
