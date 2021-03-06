﻿using Mach.Kinectomix.LevelEditor.ViewModel;
using Mach.Kinectomix.Logic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mach.Kinectomix.LevelEditor.Behavior
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
            if (_originalTile != null)
            {
                FrameworkElement element = sender as FrameworkElement;

                PaintTile(_originalTile, element.DataContext as BoardTileViewModel, false);

                _originalTile = null;
            }
        }

        private static BoardTileViewModel _originalTile;
        private static void Tile_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_paintTile == null)
                return;

            FrameworkElement element = sender as FrameworkElement;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                PaintTile(_paintTile, element.DataContext as BoardTileViewModel, false);
            }
            else
            {
                // Just preview
                BoardTileViewModel tile = element.DataContext as BoardTileViewModel;

                // Do not preview same drawing
                if (_paintTile.Asset == tile.Asset && _paintTile.AssetSource == tile.AssetSource)
                    return;

                _originalTile = new BoardTileViewModel(new BoardTile());
                _originalTile.Asset = tile.Asset;
                _originalTile.Name = tile.Name;
                _originalTile.AssetSource = tile.AssetSource;
                _originalTile.AssetFile = tile.AssetFile;
                _originalTile.IsFixed = tile.IsFixed;
                _originalTile.IsEmpty = tile.IsEmpty;
                _originalTile.IsClear = tile.IsClear;

                PaintTile(_paintTile, tile, true);
            }
        }

        private static void Tile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            // As user clicked we can remove preview tile.
            _originalTile = null;

            PaintTile(_paintTile, element.DataContext as BoardTileViewModel, false);
        }

        private static void PaintTile(BoardTileViewModel template, BoardTileViewModel selectedTile, bool isPreview)
        {
            if (_paintTile != null && selectedTile != null)
            {
                selectedTile.Asset = template.Asset;
                selectedTile.Name = template.Name;
                selectedTile.AssetSource = template.AssetSource;
                selectedTile.AssetFile = template.AssetFile;
                selectedTile.IsFixed = template.IsFixed;
                selectedTile.IsEmpty = template.IsEmpty;
                selectedTile.IsClear = template.IsClear;
                selectedTile.IsPreview = isPreview;
            }
        }
    }
}
