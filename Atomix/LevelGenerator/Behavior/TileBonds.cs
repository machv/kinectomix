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
            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(element);

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                parentAdorner.Add(new BondAdorner(element));
            }
            else {
                Adorner[] toRemoveArray = parentAdorner.GetAdorners(element);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                        parentAdorner.Remove(toRemoveArray[x]);
                }
            }
        }
    }
}
