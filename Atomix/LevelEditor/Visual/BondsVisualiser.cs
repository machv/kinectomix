using Mach.Kinectomix.Logic;
using System.Windows;
using System.Windows.Media;

namespace Kinectomix.LevelEditor
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawBond(RenderSize, TopBond, BondDirection.Top);
            drawingContext.DrawBond(RenderSize, TopRightBond, BondDirection.TopRight);
            drawingContext.DrawBond(RenderSize, RightBond, BondDirection.Right);
            drawingContext.DrawBond(RenderSize, BottomRightBond, BondDirection.BottomRight);
            drawingContext.DrawBond(RenderSize, BottomBond, BondDirection.Bottom);
            drawingContext.DrawBond(RenderSize, BottomLeftBond, BondDirection.BottomLeft);
            drawingContext.DrawBond(RenderSize, LeftBond, BondDirection.Left);
            drawingContext.DrawBond(RenderSize, TopLeftBond, BondDirection.TopLeft);

            base.OnRender(drawingContext);
        }
    }
}
