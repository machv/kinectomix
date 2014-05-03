using Kinectomix.LevelGenerator.ViewModel;
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
                tile.MouseLeave += Tile_MouseLeave;
            }
            else
            {
                tile.MouseDown -= Tile_MouseDown;
                tile.MouseEnter -= Tile_MouseEnter;
                tile.MouseLeave -= Tile_MouseLeave;
            }
        }

        private static void Tile_MouseLeave(object sender, MouseEventArgs e)
        {
            // Remove preview
        }

        private static void Tile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement element = sender as FrameworkElement;

                PaintTile(element.DataContext as BoardTileViewModel);
            }
            else
            {
                // Just preview
            }
        }

        private static void Tile_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            PaintTile(element.DataContext as BoardTileViewModel);
        }

        private static void PaintTile(BoardTileViewModel selectedTile)
        {
            if (_paintTile != null)
            {
                selectedTile.Asset = _paintTile.Asset;
                selectedTile.AssetSource = _paintTile.AssetSource;
                selectedTile.AssetFile = _paintTile.AssetFile;
            }
        }
    }
}
