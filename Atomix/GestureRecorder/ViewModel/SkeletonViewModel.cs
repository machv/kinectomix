using System.Collections.Generic;
using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;

namespace Kinectomix.GestureRecorder.ViewModel
{
    public class SkeletonViewModel : NotifyPropertyBase
    {
        public const int JointsCount = 20;
        
        private bool[] _joints;

        public SkeletonViewModel()
        {
            _joints = new bool[JointsCount];
        }

        public bool HipCenter
        {
            get { return _joints[(int)JointType.HipCenter]; }
            set
            {
                _joints[(int)JointType.HipCenter] = value;
                OnPropertyChanged();
            }
        }


        public bool Spine
        {
            get { return _joints[(int)JointType.Spine]; }
            set
            {
                _joints[(int)JointType.Spine] = value;
                OnPropertyChanged();
            }
        }


        public bool ShoulderCenter
        {
            get { return _joints[(int)JointType.ShoulderCenter]; }
            set
            {
                _joints[(int)JointType.ShoulderCenter] = value;
                OnPropertyChanged();
            }
        }


        public bool Head
        {
            get { return _joints[(int)JointType.Head]; }
            set
            {
                _joints[(int)JointType.Head] = value;
                OnPropertyChanged();
            }
        }


        public bool ShoulderLeft
        {
            get { return _joints[(int)JointType.ShoulderLeft];  }
            set
            {
                _joints[(int)JointType.ShoulderLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool ElbowLeft
        {
            get { return _joints[(int)JointType.ElbowLeft];  }
            set
            {
                _joints[(int)JointType.ElbowLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool WristLeft
        {
            get { return _joints[(int)JointType.WristLeft];  }
            set
            {
                _joints[(int)JointType.WristLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool HandLeft
        {
            get { return _joints[(int)JointType.HandLeft]; }
            set
            {
                _joints[(int)JointType.HandLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool ShoulderRight
        {
            get { return _joints[(int)JointType.ShoulderRight]; }
            set
            {
                _joints[(int)JointType.ShoulderRight] = value;
                OnPropertyChanged();
            }
        }
        public bool ElbowRight
        {
            get { return _joints[(int)JointType.ElbowRight]; }
            set
            {
                _joints[(int)JointType.ElbowRight] = value;
                OnPropertyChanged();
            }
        }
        public bool WristRight
        {
            get { return _joints[(int)JointType.WristRight]; }
            set
            {
                _joints[(int)JointType.WristRight] = value;
                OnPropertyChanged();
            }
        }
        public bool HandRight
        {
            get { return _joints[(int)JointType.HandRight]; }
            set
            {
                _joints[(int)JointType.HandRight] = value;
                OnPropertyChanged();
            }
        }
        public bool HipLeft
        {
            get { return _joints[(int)JointType.HipLeft]; }
            set
            {
                _joints[(int)JointType.HipLeft] = value;
                OnPropertyChanged();
            }
        }

        public bool KneeLeft
        {
            get { return _joints[(int)JointType.KneeLeft]; }
            set
            {
                _joints[(int)JointType.KneeLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool AnkleLeft
        {
            get { return _joints[(int)JointType.AnkleLeft]; }
            set
            {
                _joints[(int)JointType.AnkleLeft] = value;
                OnPropertyChanged();
            }
        }
        public bool FootLeft
        {
            get { return _joints[(int)JointType.FootLeft]; }
            set
            {
                _joints[(int)JointType.FootLeft] = value;
                OnPropertyChanged();

            }
        }
        public bool HipRight
        {
            get { return _joints[(int)JointType.HipRight]; }
            set
            {
                _joints[(int)JointType.HipRight] = value;
                OnPropertyChanged();
            }
        }
        public bool KneeRight
        {
            get { return _joints[(int)JointType.KneeRight]; }
            set
            {
                _joints[(int)JointType.KneeRight] = value;
                OnPropertyChanged();
            }
        }
        public bool AnkleRight
        {
            get { return _joints[(int)JointType.AnkleRight]; }
            set
            {
                _joints[(int)JointType.AnkleRight] = value;
                OnPropertyChanged();
            }
        }
        public bool FootRight
        {
            get { return _joints[(int)JointType.FootRight]; }
            set
            {
                _joints[(int)JointType.FootRight] = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<JointType> GetSelectedJoints()
        {
            List<JointType> joints = new List<JointType>();

            for (int i = 0; i < _joints.Length; i++)
            {
                if (_joints[i] == true)
                {
                    JointType joint = (JointType)i;
                    joints.Add(joint);
                }
            }

            return joints;
        }
    }
}
