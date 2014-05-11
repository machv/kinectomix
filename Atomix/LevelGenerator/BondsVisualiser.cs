using AtomixData;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    public class BondsVisualiser : FrameworkElement
    {
        public static readonly DependencyProperty TopLeftBondProperty = DependencyProperty.Register("TopLeftBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TopBondProperty = DependencyProperty.Register("TopBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TopRightBondProperty = DependencyProperty.Register("TopRightBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RightBondProperty = DependencyProperty.Register("RightBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomRightBondProperty = DependencyProperty.Register("BottomRightBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomBondProperty = DependencyProperty.Register("BottomBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BottomLeftBondProperty = DependencyProperty.Register("BottomLeftBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty LeftBondProperty = DependencyProperty.Register("LeftBond", typeof(BondType), typeof(BondsVisualiser), new FrameworkPropertyMetadata(BondType.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public BondType TopLeftBond
        {
            get { return (BondType)GetValue(TopLeftBondProperty); }
            set { SetValue(TopLeftBondProperty, value); }
        }
        public BondType TopBond
        {
            get { return (BondType)GetValue(TopBondProperty); }
            set { SetValue(TopBondProperty, value); }
        }
        public BondType TopRightBond
        {
            get { return (BondType)GetValue(TopRightBondProperty); }
            set { SetValue(TopRightBondProperty, value); }
        }
        public BondType RightBond
        {
            get { return (BondType)GetValue(RightBondProperty); }
            set { SetValue(RightBondProperty, value); }
        }
        public BondType BottomRightBond
        {
            get { return (BondType)GetValue(BottomRightBondProperty); }
            set { SetValue(BottomRightBondProperty, value); }
        }
        public BondType BottomBond
        {
            get { return (BondType)GetValue(BottomBondProperty); }
            set { SetValue(BottomBondProperty, value); }
        }
        public BondType BottomLeftBond
        {
            get { return (BondType)GetValue(BottomLeftBondProperty); }
            set { SetValue(BottomLeftBondProperty, value); }
        }
        public BondType LeftBond
        {
            get { return (BondType)GetValue(LeftBondProperty); }
            set { SetValue(LeftBondProperty, value); }
        }

        static BondsVisualiser()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(BondsVisualiser), new PropertyMetadata(true));
        }

        private void RenderBond(DrawingContext drawingContext, int arity, int angle)
        {
            if (arity > 0)
            {
                drawingContext.PushTransform(new RotateTransform(angle, RenderSize.Width / 2, RenderSize.Height / 2));

                double penWidth = 2;
                double gap = 2;
                Pen pen = new Pen(new SolidColorBrush(Colors.DarkGray), penWidth);
                double rel = RenderSize.Width - RenderSize.Height * RenderSize.Width;
                double centerY = RenderSize.Height / 2;
                double width = arity * penWidth + (arity - 1) * gap;
                double start = RenderSize.Width / 2 - (width / 2);

                for (int i = 0; i < arity; i++)
                {
                    Point point1 = new Point(start, rel);
                    Point point2 = new Point(start, centerY);
                    if (angle > 90)
                    {
                        point1.X += penWidth;
                        point2.X += penWidth;
                    }
                    drawingContext.DrawLine(pen, point1, point2);

                    start += penWidth + gap;
                }

                drawingContext.Pop();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            RenderBond(drawingContext, (int)TopBond, 0);
            RenderBond(drawingContext, (int)TopRightBond, 45);
            RenderBond(drawingContext, (int)RightBond, 90);
            RenderBond(drawingContext, (int)BottomRightBond, 135);
            RenderBond(drawingContext, (int)BottomBond, 180);
            RenderBond(drawingContext, (int)BottomLeftBond, 225);
            RenderBond(drawingContext, (int)LeftBond, 270);

            base.OnRender(drawingContext);
        }
    }
}
