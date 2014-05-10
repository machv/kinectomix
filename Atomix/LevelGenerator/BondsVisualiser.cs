using AtomixData;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    public class BondsVisualiser : FrameworkElement
    {
        public static readonly DependencyProperty TopLeftBondProperty = DependencyProperty.Register("TopLeftBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TopBondProperty = DependencyProperty.Register("TopBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TopRightBondProperty = DependencyProperty.Register("TopRightBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RightBondProperty = DependencyProperty.Register("RightBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomRightBondProperty = DependencyProperty.Register("BottomRightBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomBondProperty = DependencyProperty.Register("BottomBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomLeftBondProperty = DependencyProperty.Register("BottomLeftBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty LeftBondProperty = DependencyProperty.Register("LeftBond", typeof(BondArity), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondArity.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public BondArity TopBond
        {
            get { return (BondArity)GetValue(TopBondProperty); }
            set { SetValue(TopBondProperty, value); }
        }


        public BondArity TopRightBond
        {
            get { return (BondArity)GetValue(TopRightBondProperty); }
            set { SetValue(TopRightBondProperty, value); }
        }

        static BondsVisualiser()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(BondsVisualiser), new PropertyMetadata(true));
        }

        private void RenderBond(DrawingContext drawingContext, int arity, int angle)
        {
            if (arity > 0)
            {
                int penWidth = 2;
                int gap = 2;
                Pen pen = new Pen(new SolidColorBrush(Colors.Black), penWidth);
                Point center = new Point(ActualWidth / 2, ActualHeight / 2);
                double rel = ActualWidth - Math.Sqrt(ActualWidth * ActualWidth + ActualHeight * ActualHeight);
                double centerY = ActualHeight / 2;
                double width = arity * penWidth + (arity - 1) * gap;
                double start = ActualWidth / 2 - (width / 2);

                for (int i = 0; i < arity; i++)
                {
                    Point point1 = RotatePoint(new Point(start, rel), center, angle);
                    Point point2 = RotatePoint(new Point(start, centerY), center, angle);
                    drawingContext.DrawLine(pen, point1, point2);

                    start += penWidth + gap;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //drawingContext.PushClip(new RectangleGeometry(new Rect(new Size(ActualWidth, ActualHeight))));

            

            RenderBond(drawingContext, (int)TopBond, 0);

            //drawingContext.PushTransform(new RotateTransform(45));
            RenderBond(drawingContext, (int)TopRightBond, 45);
            //drawingContext.Pop();

            //drawingContext.Pop();

            base.OnRender(drawingContext);
        }

        static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double radians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(radians);
            double sinTheta = Math.Sin(radians);
            return new Point
            {
                X = (int)(cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (int)(sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
