using AtomixData;
using Kinectomix.LevelGenerator.Model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class LevelViewModel : Mvvm.NotifyPropertyBase
    {
        BoardViewModel _board = new BoardViewModel();
        BoardViewModel _molecule = new BoardViewModel();

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
            viewModel.Board = new BoardViewModel(level.Board, tiles);
            viewModel.Molecule = new BoardViewModel(level.Molecule, tiles);

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

            public static string GetTileAssetWithBondsName(BoardTileViewModel tile)
            {
                return string.Format("{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}_{0}", tile.Asset,
                    (int)tile.TopLeftBond,
                    (int)tile.TopBond,
                    (int)tile.TopRightBond,
                    (int)tile.RightBond,
                    (int)tile.BottomRightBond,
                    (int)tile.BottomBond,
                    (int)tile.BottomLeftBond,
                    (int)tile.LeftBond);
            }
        }

        public static Level ToLevel(LevelViewModel levelViewModel, Tiles tiles)
        {
            Dictionary<string, BuildAsset> required = new Dictionary<string, BuildAsset>();
            //List<string> requiredAssets = new List<string>();

            Level level = new Level();

            level.Board = new TilesCollection<BoardTile>(levelViewModel.Board.Tiles.RowsCount, levelViewModel.Board.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Board.Tiles)
            {
                BoardTile tile = new BoardTile() { IsFixed = tileViewModel.IsFixed, Asset = tileViewModel.Asset };
                level.Board.Add(tile);

                //if (!requiredAssets.Contains(tile.Asset))
                //    requiredAssets.Add(tile.Asset);

                if (!required.ContainsKey(tile.Asset))
                    required.Add(tile.Asset, new BuildAsset(tile.Asset));

                string key = BuildAsset.GetTileAssetWithBondsName(tileViewModel);
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tile.Asset, tileViewModel));
            }

            level.Molecule = new TilesCollection<BoardTile>(levelViewModel.Molecule.Tiles.RowsCount, levelViewModel.Molecule.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Molecule.Tiles)
            {
                BoardTile tile = new BoardTile() { IsFixed = tileViewModel.IsFixed, Asset = tileViewModel.Asset };
                level.Molecule.Add(tile);

                if (!required.ContainsKey(tile.Asset))
                    required.Add(tile.Asset, new BuildAsset(tile.Asset));

                string key = BuildAsset.GetTileAssetWithBondsName(tileViewModel);
                if (!required.ContainsKey(key))
                    required.Add(key, new BuildAsset(tile.Asset, tileViewModel));
            }

            foreach (KeyValuePair<string, BuildAsset> item in required)
            {
                BoardTileViewModel tile = tiles[item.Value.AssetName];
                if (!string.IsNullOrEmpty(tile.AssetFile))
                {
                    LevelAsset levelAsset = new LevelAsset();
                    levelAsset.AssetName = item.Value.AssetName;
                    levelAsset.AssetContent = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(tile.AssetFile));
                    level.Assets.Add(levelAsset);

                    //TODO: Zkontrolovat poměr stran a velikost assetu
                }

                if (item.Value.BondsTemplate != null)
                {
                    DrawingVisual drawingVisual = new DrawingVisual();
                    DrawingContext drawingContext = drawingVisual.RenderOpen();
                    DrawBond(drawingContext, item.Value.BondsTemplate.TopBond, 0);
                    drawingContext.DrawImage(item.Value.BondsTemplate.AssetSource, new Rect(0, 0, 49, 49));
                    drawingContext.Close();

                    RenderTargetBitmap bmp = new RenderTargetBitmap(49, 49, 96, 96, PixelFormats.Pbgra32);
                    bmp.Render(drawingVisual);

                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));

                    using (Stream outputStream = File.Open(@"D:\TEMP\test.png", System.IO.FileMode.OpenOrCreate))
                        encoder.Save(outputStream);

                }
            }

            return level;
        }

        private static Size RenderSize = new Size(49, 49);

        private static void DrawBond(DrawingContext drawingContext, BondType type, int angle)
        {
            int arity = (int)type;
            if (arity > 0)
            {
                drawingContext.PushTransform(new RotateTransform(angle, RenderSize.Width / 2, RenderSize.Height / 2));

                double penWidth = 2;
                double gap = 2;
                Pen pen = new Pen(new SolidColorBrush(Colors.DarkGray), penWidth);
                double rel = RenderSize.Width - RenderSize.Height * RenderSize.Width;
                double centerY = RenderSize.Height / 2;
                double width = arity * penWidth + (arity - 1) * gap;
                double start = RenderSize.Width / 2 - (width / 2);

                for (int i = 0; i < arity; i++)
                {
                    Point point1 = new Point(start, rel);
                    Point point2 = new Point(start, centerY);
                    if (angle > 90)
                    {
                        point1.X += penWidth;
                        point2.X += penWidth;
                    }
                    drawingContext.DrawLine(pen, point1, point2);

                    start += penWidth + gap;
                }

                drawingContext.Pop();
            }
        }

        public Level ToLevel(Tiles tiles)
        {
            return ToLevel(this, tiles);
        }
    }
}
