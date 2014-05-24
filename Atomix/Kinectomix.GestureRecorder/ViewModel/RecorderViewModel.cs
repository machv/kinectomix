using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.GestureRecorder.ViewModel
{
    public class RecorderViewModel : NotifyPropertyBase
    {
        private KinectSensor _sensor;
        public KinectSensor Sensor
        {
            get { return _sensor; }
            set
            {
                _sensor = value;
                OnPropertyChanged();
            }
        }

        private SkeletonViewModel _trackedJoints;
        public SkeletonViewModel TrackedJoints
        {
            get { return _trackedJoints; }
            set
            {
                _trackedJoints = value;
                OnPropertyChanged();
            }
        }

        public RecorderViewModel()
        {
            _trackedJoints = new SkeletonViewModel();
        }
    }
}
