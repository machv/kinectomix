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

namespace Kinectomix.LevelGenerator
{
    public class BondsVisualiser : Control
    {
        static BondsVisualiser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BondsVisualiser), new FrameworkPropertyMetadata(typeof(BondsVisualiser)));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //Line line = new Line();
            //Thickness thickness = new Thickness(101, -11, 362, 250);
            //line.Margin = thickness;
            //line.Visibility = System.Windows.Visibility.Visible;
            //line.StrokeThickness = 4;
            //line.Stroke = System.Windows.Media.Brushes.Black;
            //line.X1 = 10;
            //line.X2 = 40;
            //line.Y1 = 70;
            //line.Y2 = 70;

            Pen LinePen = new Pen(new SolidColorBrush(Colors.Green), 3.0d);

            drawingContext.DrawLine(LinePen, new Point(0, 0), new Point(20, 20));

            base.OnRender(drawingContext);
        }
    }
}
