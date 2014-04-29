using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.Behavior
{
    public class TilePainter
    {
        private static BoardTileViewModel _paintTile;

        public static readonly DependencyProperty PaintTileProperty = DependencyProperty.RegisterAttached("PaintTile", typeof(BoardTileViewModel), typeof(TilePainter), new PropertyMetadata(null, OnPaintTileChanged));

        public static BoardTileViewModel GetPaintTile(DependencyObject obj)
        {
            return (BoardTileViewModel)obj.GetValue(PaintTileProperty);
        }
        public static void SetPaintTile(DependencyObject obj, BoardTileViewModel value)
        {
            obj.SetValue(PaintTileProperty, value);
        }

        private static void OnPaintTileChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _paintTile = e.NewValue as BoardTileViewModel;
        }

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
            if (e.LeftButton == MouseButtonState.Pressed)
            {

            }
        }

        private static void Tile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            var selectedTile = element.DataContext as BoardTileViewModel;

            if (_paintTile != null)
            {
                selectedTile.Asset = _paintTile.Asset;
                selectedTile.AssetSource = _paintTile.AssetSource;
                selectedTile.AssetFile = _paintTile.AssetFile;
            }
        }
    }
}
