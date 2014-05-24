using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kinectomix.GestureRecorder
{
    /// <summary>
    /// Interaction logic for KinectStreamViewer.xaml
    /// </summary>
    public partial class KinectStreamViewer : UserControl
    {
        public KinectStreamViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty KinectSensorProperty =
    DependencyProperty.Register(
        "KinectSensorManager",
        typeof(KinectSensor),
        typeof(KinectStreamViewer),
        new UIPropertyMetadata(null, KinectSensorChanged));

        public KinectSensor KinectSensor
        {
            get { return (KinectSensor)GetValue(KinectSensorProperty); }
            set { SetValue(KinectSensorProperty, value); }
        }

        private static void KinectSensorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var kinectControl = sender as KinectStreamViewer;

            if (null == kinectControl)
            {
                return;
            }

            KinectSensor sensor = args.NewValue as KinectSensor;

            if (sensor.Status != KinectStatus.Connected)
            {
                sensor.ColorFrameReady += Sensor_ColorFrameReady;
                sensor.ColorStream.Enable();

                sensor.Start();
            }

        }

        private static void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static readonly DependencyProperty FlipHorizontallyProperty =
            DependencyProperty.Register(
                "FlipHorizontally",
                typeof(bool),
                typeof(KinectStreamViewer),
                new UIPropertyMetadata(false, FlipHorizontallyChanged));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                "Stretch",
                typeof(Stretch),
                typeof(KinectStreamViewer),
                new UIPropertyMetadata(Stretch.Uniform));

        private static readonly DependencyPropertyKey HorizontalScaleTransformPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HorizontalScaleTransform",
                typeof(Transform),
                typeof(KinectStreamViewer),
                new PropertyMetadata(Transform.Identity));

        public static readonly DependencyProperty HorizontalScaleTransformProperty = HorizontalScaleTransformPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CollectFrameRateProperty =
            DependencyProperty.Register(
                "CollectFrameRate",
                typeof(bool),
                typeof(KinectStreamViewer),
                new PropertyMetadata(false));

        private static readonly DependencyPropertyKey FrameRatePropertyKey =
            DependencyProperty.RegisterReadOnly(
                "FrameRate",
                typeof(int),
                typeof(KinectStreamViewer),
                new PropertyMetadata(0));

        public static readonly DependencyProperty FrameRateProperty = FrameRatePropertyKey.DependencyProperty;

        public static readonly DependencyProperty RetainImageOnSensorChangeProperty =
            DependencyProperty.Register(
                "RetainImageOnSensorChange",
                typeof(bool),
                typeof(KinectStreamViewer),
                new PropertyMetadata(false));

        private static readonly ScaleTransform FlipXTransform = CreateFlipXTransform();

        private DateTime lastTime = DateTime.MinValue;

        public bool FlipHorizontally
        {
            get { return (bool)GetValue(FlipHorizontallyProperty); }
            set { SetValue(FlipHorizontallyProperty, value); }
        }

        public Transform HorizontalScaleTransform
        {
            get { return (Transform)GetValue(HorizontalScaleTransformProperty); }
            private set { SetValue(HorizontalScaleTransformPropertyKey, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public bool CollectFrameRate
        {
            get { return (bool)GetValue(CollectFrameRateProperty); }
            set { SetValue(CollectFrameRateProperty, value); }
        }

        public int FrameRate
        {
            get { return (int)GetValue(FrameRateProperty); }
            private set { SetValue(FrameRatePropertyKey, value); }
        }

        public bool RetainImageOnSensorChange
        {
            get { return (bool)GetValue(RetainImageOnSensorChangeProperty); }
            set { SetValue(RetainImageOnSensorChangeProperty, value); }
        }

        protected int TotalFrames { get; set; }

        protected int LastFrames { get; set; }

        protected void ResetFrameRateCounters()
        {
            if (this.CollectFrameRate)
            {
                this.lastTime = DateTime.MinValue;
                this.TotalFrames = 0;
                this.LastFrames = 0;
            }
        }

        protected void UpdateFrameRate()
        {
            if (this.CollectFrameRate)
            {
                ++this.TotalFrames;

                DateTime cur = DateTime.Now;
                var span = cur.Subtract(this.lastTime);

                if (span >= TimeSpan.FromSeconds(1))
                {
                    // A straight cast will truncate the value, leading to chronic under-reporting of framerate.
                    // rounding yields a more balanced result
                    this.FrameRate = (int)Math.Round((this.TotalFrames - this.LastFrames) / span.TotalSeconds);
                    this.LastFrames = this.TotalFrames;
                    this.lastTime = cur;
                }
            }
        }

        private static ScaleTransform CreateFlipXTransform()
        {
            var flipXTransform = new ScaleTransform(-1, 1);
            flipXTransform.Freeze();
            return flipXTransform;
        }

        private static void FlipHorizontallyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            KinectStreamViewer kinectViewer = sender as KinectStreamViewer;

            if (null != kinectViewer)
            {
                kinectViewer.HorizontalScaleTransform = (bool)args.NewValue ? FlipXTransform : Transform.Identity;
            }
        }

    }
}
