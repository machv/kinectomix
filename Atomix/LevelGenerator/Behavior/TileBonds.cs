using System;
using System.Windows;
using System.Windows.Documents;

namespace Kinectomix.LevelGenerator.Behavior
{
    public class TileBonds
    {
        public static readonly DependencyProperty IsBondingEnabledProperty = DependencyProperty.RegisterAttached("IsBondingEnabled", typeof(bool), typeof(TileBonds), new PropertyMetadata(false, OnIsBondingEnabledChanged));

        public static bool GetIsBondingEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBondingEnabledProperty);
        }
        public static void SetIsBondingEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBondingEnabledProperty, value);
        }

        private static void OnIsBondingEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                element.MouseEnter += Element_MouseEnter;
                element.MouseLeave += Element_MouseLeave;
            }
            else
            {
                element.MouseEnter -= Element_MouseEnter;
                element.MouseLeave -= Element_MouseLeave;
            }
        }

        private static void AddBondAdorner(FrameworkElement element)
        {
            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(element);
            parentAdorner.Add(new ButtonsAdorner(element));
        }

        private static FrameworkElement _element;
        private static void RemoveBondAdorners(FrameworkElement element)
        {
            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(element);

            Adorner[] toRemoveArray = parentAdorner.GetAdorners(element);
            if (toRemoveArray != null)
            {
                for (int x = 0; x < toRemoveArray.Length; x++)
                {
                    if (toRemoveArray[x] is ButtonsAdorner)
                    {
                        if (toRemoveArray[x].IsMouseOver)
                        {
                            _element = element;
                            toRemoveArray[x].MouseLeave += UIElement_MouseLeave;
                        }
                        else
                        {
                            parentAdorner.Remove(toRemoveArray[x]);
                        }
                    }
                }
            }
        }

        private static void UIElement_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(_element != null)
                RemoveBondAdorners(_element);
        }

        private static void Element_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AddBondAdorner(sender as FrameworkElement);
        }

        private static void Element_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RemoveBondAdorners(sender as FrameworkElement);
        }
    }
}
