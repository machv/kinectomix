using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.GestureRecorder.Control
{
    /// <summary>
    /// Interaction logic for KinectStreamViewer.xaml
    /// </summary>
    public partial class KinectStreamViewer : UserControl
    {
        private ColorImageFormat lastImageFormat = ColorImageFormat.Undefined;
        private byte[] rawPixelData;
        private byte[] pixelData;
        private WriteableBitmap outputImage;

        public KinectStreamViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty KinectSensorProperty =
    DependencyProperty.Register(
        "KinectSensor",
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
            var streamControl = sender as KinectStreamViewer;

            if (null == streamControl)
            {
                return;
            }

            KinectSensor sensor = args.NewValue as KinectSensor;

            sensor.ColorFrameReady += streamControl.Sensor_ColorFrameReady;

            if (sensor.Status != KinectStatus.Connected)
            {
                sensor.ColorStream.Enable();

                sensor.Start();
            }

        }

        protected void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    // We need to detect if the format has changed.
                    bool haveNewFormat = lastImageFormat != imageFrame.Format;
                    bool convertToRgb = false;
                    int bytesPerPixel = imageFrame.BytesPerPixel;

                    if (imageFrame.Format == ColorImageFormat.RawBayerResolution640x480Fps30 ||
                        imageFrame.Format == ColorImageFormat.RawBayerResolution1280x960Fps12)
                    {
                        convertToRgb = true;
                        bytesPerPixel = 4;
                    }

                    if (haveNewFormat)
                    {
                        if (convertToRgb)
                        {
                            rawPixelData = new byte[imageFrame.PixelDataLength];
                            pixelData = new byte[bytesPerPixel * imageFrame.Width * imageFrame.Height];
                        }
                        else
                        {
                            this.pixelData = new byte[imageFrame.PixelDataLength];
                        }
                    }

                    if (convertToRgb)
                    {
                        imageFrame.CopyPixelDataTo(this.rawPixelData);
                        ConvertBayerToRgb32(imageFrame.Width, imageFrame.Height);
                    }
                    else
                    {
                        imageFrame.CopyPixelDataTo(this.pixelData);
                    }

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (haveNewFormat)
                    {
                        PixelFormat format = PixelFormats.Bgr32;
                        if (imageFrame.Format == ColorImageFormat.InfraredResolution640x480Fps30)
                        {
                            format = PixelFormats.Gray16;
                        }

                        kinectColorImage.Visibility = Visibility.Visible;
                        this.outputImage = new WriteableBitmap(
                            imageFrame.Width,
                            imageFrame.Height,
                            96,  // DpiX
                            96,  // DpiY
                            format,
                            null);

                        this.kinectColorImage.Source = this.outputImage;
                    }

                    this.outputImage.WritePixels(
                        new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                        this.pixelData,
                        imageFrame.Width * bytesPerPixel,
                        0);

                    this.lastImageFormat = imageFrame.Format;

                    UpdateFrameRate();
                }
            }

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

        private void ConvertBayerToRgb32(int width, int height)
        {
            // Demosaic using a basic nearest-neighbor algorithm, operating on groups of four pixels.
            for (int y = 0; y < height; y += 2)
            {
                for (int x = 0; x < width; x += 2)
                {
                    int firstRowOffset = (y * width) + x;
                    int secondRowOffset = firstRowOffset + width;

                    // Cache the Bayer component values.
                    byte red = rawPixelData[firstRowOffset + 1];
                    byte green1 = rawPixelData[firstRowOffset];
                    byte green2 = rawPixelData[secondRowOffset + 1];
                    byte blue = rawPixelData[secondRowOffset];

                    // Adjust offsets for RGB.
                    firstRowOffset *= 4;
                    secondRowOffset *= 4;

                    // Top left
                    pixelData[firstRowOffset] = blue;
                    pixelData[firstRowOffset + 1] = green1;
                    pixelData[firstRowOffset + 2] = red;

                    // Top right
                    pixelData[firstRowOffset + 4] = blue;
                    pixelData[firstRowOffset + 5] = green1;
                    pixelData[firstRowOffset + 6] = red;

                    // Bottom left
                    pixelData[secondRowOffset] = blue;
                    pixelData[secondRowOffset + 1] = green2;
                    pixelData[secondRowOffset + 2] = red;

                    // Bottom right
                    pixelData[secondRowOffset + 4] = blue;
                    pixelData[secondRowOffset + 5] = green2;
                    pixelData[secondRowOffset + 6] = red;
                }
            }
        }
    }
}
