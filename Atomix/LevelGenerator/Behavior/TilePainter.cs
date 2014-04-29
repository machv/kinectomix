using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kinectomix.LevelGenerator.Behavior
{
    public class TilePainter
    {
        public static readonly DependencyProperty IsPaintEnabledProperty = DependencyProperty.RegisterAttached("IsPaintEnabled", typeof(bool), typeof(TilePainter), new UIPropertyMetadata(false, OnSetIsPaintEnabledChanged));

        public static bool GetIsPaintEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPaintEnabledProperty);
        }

        public static void SetIsPaintEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPaintEnabledProperty, value);
        }

        private static void OnSetIsPaintEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Image tile = sender as Image;
            bool isEnabled = (bool)(e.NewValue);

            if (isEnabled)
            {
                tile.MouseDown += Tile_MouseDown;
                tile.MouseEnter += Tile_MouseEnter;
            }
            else
            {
                tile.MouseDown -= Tile_MouseDown;
                tile.MouseEnter -= Tile_MouseEnter;
            }
        }

        private static void Tile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private static void Tile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("down");
        }
    }
}
