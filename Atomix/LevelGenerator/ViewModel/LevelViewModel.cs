using AtomixData;
using Kinectomix.LevelGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Level ToLevel(LevelViewModel levelViewModel, Tiles tiles)
        {
            List<string> requiredAssets = new List<string>();

            Level level = new Level();

            level.Board = new TilesCollection<BoardTile>(levelViewModel.Board.Tiles.RowsCount, levelViewModel.Board.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Board.Tiles)
            {
                BoardTile tile = new BoardTile() { IsFixed = tileViewModel.IsFixed, Asset = tileViewModel.Asset };
                level.Board.Add(tile);

                if (!requiredAssets.Contains(tile.Asset))
                    requiredAssets.Add(tile.Asset);
            }

            level.Molecule = new TilesCollection<BoardTile>(levelViewModel.Molecule.Tiles.RowsCount, levelViewModel.Molecule.Tiles.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Molecule.Tiles)
            {
                BoardTile tile = new BoardTile() { IsFixed = tileViewModel.IsFixed, Asset = tileViewModel.Asset };
                level.Molecule.Add(tile);

                if (!requiredAssets.Contains(tile.Asset))
                    requiredAssets.Add(tile.Asset);
            }

            foreach (string asset in requiredAssets)
            {
                BoardTileViewModel tile = tiles[asset];
                if (!string.IsNullOrEmpty(tile.AssetFile))
                {
                    LevelAsset levelAsset = new LevelAsset();
                    levelAsset.AssetName = asset;
                    levelAsset.AssetContent = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(tile.AssetFile));
                    level.Assets.Add(levelAsset);

                    //TODO: Zkontrolovat poměr stran a velikost assetu
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
