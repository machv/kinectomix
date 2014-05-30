using Kinectomix.GestureRecorder.Model;
using Kinectomix.Logic.Gestures;
using Kinectomix.Wpf.Mvvm;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;

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

        private int _recordedFrameCount;
        public int RecordedFrameCount
        {
            get { return _recordedFrameCount; }
            set
            {
                _recordedFrameCount = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _recordingRemainingTime;
        public TimeSpan RecordingRemainingTime
        {
            get { return _recordingRemainingTime; }
            set
            {
                _recordingRemainingTime = value;
                OnPropertyChanged();
            }
        }

        private GestureFileDialog _fileDialog;

        public RecorderViewModel()
        {
            _fileDialog = new GestureFileDialog();
            _trackedJoints = new SkeletonViewModel();
            _recognizer = new Recognizer();
            _startRecordingCommand = new DelegateCommand(StartRecordingCountdown, CanStartRecording);
            _startRecognizingCommand = new DelegateCommand(StartRecognizing, CanStartRecognizing);
            _addGestureCommand = new DelegateCommand(AddGesture);
            _gestures = new ObservableCollection<GestureViewModel>();

            _countDownTimer = new DispatcherTimer();
            _countDownTimer.Interval = TimeSpan.FromSeconds(_step.TotalSeconds);
            _countDownTimer.Tick += _countDownTimer_Tick;

            _recordingTimer = new DispatcherTimer();
            _recordingTimer.Interval = TimeSpan.FromSeconds(_step.TotalSeconds);
            _recordingTimer.Tick += _recordingTimer_Tick;

            KinectSensor.KinectSensors.StatusChanged += KinectSensorsChanged;

            if (KinectSensor.KinectSensors.Count > 0)
                StartKinect(KinectSensor.KinectSensors.FirstOrDefault());
        }

        private void AddGesture()
        {
            if (_fileDialog.OpenFileDialog())
            {
                XmlSerializer seralizer = new XmlSerializer(typeof(Gesture));
                using (Stream stream = File.Open(_fileDialog.FileName, FileMode.Open))
                {
                    Gesture gesture = seralizer.Deserialize(stream) as Gesture;

                    _gestures.Add(new GestureViewModel(gesture));
                    _recognizer.AddGesture(gesture);
                }
            }
        }

        private double _dtwCost;
        public double DtwCost
        {
            get { return _dtwCost; }
            set { _dtwCost = value; OnPropertyChanged(); }
        }


        private bool CanStartRecognizing(object parameter)
        {
            return true;
        }

        private void StartRecognizing()
        {
            _recognizer.Start();
        }

        private ObservableCollection<GestureViewModel> _gestures;
        public ObservableCollection<GestureViewModel> Gestures
        {
            get { return _gestures; }
            set
            {
                _gestures = value;
                OnPropertyChanged();
            }
        }

        private void _recordingTimer_Tick(object sender, EventArgs e)
        {
            RecordingRemainingTime = _recordingRemainingTime.Subtract(_step);

            if (_recordingStarted + _recordingDuration < DateTime.Now)
            {
                _recordingTimer.Stop();

                Gesture gesture = _recorder.GetRecordedGesture();
                gesture.Name = GestureName;
                gesture.Id = GestureId;

                if (_fileDialog.SaveFileDialog())
                {
                    using (Stream stream = File.Open(_fileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        SaveGestureToStream(gesture, stream);
                    }

                    _gestures.Add(new GestureViewModel(gesture));
                    _recognizer.AddGesture(gesture);
                }

                IsRecording = false;
                _recorder = null;
            }
        }

        private void SaveGestureToStream(Gesture gesture, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Gesture));
            serializer.Serialize(stream, gesture);
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

        private bool _isRecording = false;
        public bool IsRecording
        {
            get { return _isRecording; }
            protected set
            {
                _isRecording = value;
                OnPropertyChanged();
                _startRecordingCommand.RaiseCanExecuteChanged();
            }
        }

        private TimeSpan _step = new TimeSpan(0, 0, 1);
        private TimeSpan _recordingCountDownDuration = TimeSpan.Zero;
        private DispatcherTimer _countDownTimer;
        private DispatcherTimer _recordingTimer;
        private DelegateCommand _startRecordingCommand;
        public ICommand StartRecordingCommand
        {
            get { return _startRecordingCommand; }
        }

        private DelegateCommand _startRecognizingCommand;
        public ICommand StartRecognizingCommand
        {
            get { return _startRecognizingCommand; }
        }

        private DelegateCommand _addGestureCommand;
        public ICommand AddGestureCommand
        {
            get { return _addGestureCommand; }
        }
        public TimeSpan RecordingTimeout
        {
            get { return _recordingCountDownDuration; }
            set
            {
                _recordingCountDownDuration = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _recordingDuration = new TimeSpan(0, 0, 5);
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
            _recognizer.Stop();

            IsRecording = true;
            IsActiveSkeleton = false;
            RecordingRemainingTime = _recordingDuration;
            RecordingTimeout = new TimeSpan(0, 0, 5);

            _recorder = new Logic.Gestures.Recorder();
            _recorder.Start(_trackedJoints.GetSelectedJoints(), TrackingDimension.Two);

            _countDownTimer.Start();
        }

        private void _countDownTimer_Tick(object sender, EventArgs e)
        {
            if (IsActiveSkeleton)
            {
                RecordingTimeout = RecordingTimeout.Subtract(_step);

                if (RecordingTimeout == TimeSpan.Zero)
                {
                    _countDownTimer.Stop();

                    StartRecording();
                }
            }
        }

        private string _gestureName;
        public string GestureName
        {
            get { return _gestureName; }
            set
            {
                _gestureName = value;
                OnPropertyChanged();
            }
        }

        private int _gestureId;
        public int GestureId
        {
            get { return _gestureId; }
            set
            {
                _gestureId = value;
                OnPropertyChanged();
            }
        }

        private DateTime _recordingStarted;
        private Logic.Gestures.Recorder _recorder;
        private Recognizer _recognizer;
        private void StartRecording()
        {
            _recordingTimer.Start();
            _recordingStarted = DateTime.Now;
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

        private bool _isActiveSkeleton = false;
        public bool IsActiveSkeleton
        {
            get { return _isActiveSkeleton; }
            set
            {
                _isActiveSkeleton = value;
                OnPropertyChanged();
            }
        }
        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (_recorder == null && _recognizer == null)
                return;

            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);

                Skeleton trackedSkeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                if (trackedSkeleton != null)
                {
                    IsActiveSkeleton = true;

                    if (_isRecording == true && _recorder != null && RecordingTimeout <= TimeSpan.Zero)
                    {
                        _recorder.ProcessSkeleton(trackedSkeleton);

                        RecordedFrameCount = _recorder.FrameBufferCount;
                    }
                    else if (_recognizer != null)
                    {
                        _recognizer.ProcessSkeleton(trackedSkeleton);

                        DtwCost = _recognizer.GetLastCost();
                    }
                }
                else
                    IsActiveSkeleton = false;
            }
        }
    }
}
