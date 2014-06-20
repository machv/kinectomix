using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinectomix.LevelEditor.Visual
{
    class DragAdorner : Adorner
    {
        private Rectangle _child;
        private double _offsetLeft;
        private double _offsetTop;

        /// <summary>
        /// Gets/sets the horizontal offset of the adorner.
        /// </summary>
        public double OffsetLeft
        {
            get { return _offsetLeft; }
            set
            {
                _offsetLeft = value;
                UpdateLocation();
            }
        }
        /// <summary>
        /// Gets/sets the vertical offset of the adorner.
        /// </summary>
        public double OffsetTop
        {
            get { return _offsetTop; }
            set
            {
                _offsetTop = value;
                UpdateLocation();
            }
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Initializes a new instance of DragVisualAdorner.
        /// </summary>
        /// <param name="adornedElement">The element being adorned.</param>
        /// <param name="size">The size of the adorner.</param>
        /// <param name="brush">A brush to with which to paint the adorner.</param>
        public DragAdorner(UIElement adornedElement, Size size, Brush brush)
            : base(adornedElement)
        {
            _child = new Rectangle()
            {
                Fill = brush,
                Width = size.Width,
                Height = size.Height,
                IsHitTestVisible = false
            };
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_offsetLeft, _offsetTop));
            return result;
        }

        /// <summary>
        /// Updates the location of the adorner.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetOffsets(double left, double top)
        {
            _offsetLeft = left;
            _offsetTop = top;
            UpdateLocation();
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            return _child;
        }

        private void UpdateLocation()
        {
            AdornerLayer adornerLayer = Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(AdornedElement);
        }
    }
}
