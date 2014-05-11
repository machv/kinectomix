using System.Windows;
using System.Windows.Controls;

namespace Kinectomix.LevelGenerator
{
    public class ButtonsAdorner : FrameworkElementAdorner
    {
        public ButtonsAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            var panel = new StackPanel();
            var btn = new Button() { Content = "ahoj" };
            btn.Click += Btn_Click;
            panel.Children.Add(btn);

            Child = panel;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hit");
        }
    }
}
