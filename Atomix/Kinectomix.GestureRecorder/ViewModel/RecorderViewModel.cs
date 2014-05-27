using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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
            _startRecordingCommand = new DelegateCommand(StartRecordingCountdown, CanStartRecording);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(TimerTick);

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

        private TimeSpan _step = new TimeSpan(0,0,1);
        private TimeSpan _recordingTimeout = TimeSpan.Zero;
        private DispatcherTimer _timer;
        private bool _isRecording = false;
        private DelegateCommand _startRecordingCommand;
        public ICommand StartRecordingCommand
        {
            get { return _startRecordingCommand; }
        }
        public TimeSpan RecordingTimeout
        {
            get { return _recordingTimeout; }
            set
            {
                _recordingTimeout = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _recordingDuration = new TimeSpan(0,0,5);
        public TimeSpan RecordingDuration
        {
            get { return _recordingDuration; }
            set
            {
                _recordingDuration = value;
                OnPropertyChanged();
            }
        }

        private void StartRecordingCountdown()
        {
            _isRecording = true;
            _startRecordingCommand.RaiseCanExecuteChanged();

            _recordingTimeout = new TimeSpan(0, 0, 5);
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            RecordingTimeout = RecordingTimeout.Subtract(_step);

            if (RecordingTimeout == TimeSpan.Zero)
            {
                _timer.Stop();

                StartRecording();
            }
        }

        private Logic.DTW.GestureRecorder _recorder;
        private void StartRecording()
        {
            MessageBox.Show("Start");
            _isRecording = false;

            _recorder = new Logic.DTW.GestureRecorder();
            _recorder.Start(_trackedJoints.GetSelectedJoints(), Logic.DTW.GestureTrackingDimension.Two);
        }

        private bool CanStartRecording(object parameter)
        {
            return _isRecording == false;
        }

        private void StartKinect(KinectSensor sensor)
        {
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;

#if DEBUG
            try
            {
                sensor.Stop();
            }
            catch
            { }
#endif
            try
            {

                sensor.Start();

                Sensor = sensor;
            }
            catch (IOException)
            { }

        }

        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (_isRecording)
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);

                    Skeleton trackedSkeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                    if (trackedSkeleton != null)
                        _recorder.ProcessSkeleton(trackedSkeleton);
                }
            }
        }
    }
}
