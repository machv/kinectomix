using Microsoft.Kinect;
using System;

namespace Mach.Kinect
{
    public class ConnectedSensor
    {
        private KinectSensor _sensor;
        private Skeletons _skeletons;
        private bool _processSkeletonsAutomatically;

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

        public KinectSensor Sensor
        {
            get { return _sensor; }
        }

        public Skeletons Skeletons
        {
            get { return _skeletons; }
        }

        public ConnectedSensor(KinectSensor sensor)
            : this(sensor, Skeletons.SkeletonTrackingType.NearestFullyTracked)
        {

        }

        public ConnectedSensor(KinectSensor sensor, Skeletons.SkeletonTrackingType trackingType)
        {
            if (sensor == null)
                throw new ArgumentNullException("sensor");

            _skeletons = new Skeletons(trackingType);
            _sensor = sensor;
        }

        public void ProcessSkeletons()
        {
            if (_sensor != null && _sensor.SkeletonStream.IsEnabled)
            {
                using (SkeletonFrame skeletonFrame = _sensor.SkeletonStream.OpenNextFrame(0))
                {
                    ProcessSkeletonFrame(skeletonFrame);
                }
            }
        }

        private void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                ProcessSkeletonFrame(skeletonFrame);
            }
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

        public void SetSeatedMode()
        {
            if (_sensor != null && _sensor.DepthStream != null && _sensor.SkeletonStream != null)
            {
                _sensor.DepthStream.Range = DepthRange.Near;
                _sensor.SkeletonStream.EnableTrackingInNearRange = true;
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        public void SetDefaultMode()
        {
            if (_sensor != null && _sensor.DepthStream != null && _sensor.SkeletonStream != null)
            {
                _sensor.DepthStream.Range = DepthRange.Default;
                _sensor.SkeletonStream.EnableTrackingInNearRange = false;
                _sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }
    }
}
