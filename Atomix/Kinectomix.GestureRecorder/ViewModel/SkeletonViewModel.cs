using System;
using System.Collections.Generic;
using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;

namespace Kinectomix.GestureRecorder.ViewModel
{
    public class SkeletonViewModel : NotifyPropertyBase
    {
        private bool _hipCenter;
        private bool _spine;
        private bool _shoulderCenter;
        private bool _head;
        private bool _shoulderLeft;
        private bool _elbowLeft;
        private bool _wristLeft;
        private bool _handLeft;
        private bool _shoulderRight;
        private bool _elbowRight;
        private bool _wristRight;
        private bool _handRight;
        private bool _hipLeft;
        private bool _kneeLeft;
        private bool _ankleLeft;
        private bool _footLeft;
        private bool _hipRight;
        private bool _kneeRight;
        private bool _ankleRight;
        private bool _footRight;


        public bool HipCenter
        {
            get { return _hipCenter; }
            set
            {
                _hipCenter = value;
                OnPropertyChanged();
            }
        }


        public bool Spine
        {
            get { return _spine; }
            set
            {
                _spine = value; OnPropertyChanged();
            }
        }


        public bool ShoulderCenter
        {
            get { return _shoulderCenter; }
            set
            {
                _shoulderCenter = value;
                OnPropertyChanged();
            }
        }


        public bool Head
        {
            get { return _head; }
            set
            {
                _head = value;
                OnPropertyChanged();
            }
        }


        public bool ShoulderLeft
        {
            get { return _shoulderLeft; }
            set
            {
                _shoulderLeft = value;
                OnPropertyChanged();
            }
        }
        public bool ElbowLeft
        {
            get { return _elbowLeft; }
            set
            {
                _elbowLeft = value;
                OnPropertyChanged();
            }
        }
        public bool WristLeft
        {
            get { return _wristLeft; }
            set
            {
                _wristLeft = value;
                OnPropertyChanged();
            }
        }
        public bool HandLeft
        {
            get { return _handLeft; }
            set
            {
                _handLeft = value;
                OnPropertyChanged();
            }
        }
        public bool ShoulderRight
        {
            get { return _shoulderRight; }
            set
            {
                _shoulderRight = value;
                OnPropertyChanged();
            }
        }
        public bool ElbowRight
        {
            get { return _elbowRight; }
            set
            {
                _elbowRight = value;
                OnPropertyChanged();
            }
        }
        public bool WristRight
        {
            get { return _wristRight; }
            set
            {
                _wristRight = value;
                OnPropertyChanged();
            }
        }
        public bool HandRight
        {
            get { return _handRight; }
            set
            {
                _handRight = value;
                OnPropertyChanged();
            }
        }
        public bool HipLeft
        {
            get { return _hipLeft; }
            set
            {
                _hipLeft = value;
                OnPropertyChanged();
            }
        }

        internal IEnumerable<JointType> GetSelectedJoints()
        {
            throw new NotImplementedException();
        }

        public bool KneeLeft
        {
            get { return _kneeLeft; }
            set
            {
                _kneeLeft = value;
                OnPropertyChanged();
            }
        }
        public bool AnkleLeft
        {
            get { return _ankleLeft; }
            set
            {
                _ankleLeft = value;
                OnPropertyChanged();
            }
        }
        public bool FootLeft
        {
            get { return _footLeft; }
            set
            {
                _footLeft = value;
                OnPropertyChanged();

            }
        }
        public bool HipRight
        {
            get { return _hipRight; }
            set
            {
                _hipRight = value;
                OnPropertyChanged();
            }
        }
        public bool KneeRight
        {
            get { return _kneeRight; }
            set
            {
                _kneeRight = value;
                OnPropertyChanged();
            }
        }
        public bool AnkleRight
        {
            get { return _ankleRight; }
            set
            {
                _ankleRight = value;
                OnPropertyChanged();
            }
        }
        public bool FootRight
        {
            get { return _footRight; }
            set
            {
                _footRight = value;
                OnPropertyChanged();
            }
        }
    }
}
