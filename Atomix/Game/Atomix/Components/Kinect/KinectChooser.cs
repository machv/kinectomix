using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Atomix
{
    /// <summary>
    /// Handles Kinect initialization.
    /// </summary>
    public class KinectChooser : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _iconTexture;
        private Skeletons _skeletons;
        private KinectSensor _sensor;
        private KinectStatus _lastStatus;
        private bool _useSeatedMode;
        private bool _startColorStream;
        private bool _startDepthStream;

        /// <summary>
        /// Gets skeletons returned from the Kinect Sensor.
        /// </summary>
        /// <returns>Skeletons tracked by the Kinect sensor.</returns>
        public Skeletons Skeletons
        {
            get { return _skeletons; }
        }
        /// <summary>
        /// Gets selected Kinect sensor.
        /// </summary>
        /// <returns>Selected Kinect sensor.</returns>
        public KinectSensor Sensor
        {
            get { return _sensor; }
        }
        /// <summary>
        /// Gets the last known status of the <see cref="KinectSensor"/>.
        /// </summary>
        ///<returns>Last known status of the <see cref="KinectSensor"/>.</returns>
        public KinectStatus LastStatus
        {
            get { return _lastStatus; }
        }
        /// <summary>
        /// Gets or sets if should be used seated or nomal mode.
        /// </summary>
        /// <returns>True if seated mode is used.</returns>
        public bool UseSeatedMode
        {
            get { return _useSeatedMode; }
            set
            {
                _useSeatedMode = value;

                if (_sensor != null)
                {
                    if (value == true)
                        SetSeatedMode(_sensor);
                    else
                        SetDefaultMode(_sensor);
                }
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="KinectChooser"/>.
        /// </summary>
        /// <param name="game">Game containing this component.</param>
        public KinectChooser(Game game, bool startColorStream, bool startDepthStream)
            : base(game)
        {
            _skeletons = new Skeletons();
            _startColorStream = startColorStream;
            _startDepthStream = startDepthStream;

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            DiscoverSensor();

        }

        /// <summary>
        /// Initializes required objects for this component.
        /// </summary>
        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Updates <see cref="KinectChooser"/> for rendering phase.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            if (Sensor != null && Sensor.SkeletonStream.IsEnabled)
            {
                using (SkeletonFrame skeletonFrame = Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    if (skeletonFrame != null)
                    {
                        Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        skeletonFrame.CopySkeletonDataTo(skeletonData);

                        _skeletons.SetSkeletonData(skeletonData, skeletonFrame.Timestamp);
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the current state of the <see cref="KinectChooser"/>.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If we don't have a sensor, or the sensor we have is not connected
            // then we will display the information text
            if (Sensor == null || _lastStatus != KinectStatus.Connected)
            {
                _spriteBatch.Begin();

                float scale = 0.5f;
                Vector2 position = new Vector2();
                position.X = Game.GraphicsDevice.Viewport.Width - _iconTexture.Width - 20 + (_iconTexture.Width * scale / 2); // Centered
                position.Y = 20;

                _spriteBatch.Draw(_iconTexture, position, null, Color.White, 0, new Vector2(), scale, SpriteEffects.None, 0);

                position.X -= _iconTexture.Width * scale / 2;
                position.Y += _iconTexture.Height * scale + 10;

                // Determine the text
                string txt = "please connect sensor";
                Vector2 size = _font.MeasureString(txt);
                if (this.Sensor != null)
                {
                    txt = LastStatus.ToString();
                }

                // Render the text
                _spriteBatch.DrawString(
                    _font,
                    txt,
                    position,
                    Color.Black);

                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Loads the textures and fonts.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _font = Game.Content.Load<SpriteFont>("Fonts/Normal");
            _iconTexture = Game.Content.Load<Texture2D>("Images/KinectIcon");
        }

        /// <summary>
        /// This method ensures that the <see cref="KinectSensor"/> is stopped.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (Sensor != null)
            {
                Sensor.Stop();
            }
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // If the status is not connected, try to stop it
            if (e.Status != KinectStatus.Connected)
            {
                e.Sensor.Stop();
            }

            _lastStatus = e.Status;

            DiscoverSensor();
        }

        private string GetStatusDescription(KinectStatus kinectStatus)
        {
            string status = "Unknown";

            switch (kinectStatus)
            {
                case KinectStatus.Undefined:
                    status = "Status of the attached Kinect cannot be determined.";
                    break;
                case KinectStatus.Disconnected:
                    status = "The Kinect is not connected to the USB connector.";
                    break;
                case KinectStatus.Connected:
                    status = "The Kinect is fully connected and ready.";
                    break;
                case KinectStatus.Initializing:
                    status = "The Kinect is initializing.";
                    break;
                case KinectStatus.Error:
                    status = "Communication with the Kinect procudes errors.";
                    break;
                case KinectStatus.NotPowered:
                    status = "The Kinect is not fully powered. An additional power adapter is required.";
                    break;
                case KinectStatus.NotReady:
                    status = "Some part of the Kinect is not yet ready.";
                    break;
                case KinectStatus.DeviceNotGenuine:
                    status = "The attached device is not genuine Kinect sensor.";
                    break;
                case KinectStatus.DeviceNotSupported:
                    status = "The attached Kinect is not supported.";
                    break;
                case KinectStatus.InsufficientBandwidth:
                    status = "The USB connector does not have sufficient bandwidth.";
                    break;
            }

            return status;
        }

        private void DiscoverSensor()
        {
            KinectSensor sensor = null;

            foreach (KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    sensor = candidate;
                    break;
                }
            }

            if (sensor != null)
            {
                _lastStatus = sensor.Status;

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

                        if (_startDepthStream)
                            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                        if (_startColorStream)
                            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                        sensor.Start();

                        if (sensor.ElevationAngle != 0)
                            sensor.ElevationAngle = 0;

                        if (_useSeatedMode)
                            SetSeatedMode(sensor);
                        else
                            SetDefaultMode(sensor);

                        _sensor = sensor;
                    }
                    catch
                    {
                        _sensor = null;
                    }
                }
            }
        }

        private void SetSeatedMode(KinectSensor sensor)
        {
            if (sensor != null && sensor.DepthStream != null && sensor.SkeletonStream != null)
            {
                sensor.DepthStream.Range = DepthRange.Near;
                sensor.SkeletonStream.EnableTrackingInNearRange = true;
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        private void SetDefaultMode(KinectSensor sensor)
        {
            if (sensor != null && sensor.DepthStream != null && sensor.SkeletonStream != null)
            {
                sensor.DepthStream.Range = DepthRange.Default;
                sensor.SkeletonStream.EnableTrackingInNearRange = false;
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }
    }
}
