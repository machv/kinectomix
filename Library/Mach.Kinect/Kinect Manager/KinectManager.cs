using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mach.Kinect
{
    /// <summary>
    /// Handles Kinect sensor initialization.
    /// </summary>
    public class KinectManager : IDisposable
    {
        private KinectStatus _lastStatus;
        private bool _useSeatedMode;
        private bool _startColorStream;
        private bool _startDepthStream;
        private bool _startSkeletonStream;
        private List<ConnectedSensor> _sensors;
        private Skeletons.SkeletonTrackingType _skeletonsTrackingType;
        private int _connectedSensorsLimit;
        private bool _processSkeletonsAutomatically;

        /// <summary>
        /// Gets or sets a value indicating whether skeletons should be processed automatically.
        /// </summary>
        /// <value>
        /// <c>true</c> if skeletons should be processed automatically; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessSkeletonsAutomatically
        {
            get
            {
                return _processSkeletonsAutomatically;
            }
            set
            {
                foreach (ConnectedSensor sensor in _sensors)
                {
                    sensor.ProcessSkeletonsAutomatically = value;
                }

                _processSkeletonsAutomatically = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ConnectedSensor"/> from the supplied instance identifier.
        /// </summary>
        /// <param name="deviceConnectionId"></param>
        /// <returns>The connected Kinect sensor.</returns>
        public ConnectedSensor this[string deviceConnectionId]
        {
            get
            {
                return _sensors.Where(s => s.Sensor != null && s.Sensor.DeviceConnectionId == deviceConnectionId).FirstOrDefault();
            }
        }

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
        /// Gets the first available initialized <see cref="ConnectedSensor"/>.
        /// </summary>
        /// <returns>Initialized Kinect sensor.</returns>
        public ConnectedSensor ConnectedSensor
        {
            get { return _sensors.FirstOrDefault(); }
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
        /// Gets or sets if should be used seated or normal mode.
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
                            sensor.SetSeatedMode();
                        }
                        else
                        {
                            sensor.SetDefaultMode();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Represents the method that will handle <see cref="KinectStatusChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public delegate void KinectStatusChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when status of any connected Kinect sensor changes.
        /// </summary>
        public event KinectStatusChangedEventHandler KinectStatusChanged;

        /// <summary>
        /// Initializes the <see cref="KinectManager"/> class.
        /// </summary>
        static KinectManager()
        {
            Localization.KinectManagerResources.Culture = System.Globalization.CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        public KinectManager()
                : this(true, true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        /// <param name="startColorStream">if set to <c>true</c> color stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startDepthStream">if set to <c>true</c> depth stream on connected Kinect sensors will be enabled.</param>
        public KinectManager(bool startColorStream, bool startDepthStream)
                : this(startColorStream, startDepthStream, int.MaxValue)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        /// <param name="startColorStream">if set to <c>true</c> color stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startDepthStream">if set to <c>true</c> depth stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startSkeletonStream">if set to <c>true</c> skeleton stream on connected Kinect sensors will be enabled.</param>
        public KinectManager(bool startColorStream, bool startDepthStream, bool startSkeletonStream)
                : this(startColorStream, startDepthStream, startSkeletonStream, int.MaxValue, Skeletons.SkeletonTrackingType.NearestFullyTracked)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        /// <param name="startColorStream">if set to <c>true</c> color stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startDepthStream">if set to <c>true</c> depth stream on connected Kinect sensors will be enabled.</param>
        /// <param name="connectedSensorsLimit">The connected sensors limit.</param>
        public KinectManager(bool startColorStream, bool startDepthStream, int connectedSensorsLimit)
                : this(startColorStream, startDepthStream, connectedSensorsLimit, Skeletons.SkeletonTrackingType.NearestFullyTracked)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        /// <param name="startColorStream">if set to <c>true</c> color stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startDepthStream">if set to <c>true</c> depth stream on connected Kinect sensors will be enabled.</param>
        /// <param name="connectedSensorsLimit">The connected sensors limit.</param>
        /// <param name="skeletonsTrackingType">Type of the skeletons tracking.</param>
        public KinectManager(bool startColorStream, bool startDepthStream, int connectedSensorsLimit, Skeletons.SkeletonTrackingType skeletonsTrackingType)
                : this(startColorStream, startDepthStream, true, connectedSensorsLimit, skeletonsTrackingType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectManager"/> class.
        /// </summary>
        /// <param name="startColorStream">if set to <c>true</c> color stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startDepthStream">if set to <c>true</c> depth stream on connected Kinect sensors will be enabled.</param>
        /// <param name="startSkeletonStream">if set to <c>true</c> skeleton stream on connected Kinect sensors will be enabled.</param>
        /// <param name="connectedSensorsLimit">The connected sensors limit.</param>
        /// <param name="skeletonsTrackingType">Type of the skeletons tracking.</param>
        public KinectManager(bool startColorStream, bool startDepthStream, bool startSkeletonStream, int connectedSensorsLimit, Skeletons.SkeletonTrackingType skeletonsTrackingType)
        {
            _startColorStream = startColorStream;
            _startDepthStream = startDepthStream;
            _startSkeletonStream = startSkeletonStream;
            _connectedSensorsLimit = connectedSensorsLimit;
            _skeletonsTrackingType = skeletonsTrackingType;

            _sensors = new List<ConnectedSensor>();

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            DiscoverSensor();
        }

        /// <summary>
        /// Processes the update on each connected Kinect sensor managed by this <see cref="KinectManager"/>.
        /// </summary>
        public void ProcessUpdate()
        {
            foreach (ConnectedSensor sensor in _sensors)
            {
                sensor.ProcessSkeletons();
            }
        }

        /// <summary>
        /// Gets the descriptive text of the <see cref="KinectStatus"/> value.
        /// </summary>
        /// <param name="kinectStatus">The kinect status.</param>
        /// <returns>The descriptive text of the <see cref="KinectStatus"/> value.</returns>
        public string GetStatusDescription(KinectStatus kinectStatus)
        {
            string status = Localization.KinectManagerResources.Unknown;

            switch (kinectStatus)
            {
                case KinectStatus.Undefined:
                    status = Localization.KinectManagerResources.Undefined;
                    break;
                case KinectStatus.Disconnected:
                    status = Localization.KinectManagerResources.Disconnected;
                    break;
                case KinectStatus.Connected:
                    status = Localization.KinectManagerResources.Connected;
                    break;
                case KinectStatus.Initializing:
                    status = Localization.KinectManagerResources.Initializing;
                    break;
                case KinectStatus.Error:
                    status = Localization.KinectManagerResources.Error;
                    break;
                case KinectStatus.NotPowered:
                    status = Localization.KinectManagerResources.NotPowered;
                    break;
                case KinectStatus.NotReady:
                    status = Localization.KinectManagerResources.NotReady;
                    break;
                case KinectStatus.DeviceNotGenuine:
                    status = Localization.KinectManagerResources.DeviceNotGenuine;
                    break;
                case KinectStatus.DeviceNotSupported:
                    status = Localization.KinectManagerResources.DeviceNotSupported;
                    break;
                case KinectStatus.InsufficientBandwidth:
                    status = Localization.KinectManagerResources.InsufficientBandwidth;
                    break;
            }

            return status;
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
            if (_sensors.Count >= _connectedSensorsLimit)
                return;

            _lastStatus = sensor.Status;

            if (KinectStatusChanged != null)
                KinectStatusChanged(this, EventArgs.Empty);

            if (sensor.Status == KinectStatus.Connected)
            {
                try
                {
                    if (_startSkeletonStream)
                    {
                        // http://msdn.microsoft.com/en-us/library/jj131024.aspx + http://msdn.microsoft.com/en-us/library/microsoft.kinect.transformsmoothparameters_properties.aspx for default values
                        TransformSmoothParameters parameters = new TransformSmoothParameters();
                        parameters.Smoothing = 0.5f;
                        parameters.Correction = 0.1f;
                        parameters.Prediction = 0.5f;
                        parameters.JitterRadius = 0.1f;
                        parameters.MaxDeviationRadius = 0.1f;

                        sensor.SkeletonStream.Enable(parameters);
                    }

                    if (_startDepthStream)
                    {
                        sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    }

                    if (_startColorStream)
                    {
                        sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    }

                    sensor.Start();

                    if (sensor.ElevationAngle != 0)
                    {
                        sensor.ElevationAngle = 0;
                    }

                    ConnectedSensor connectedSensor = new ConnectedSensor(sensor, _skeletonsTrackingType);

                    if (_useSeatedMode)
                    {
                        connectedSensor.SetSeatedMode();
                    }
                    else
                    {
                        connectedSensor.SetDefaultMode();
                    }

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

        /// <summary>
        /// Stops connected Kinect sensors.
        /// </summary>
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
