using Kinectomix.Logic.DTW;
using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;
using System.Collections.Generic;

namespace Kinectomix.GestureRecorder.ViewModel
{
    public class GestureViewModel : NotifyPropertyBase
    {
        private Gesture _gesture;

        public string Name
        {
            get { return _gesture.Name; }
            set
            {
                _gesture.Name = value;
                OnPropertyChanged();
            }
        }

        public int Id
        {
            get { return _gesture.Id; }
            set
            {
                _gesture.Id = value;
                OnPropertyChanged();
            }
        }

        public GestureTrackingDimension Dimension
        {
            get { return _gesture.Dimension; }
            set
            {
                _gesture.Dimension = value;
                OnPropertyChanged();
            }
        }

        public List<GestureFrame> Sequence
        {
            get { return _gesture.GestureSequence; }
            set
            {
                _gesture.GestureSequence = value;
                OnPropertyChanged();
            }
        }

        public JointType[] TrackedJoints
        {
            get { return _gesture.TrackedJoints; }
            set
            {
                _gesture.TrackedJoints = value;
                OnPropertyChanged();
            }
        }

        public GestureViewModel(Gesture gesture)
        {
            _gesture = gesture;
        }
    }
}
