using Kinectomix.Logic;
using Kinectomix.LevelEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class LevelViewModel : Mvvm.NotifyPropertyBase
    {
        BoardViewModel _board;
        BoardViewModel _molecule;

        public BoardViewModel Board
        {
            get { return _board; }
            set
            {
                _board = value;

                OnPropertyChanged();
            }
        }

        public BoardViewModel Molecule
        {
            get { return _molecule; }
            set
            {
                _molecule = value;

                OnPropertyChanged();
            }
        }

        public static LevelViewModel FromLevel(Level level, Tiles tiles)
        {
            LevelViewModel viewModel = new LevelViewModel();
            viewModel.Board = new BoardViewModel(level.Board, tiles) { EmptyTileTemplate = tiles["Empty"] };
            viewModel.Molecule = new BoardViewModel(level.Molecule, tiles) { EmptyTileTemplate = tiles["Empty"] };

            return viewModel;
        }

        class BuildAsset
        {
            public string AssetName { get; set; }
            public BoardTileViewModel Template { get; set; }
            public bool RenderWithBonds { get; set; }

            public BuildAsset(string asset, BoardTileViewModel tile, bool renderWithBonds)
            {
                AssetName = asset;
                Template = tile;
                RenderWithBonds = renderWithBonds;
            }
        }

        public static Level ToLevel(LevelViewModel levelViewModel, Tiles tiles)
        {
            Dictionary<string, BuildAsset> required = new Dictionary<string, BuildAsset>();

            Level level = new Level();

            level.Board = new TilesCollection<BoardTile>(levelViewModel.Board.Tiles.RowsCount, levelViewModel.Board.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Board.Tiles)
            {
                level.Board.Add(tileViewModel.Tile);

                AddAssetForBuild(required, tileViewModel);
            }

            level.Molecule = new TilesCollection<BoardTile>(levelViewModel.Molecule.Tiles.RowsCount, levelViewModel.Molecule.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Molecule.Tiles)
            {
                level.Molecule.Add(tileViewModel.Tile);

                AddAssetForBuild(required, tileViewModel);
            }

            foreach (KeyValuePair<string, BuildAsset> item in required)
            {
                if (item.Value.Template != null)
                {
                    Size dimensions = new Size(49, 49);
                    int width = 4;

                    DrawingVisual drawingVisual = new DrawingVisual();
                    DrawingContext drawingContext = drawingVisual.RenderOpen();
                    if (item.Value.RenderWithBonds)
                    {
                        drawingContext.DrawBond(dimensions, item.Value.Template.TopBond, BondDirection.Top, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.TopRightBond, BondDirection.TopRight, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.RightBond, BondDirection.Right, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.BottomRightBond, BondDirection.BottomRight, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.BottomBond, BondDirection.Bottom, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.BottomLeftBond, BondDirection.BottomLeft, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.LeftBond, BondDirection.Left, width);
                        drawingContext.DrawBond(dimensions, item.Value.Template.TopLeftBond, BondDirection.TopLeft, width);
                    }
                    drawingContext.DrawImage(item.Value.Template.AssetSource, new Rect(0, 0, dimensions.Width, dimensions.Height));
                    drawingContext.Close();

                    RenderTargetBitmap bmp = new RenderTargetBitmap((int)dimensions.Width, (int)dimensions.Height, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(drawingVisual);

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));

                    byte[] bytes;
                    using (Stream stream = new MemoryStream())
                    {
                        encoder.Save(stream);

                        bytes = new byte[stream.Length];
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Read(bytes, 0, bytes.Length);
                    }

                    LevelAsset levelAsset = new LevelAsset();
                    levelAsset.AssetName = item.Value.AssetName;
                    levelAsset.AssetCode = item.Key;
                    levelAsset.HasBonds = item.Value.RenderWithBonds ? item.Value.Template.Tile.HasBonds : false;
                    levelAsset.IsFixed = item.Value.Template.IsFixed;
                    levelAsset.AssetContent = Convert.ToBase64String(bytes);
                    level.Assets.Add(levelAsset);
                }
            }

            return level;
        }

        private static void AddAssetForBuild(Dictionary<string, BuildAsset> required, BoardTileViewModel tileViewModel)
        {
            if (tileViewModel.IsEmpty)
                return;

            if (!required.ContainsKey(tileViewModel.Tile.Asset))
                required.Add(tileViewModel.Tile.Asset, new BuildAsset(tileViewModel.Tile.Asset, tileViewModel, false));

            if (tileViewModel.HasBonds)
            {
                string key = tileViewModel.GetAssetCode();
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tileViewModel.Tile.Asset, tileViewModel, true));
            }
        }

        public Level ToLevel(Tiles tiles)
        {
            return ToLevel(this, tiles);
        }
    }
}
