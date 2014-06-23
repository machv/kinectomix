using Atomix;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Components.Kinect
{
    /// <summary>
    /// Handles Kinect sensor initialization.
    /// </summary>
    public class KinectManager : IDisposable
    {
        private Skeletons _skeletons;
        private KinectSensor _sensor;
        private KinectStatus _lastStatus;
        private bool _useSeatedMode;
        private bool _startColorStream;
        private bool _startDepthStream;

        /// <summary>
        /// Gets skeletons returned from the Kinect Sensor.
        /// </summary>
        /// <returns>Skeletons tracked by the Kinect sensor.</returns>
        public Skeletons Skeletons
        {
            get { return _skeletons; }
        }

        /// <summary>
        /// Gets selected Kinect sensor.
        /// </summary>
        /// <returns>Selected Kinect sensor.</returns>
        public KinectSensor Sensor
        {
            get { return _sensor; }
        }

        /// <summary>
        /// Gets the last known status of the <see cref="KinectSensor"/>.
        /// </summary>
        ///<returns>Last known status of the <see cref="KinectSensor"/>.</returns>
        public KinectStatus LastStatus
        {
            get { return _lastStatus; }
        }

        /// <summary>
        /// Gets or sets if should be used seated or nomal mode.
        /// </summary>
        /// <returns>True if seated mode is used.</returns>
        public bool UseSeatedMode
        {
            get { return _useSeatedMode; }
            set
            {
                _useSeatedMode = value;

                if (_sensor != null)
                {
                    if (value == true)
                        SetSeatedMode(_sensor);
                    else
                        SetDefaultMode(_sensor);
                }
            }
        }

        public delegate void KinectStatusChangedEventHandler(object sender, EventArgs e);

        public event KinectStatusChangedEventHandler KinectStatusChanged;

        public KinectManager(bool startColorStream, bool startDepthStream)
        {
            _skeletons = new Skeletons();
            _startColorStream = startColorStream;
            _startDepthStream = startDepthStream;

            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            DiscoverSensor();
        }

        public void ProcessUpdate()
        {
            if (Sensor != null && Sensor.SkeletonStream.IsEnabled)
            {
                using (SkeletonFrame skeletonFrame = Sensor.SkeletonStream.OpenNextFrame(0))
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

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status != KinectStatus.Connected)
            {
                e.Sensor.Stop();
            }

            if (e.Status == KinectStatus.Disconnected)
            {
                _sensor = null;
            }

            _lastStatus = e.Status;

            DiscoverSensor();
        }

        public string GetStatusDescription(KinectStatus kinectStatus)
        {
            string status = "Unknown";

            switch (kinectStatus)
            {
                case KinectStatus.Undefined:
                    status = "Status of the attached Kinect cannot be determined.";
                    break;
                case KinectStatus.Disconnected:
                    status = "The Kinect has been disconnected.";
                    break;
                case KinectStatus.Connected:
                    status = "The Kinect is fully connected and ready.";
                    break;
                case KinectStatus.Initializing:
                    status = "The Kinect is initializing.";
                    break;
                case KinectStatus.Error:
                    status = "Communication with the Kinect procudes errors.";
                    break;
                case KinectStatus.NotPowered:
                    status = "The Kinect is not fully powered.";
                    break;
                case KinectStatus.NotReady:
                    status = "Some part of the Kinect is not yet ready.";
                    break;
                case KinectStatus.DeviceNotGenuine:
                    status = "The attached device is not genuine Kinect sensor.";
                    break;
                case KinectStatus.DeviceNotSupported:
                    status = "The attached Kinect is not supported.";
                    break;
                case KinectStatus.InsufficientBandwidth:
                    status = "The USB connector does not have sufficient bandwidth.";
                    break;
            }

            return status;
        }

        private void DiscoverSensor()
        {
            KinectSensor sensor = null;

            foreach (KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    sensor = candidate;
                    break;
                }
            }

            if (sensor != null)
            {
                _lastStatus = sensor.Status;

                if (KinectStatusChanged != null)
                    KinectStatusChanged(this, EventArgs.Empty);

                if (sensor.Status == KinectStatus.Connected)
                {
                    try
                    {
                        // http://msdn.microsoft.com/en-us/library/jj131024.aspx + http://msdn.microsoft.com/en-us/library/microsoft.kinect.transformsmoothparameters_properties.aspx for default values
                        TransformSmoothParameters parameters = new TransformSmoothParameters();
                        parameters.Smoothing = 0.5f;
                        parameters.Correction = 0.5f;
                        parameters.Prediction = 0.4f;
                        parameters.JitterRadius = 1.0f;
                        parameters.MaxDeviationRadius = 0.5f;
                        parameters.Smoothing = 0.7f;
                        parameters.Correction = 0.3f;

                        sensor.SkeletonStream.Enable(parameters);

                        if (_startDepthStream)
                            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                        if (_startColorStream)
                            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                        sensor.Start();

                        if (sensor.ElevationAngle != 0)
                            sensor.ElevationAngle = 0;

                        if (_useSeatedMode)
                            SetSeatedMode(sensor);
                        else
                            SetDefaultMode(sensor);

                        _sensor = sensor;
                    }
                    catch
                    {
                        _sensor = null;
                        _lastStatus = KinectStatus.Error;

                        if (KinectStatusChanged != null)
                            KinectStatusChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void SetSeatedMode(KinectSensor sensor)
        {
            if (sensor != null && sensor.DepthStream != null && sensor.SkeletonStream != null)
            {
                sensor.DepthStream.Range = DepthRange.Near;
                sensor.SkeletonStream.EnableTrackingInNearRange = true;
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        private void SetDefaultMode(KinectSensor sensor)
        {
            if (sensor != null && sensor.DepthStream != null && sensor.SkeletonStream != null)
            {
                sensor.DepthStream.Range = DepthRange.Default;
                sensor.SkeletonStream.EnableTrackingInNearRange = false;
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
        }

        public void Dispose()
        {
            if (_sensor != null)
            {
                _sensor.Stop();
            }
        }
    }
}
