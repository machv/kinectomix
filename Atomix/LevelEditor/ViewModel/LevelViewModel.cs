using Kinectomix.Logic;
using Kinectomix.LevelEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kinectomix.Wpf.Mvvm;

namespace Kinectomix.LevelEditor.ViewModel
{
    /// <summary>
    /// View Model for the definition of one game level.
    /// </summary>
    public partial class LevelViewModel : NotifyPropertyBase
    {
        private BoardViewModel _board;
        private BoardViewModel _molecule;
        private Tiles _tiles;
        private string _name;

        /// <summary>
        /// Gets or sets <see cref="Tiles"/> for use in the level.
        /// </summary>
        /// <returns>Tiles for use in the level.</returns>
        public Tiles Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets <see cref="BoardViewModel"/> containing game tiles.
        /// </summary>
        /// <returns>Game tiles collection view model.</returns>
        public BoardViewModel Board
        {
            get { return _board; }
            set
            {
                _board = value;

                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets <see cref="BoardViewModel"/> containing tiles describing molecule definition.
        /// </summary>
        /// <returns>Molecule definition view model.</returns>
        public BoardViewModel Molecule
        {
            get { return _molecule; }
            set
            {
                _molecule = value;

                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets friendly name of the level.
        /// </summary>
        /// <returns>Friendly name of the level.</returns>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public static LevelViewModel FromLevel(Level level, Tiles tiles)
        {
            LevelViewModel viewModel = new LevelViewModel();
            viewModel.Tiles = tiles;
            viewModel.Board = new BoardViewModel(level.Board, tiles) { EmptyTileTemplate = tiles["Empty"] };
            viewModel.Molecule = new BoardViewModel(level.Molecule, tiles) { EmptyTileTemplate = tiles["Empty"] };

            return viewModel;
        }

        public static Level ToLevel(LevelViewModel levelViewModel, Tiles tiles)
        {
            Dictionary<string, BuildAsset> required = new Dictionary<string, BuildAsset>();

            Level level = new Level();

            level.Board = new TilesCollection<BoardTile>(levelViewModel.Board.Tiles.RowsCount, levelViewModel.Board.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Board.Tiles)
            {
                if (tileViewModel.IsClear)
                {
                    level.Board.Add(null);

                    continue;
                }

                string code = tileViewModel.AssetSource.GetHashCode().ToString();
                if (!tileViewModel.IsEmpty)
                    tileViewModel.Asset = code;

                level.Board.Add(tileViewModel.Tile);

                AddAssetForBuild(required, code, tileViewModel);
            }

            level.Molecule = new TilesCollection<BoardTile>(levelViewModel.Molecule.Tiles.RowsCount, levelViewModel.Molecule.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Molecule.Tiles)
            {
                if (tileViewModel.IsClear)
                {
                    level.Molecule.Add(null);

                    continue;
                }

                string code = tileViewModel.AssetSource.GetHashCode().ToString();
                if (!tileViewModel.IsEmpty)
                    tileViewModel.Asset = code;

                level.Molecule.Add(tileViewModel.Tile);

                AddAssetForBuild(required, code, tileViewModel);
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
                    levelAsset.Name = item.Value.AssetName;
                    levelAsset.Code = item.Key;
                    levelAsset.HasBonds = item.Value.RenderWithBonds ? item.Value.Template.Tile.HasBonds : false;
                    levelAsset.IsFixed = item.Value.Template.IsFixed;
                    levelAsset.AssetContent = Convert.ToBase64String(bytes);
                    level.Assets.Add(levelAsset);
                }
            }

            return level;
        }

        private static void AddAssetForBuild(Dictionary<string, BuildAsset> required, string code, BoardTileViewModel tileViewModel)
        {
            if (tileViewModel.IsEmpty)
                return;

            if (!required.ContainsKey(code))
                required.Add(code, new BuildAsset(tileViewModel.Tile.Name, tileViewModel, false));

            if (tileViewModel.HasBonds)
            {
                string key = tileViewModel.GetAssetCode(code);
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tileViewModel.Tile.Name, tileViewModel, true));
            }
        }

        public Level ToLevel(Tiles tiles)
        {
            return ToLevel(this, tiles);
        }
    }
}
