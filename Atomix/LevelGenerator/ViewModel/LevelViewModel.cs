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
            public BoardTileViewModel BondsTemplate { get; set; }

            public BuildAsset(string asset)
            {
                AssetName = asset;
            }

            public BuildAsset(string asset, BoardTileViewModel tile)
            {
                AssetName = asset;
                BondsTemplate = tile;
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

                if (!required.ContainsKey(tileViewModel.Tile.Asset))
                    required.Add(tileViewModel.Tile.Asset, new BuildAsset(tileViewModel.Tile.Asset));

                string key = tileViewModel.GetAssetCode();
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tileViewModel.Tile.Asset, tileViewModel));
            }

            level.Molecule = new TilesCollection<BoardTile>(levelViewModel.Molecule.Tiles.RowsCount, levelViewModel.Molecule.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Molecule.Tiles)
            {
                level.Molecule.Add(tileViewModel.Tile);

                if (!required.ContainsKey(tileViewModel.Tile.Asset))
                    required.Add(tileViewModel.Tile.Asset, new BuildAsset(tileViewModel.Tile.Asset));

                string key = tileViewModel.GetAssetCode();
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tileViewModel.Tile.Asset, tileViewModel));
            }

            foreach (KeyValuePair<string, BuildAsset> item in required)
            {
                BoardTileViewModel tile = tiles[item.Key];
                if (tile != null && !string.IsNullOrEmpty(tile.AssetFile))
                {
                    LevelAsset levelAsset = new LevelAsset();
                    levelAsset.AssetName = item.Value.AssetName;
                    levelAsset.HasBonds = false;
                    levelAsset.AssetContent = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(tile.AssetFile));
                    level.Assets.Add(levelAsset);

                    //TODO: Zkontrolovat poměr stran a velikost assetu
                }

                if (item.Value.BondsTemplate != null)
                {
                    Size dimensions = new Size(49, 49);

                    DrawingVisual drawingVisual = new DrawingVisual();
                    DrawingContext drawingContext = drawingVisual.RenderOpen();
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.TopBond, BondDirection.Top);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.TopRightBond, BondDirection.TopRight);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.RightBond, BondDirection.Right);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.BottomRightBond, BondDirection.BottomRight);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.BottomBond, BondDirection.Bottom);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.BottomLeftBond, BondDirection.BottomLeft);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.LeftBond, BondDirection.Left);
                    drawingContext.DrawBond(dimensions, item.Value.BondsTemplate.TopLeftBond, BondDirection.TopLeft);
                    drawingContext.DrawImage(item.Value.BondsTemplate.AssetSource, new Rect(0, 0, dimensions.Width, dimensions.Height));
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
                    levelAsset.HasBonds = item.Value.BondsTemplate.Tile.HasBonds;
                    levelAsset.AssetContent = Convert.ToBase64String(bytes);
                    level.Assets.Add(levelAsset);
                }
            }

            return level;
        }

        public Level ToLevel(Tiles tiles)
        {
            return ToLevel(this, tiles);
        }
    }
}
