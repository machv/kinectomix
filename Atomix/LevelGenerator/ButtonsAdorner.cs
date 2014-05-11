using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    public class ButtonsAdorner : Adorner
    {
        private VisualCollection _visualChildren;
        private FrameworkElement _content;

        public ButtonsAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            var panel = new StackPanel();
            var btn = new Button() { Content = "ahoj" };
            btn.Click += Btn_Click;
            panel.Children.Add(btn);

            _content = panel;
            _visualChildren.Add(_content);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChildren[index];
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChildren.Count; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _content.Measure(constraint);
            return _content.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _content.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return _content.RenderSize;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hit");
        }
    }
}
