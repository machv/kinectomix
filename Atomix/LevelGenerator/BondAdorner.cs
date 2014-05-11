using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    // Adorners must subclass the abstract base class Adorner. 
    public class BondsAdorner : Adorner
    {
        List<FrameworkElement> _children;

        // Be sure to call the base class constructor. 
        public BondsAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
            _children = new List<FrameworkElement>();

            _children.Add(new Button());
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender 
        // method, which is called by the layout system as part of a rendering pass. 
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Transparent);
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1);
            double renderRadius = 4.0;

            //var btn = new Button();
            //btn.rend

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, new Point(adornedElementRect.TopLeft.X + 5, adornedElementRect.TopLeft.Y + 5), renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }
    }
}
