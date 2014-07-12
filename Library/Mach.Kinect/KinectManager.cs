using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mach.Kinect
{
    /// <summary>
    /// Handles Kinect sensor initialization.
    /// </summary>
    public partial class KinectManager : IDisposable
    {
        private KinectStatus _lastStatus;
        private bool _useSeatedMode;
        private bool _startColorStream;
        private bool _startDepthStream;
        private List<ConnectedSensor> _sensors;
        private Skeletons.SkeletonType _skeletonTrackingType;

        /// <summary>
        /// Gets skeletons for the first connected Kinect Sensor.
        /// </summary>
        /// <returns>Skeletons tracked by the Kinect sensor.</returns>
        public Skeletons Skeletons
        {
            get { return _sensors.Select(s => s.Skeletons).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the skeletons for all connected sensors.
        /// </summary>
        /// <returns>List of the skeletons for all connected sensors.</returns>
        public IEnumerable<Skeletons> AllSkeletons
        {
            get { return _sensors.Select(s => s.Skeletons); }
        }

        /// <summary>
        /// Gets the first available initialized Kinect sensor.
        /// </summary>
        /// <returns>Initialized Kinect sensor.</returns>
        public KinectSensor Sensor
        {
            get { return _sensors.Select(s => s.Sensor).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the list of all connected and initialized Kinect sensors.
        /// </summary>
        /// <returns>List of all connected and initialized Kinect sensors.</returns>
        public IEnumerable<KinectSensor> Sensors
        {
            get { return _sensors.Select(s => s.Sensor); }
        }

        /// <summary>
        /// Gets the list of all connected and initialized Kinect sensors with corresponding skeletons.
        /// </summary>
        /// <returns>List of all connected and initialized Kinect sensors with corresponding skeletons.</returns>
        public IList<ConnectedSensor> ConnectedSensors
        {
            get { return _sensors.AsReadOnly(); }
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

                if (_sensors != null)
                {
                    foreach (ConnectedSensor sensor in _sensors)
                    {
                        if (value == true)
                        {
                            SetSeatedMode(sensor.Sensor);
                        }
                        else
                        {
                            SetDefaultMode(sensor.Sensor);
                        }
                    }
                }
            }
        }

        public delegate void KinectStatusChangedEventHandler(object sender, EventArgs e);

        public event KinectStatusChangedEventHandler KinectStatusChanged;

        public KinectManager()
            : this(true, true)
        {

        }

        public KinectManager(bool startColorStream, bool startDepthStream)
            : this(startColorStream, startDepthStream, Skeletons.SkeletonType.NearestFullyTracked)
        {

        }

        public KinectManager(bool startColorStream, bool startDepthStream, Skeletons.SkeletonType trackingType)
        {
            _sensors = new List<ConnectedSensor>();
            _skeletonTrackingType = trackingType;
            //_skeletons = new Skeletons(Skeletons.SkeletonType.NearestFullyTracked);
            _startColorStream = startColorStream;
            _startDepthStream = startDepthStream;

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            DiscoverSensor();
        }

        public void ProcessUpdate()
        {
            foreach (ConnectedSensor sensor in _sensors)
            {
                ProcessUpdate(sensor);
            }
        }

        public void ProcessUpdate(ConnectedSensor sensor)
        {
            if (sensor == null)
                return;

            if (sensor.Sensor != null && sensor.Sensor.SkeletonStream.IsEnabled)
            {
                using (SkeletonFrame skeletonFrame = sensor.Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    if (skeletonFrame != null)
                    {
                        Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        skeletonFrame.CopySkeletonDataTo(skeletonData);

                        sensor.Skeletons.SetSkeletonData(skeletonData, skeletonFrame.Timestamp);
                    }
                }
            }
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status != KinectStatus.Connected)
            {
                e.Sensor.Stop();
            }

            if (e.Status == KinectStatus.Disconnected)
            {
                ConnectedSensor sensor = _sensors.Where(s => s.Sensor.DeviceConnectionId == e.Sensor.DeviceConnectionId).FirstOrDefault();
                if (sensor != null)
                {
                    _sensors.Remove(sensor);
                }
            }

            _lastStatus = e.Status;

            DiscoverSensor();
        }

        public string GetStatusDescription(KinectStatus kinectStatus)
        {
            string status = "Unknown";

            switch (kinectStatus)
            {
                case KinectStatus.Undefined:
                    status = "Status of the attached Kinect cannot be determined.";
                    break;
                case KinectStatus.Disconnected:
                    status = "The Kinect has been disconnected.";
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
                    status = "The Kinect is not fully powered.";
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
            foreach (KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    if (candidate != null)
                    {
                        DoInitialization(candidate);
                    }
                }
            }
        }

        private void DoInitialization(KinectSensor sensor)
        {
            _lastStatus = sensor.Status;

            if (KinectStatusChanged != null)
                KinectStatusChanged(this, EventArgs.Empty);

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

                    ConnectedSensor connectedSensor = new ConnectedSensor(sensor, _skeletonTrackingType);
                    _sensors.Add(connectedSensor);
                }
                catch
                {
                    _lastStatus = KinectStatus.Error;

                    if (KinectStatusChanged != null)
                        KinectStatusChanged(this, EventArgs.Empty);
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

        public void Dispose()
        {
            if (_sensors != null)
            {
                foreach (ConnectedSensor sensor in _sensors)
                {
                    if (sensor.Sensor != null)
                    {
                        sensor.Sensor.Stop();
                    }
                }
            }
        }
    }
}
