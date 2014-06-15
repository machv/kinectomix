using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Circle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private BitmapImage _bitmapImage;
        public BitmapImage Output
        {
            get { return _bitmapImage; }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged();
            }
        }

        private static PathGeometry GetClockGeometry(Point position, double percentage, double radius)
        {
            const double innerFactor = 0.70d;
            double innerRadius = radius * innerFactor;

            PathGeometry pie = new PathGeometry();

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, -innerRadius);
            pathFigure.IsClosed = true;

            if (percentage >= 1)
            {
                percentage = 0.99999999d;
            }
            double angle = 360.0 * percentage;

            // Starting Point
            LineSegment inOutLine = new LineSegment(new Point(0, -radius), true);

            // Arc
            ArcSegment outerArc = new ArcSegment();

            outerArc.IsLargeArc = angle >= 180.0;
            outerArc.Point = new Point(Math.Cos((angle - 90) * Math.PI / 180.0) * radius, Math.Sin((angle - 90) * Math.PI / 180.0) * radius);
            outerArc.Size = new Size(radius, radius);
            outerArc.SweepDirection = SweepDirection.Clockwise;

            LineSegment outInLine = new LineSegment(new Point(outerArc.Point.X * innerFactor, outerArc.Point.Y * innerFactor), true);

            ArcSegment innerArc = new ArcSegment();
            innerArc.IsLargeArc = angle >= 180.0;
            innerArc.Point = pathFigure.StartPoint;
            innerArc.Size = new Size(innerRadius, innerRadius);
            innerArc.SweepDirection = SweepDirection.Counterclockwise;

            pathFigure.Segments.Add(inOutLine);
            pathFigure.Segments.Add(outerArc);
            pathFigure.Segments.Add(outInLine);
            pathFigure.Segments.Add(innerArc);

            pie.Transform = new TranslateTransform(position.X, position.Y);
            pie.Figures.Add(pathFigure);

            return pie;
        }

        private BitmapFrame CreateCircleFrame(double percentage)
        {
            double radius = hand.Height * 0.9d;

            Point center = new Point(xTranslation + (hand.Width + offset) / 2, (hand.Height + offset) / 2);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //drawingContext.DrawRectangle(Brushes.CornflowerBlue, null, new Rect(0, 0, hand.Width + offset + xTranslation, hand.Height + offset));
            //drawingContext.DrawImage(hand, new Rect(offset / 2, offset / 2, hand.Width, hand.Height));

            var fullcircle = GetClockGeometry(center, 1, radius);
            drawingContext.DrawGeometry(new SolidColorBrush(Color.FromRgb(92, 92, 92)), null, fullcircle);

            var geoc = GetClockGeometry(center, percentage, radius);
            drawingContext.DrawGeometry(Brushes.Red, null, geoc);

            drawingContext.Close();


            RenderTargetBitmap bmp = new RenderTargetBitmap((int)hand.Width + offset + xTranslation, (int)hand.Height + offset, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            return BitmapFrame.Create(bmp);
        }

        ImageSource hand;
        int frameWidth;
        int frameHeight;
        int offset = 90;
        double step = 0.06d;
        int xTranslation = 3;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageSourceConverter SourceConverter = new ImageSourceConverter();
            hand = (ImageSource)(SourceConverter.ConvertFromString(@"D:\Documents\Workspaces\TFS15\atomix\Atomix\Game\AtomixContent\Images\Hand.png"));


            Point center = new Point(xTranslation + (hand.Width + offset) / 2, (hand.Height + offset) / 2);
            frameWidth = xTranslation + (int)hand.Width + offset;
            frameHeight = (int)hand.Height + offset;

            int frames = 0;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                BitmapFrame frame;
                int startX = 0;
                double perc = 0;
                while (perc < 1)
                {
                    frame = CreateCircleFrame(perc);
                    drawingContext.DrawImage(frame, new Rect(startX, 0, frameWidth, frameHeight));
                    startX += frameWidth;
                    frames++;
                    perc += step;
                }

                // final percentage
                frame = CreateCircleFrame(1);
                drawingContext.DrawImage(frame, new Rect(startX, 0, frameWidth, frameHeight));
                startX += frameWidth;
                frames++;
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(frameWidth * frames, frameHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            // Saves the image into a file using the encoder
            using (Stream stream = File.Create(@"d:\temp\HandCircle.png"))
                encoder.Save(stream);


            // Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
            PngBitmapEncoder encoder1 = new PngBitmapEncoder();
            encoder1.Frames.Add(BitmapFrame.Create(bmp));
            var bitmapImage = new BitmapImage();
            using (Stream stream = new MemoryStream())
            {
                encoder1.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            Output = bitmapImage;
        }
    }
}
