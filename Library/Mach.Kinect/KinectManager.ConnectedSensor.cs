using Microsoft.Kinect;

namespace Mach.Kinect
{
    public partial class KinectManager
    {
        public class ConnectedSensor
        {
            private KinectSensor _sensor;
            private Skeletons _skeletons;

            public KinectSensor Sensor
            {
                get { return _sensor; }
            }

            public Skeletons Skeletons
            {
                get { return _skeletons; }
            }

            public ConnectedSensor(KinectSensor sensor)
                : this(sensor, Skeletons.SkeletonType.NearestFullyTracked)
            {

            }

            public ConnectedSensor(KinectSensor sensor, Skeletons.SkeletonType trackingType)
            {
                _skeletons = new Skeletons(trackingType);
                _sensor = sensor;
            }
        }
    }
}
