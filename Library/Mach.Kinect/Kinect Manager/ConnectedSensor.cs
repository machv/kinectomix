using Microsoft.Kinect;
using System;

namespace Mach.Kinect
{
    /// <summary>
    /// Represents connected Kinect sensor with skeletons.
    /// </summary>
    public class ConnectedSensor
    {
        private KinectSensor _sensor;
        private Skeletons _skeletons;
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
                if (_processSkeletonsAutomatically != value)
                {
                    if (_sensor != null && _sensor.SkeletonStream.IsEnabled)
                    {
                        if (value)
                        {
                            _sensor.SkeletonFrameReady += SkeletonFrameReady;
                        }
                        else
                        {
                            _sensor.SkeletonFrameReady -= SkeletonFrameReady;
                        }
                    }

                    _processSkeletonsAutomatically = value;
                }
            }
        }

        /// <summary>
        /// Gets the connected <see cref="KinectSensor"/>.
        /// </summary>
        /// <value>
        /// The connected <see cref="KinectSensor"/>.
        /// </value>
        public KinectSensor Sensor
        {
            get { return _sensor; }
        }
        /// <summary>
        /// Gets the recognized skeletons from Skeleton stream.
        /// </summary>
        /// <value>
        /// The recognized skeletons from Skeleton stream.
        /// </value>
        public Skeletons Skeletons
        {
            get { return _skeletons; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedSensor"/> class.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        public ConnectedSensor(KinectSensor sensor)
            : this(sensor, Skeletons.SkeletonTrackingType.NearestFullyTracked)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedSensor"/> class.
        /// </summary>
        /// <param name="sensor">The Kinect sensor.</param>
        /// <param name="trackingType">Type of the skeleton's tracking.</param>
        /// <exception cref="System.ArgumentNullException">sensor</exception>
        public ConnectedSensor(KinectSensor sensor, Skeletons.SkeletonTrackingType trackingType)
        {
            if (sensor == null)
                throw new ArgumentNullException("sensor");

            _skeletons = new Skeletons(trackingType);
            _sensor = sensor;
        }

        /// <summary>
        /// Reads skeletons from the sensor.
        /// </summary>
        public void ProcessSkeletons()
        {
            if (_sensor != null && _sensor.SkeletonStream.IsEnabled)
            {
                try
                {
                    using (SkeletonFrame skeletonFrame = _sensor.SkeletonStream.OpenNextFrame(0))
                    {
                        ProcessSkeletonFrame(skeletonFrame);
                    }
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Changes Kinect sensor to the seated mode, enables tracking in near range and depth stream range will be near as well.
        /// </summary>
        public void SetSeatedMode()
        {
            if (_sensor != null && _sensor.DepthStream != null && _sensor.SkeletonStream != null)
            {
                _sensor.DepthStream.Range = DepthRange.Near;
                _sensor.SkeletonStream.EnableTrackingInNearRange = true;
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        /// <summary>
        /// Changes Kinect sensor the default tracking mode.
        /// </summary>
        public void SetDefaultMode()
        {
            if (_sensor != null && _sensor.DepthStream != null && _sensor.SkeletonStream != null)
            {
                _sensor.DepthStream.Range = DepthRange.Default;
                _sensor.SkeletonStream.EnableTrackingInNearRange = false;
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }

        private void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    ProcessSkeletonFrame(skeletonFrame);
                }
            }
            catch
            { }
        }

        private void ProcessSkeletonFrame(SkeletonFrame skeletonFrame)
        {
            if (skeletonFrame != null)
            {
                Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletonData);

                _skeletons.SetSkeletonData(skeletonData, skeletonFrame.Timestamp);
            }
        }
    }
}
