using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
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

            KinectSensor.KinectSensors.StatusChanged += KinectSensorsChanged;

            if (KinectSensor.KinectSensors.Count > 0)
                StartKinect(KinectSensor.KinectSensors.FirstOrDefault());
        }

        internal void OnWindowClosing()
        {
            if (_sensor != null)
                _sensor.Stop();
        }

        private void KinectSensorsChanged(object sender, StatusChangedEventArgs e)
        {
            if (_sensor == e.Sensor && e.Status != KinectStatus.Connected)
            {
                Sensor = null;
            }

            if (_sensor == null && e.Status == KinectStatus.Connected)
            {
                StartKinect(e.Sensor);
            }
        }

        private void StartKinect(KinectSensor sensor)
        {
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.SkeletonStream.Enable();
            try
            {
                sensor.Start();

                Sensor = sensor;
            }
            catch (IOException)
            { }

        }
    }
}
