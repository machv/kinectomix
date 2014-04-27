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
        MoleculeViewModel _molecule = new MoleculeViewModel();

        public BoardViewModel Board
        {
            get { return _board; }
            set
            {
                _board = value;

                RaisePropertyChangedEvent();
            }
        }

        public MoleculeViewModel Molecule
        {
            get { return _molecule; }
            set
            {
                _molecule = value;

                RaisePropertyChangedEvent();
            }
        }

        public static LevelViewModel FromLevel(Level level)
        {
            LevelViewModel viewModel = new LevelViewModel();

            foreach (BoardTile tile in level.Board)
            {
                BoardTileViewModel tileViewModel = new BoardTileViewModel(tile);
                viewModel.Board.Tiles.Add(tileViewModel);
            }
            viewModel.Board.ColumnsCount = level.Board.ColumnsCount;
            viewModel.Board.RowsCount = level.Board.RowsCount;

            foreach (BoardTile atom in level.Molecule)
            {
                BoardTileViewModel atomViewModel = new BoardTileViewModel(atom);
                viewModel.Molecule.Tiles.Add(atomViewModel);
            }
            viewModel.Molecule.ColumnsCount = level.Molecule.ColumnsCount;
            viewModel.Molecule.RowsCount = level.Molecule.RowsCount;

            return viewModel;
        }

        public static Level ToLevel(LevelViewModel levelViewModel, Tiles tiles)
        {
            List<string> requiredAssets = new List<string>();

            Level level = new Level();

            level.Board = new BoardCollection<BoardTile>(levelViewModel.Board.RowsCount, levelViewModel.Board.ColumnsCount);
            foreach (BoardTileViewModel tileViewModel in levelViewModel.Board.Tiles)
            {
                BoardTile tile = new BoardTile() { IsFixed = tileViewModel.IsFixed, Asset = tileViewModel.Asset };
                level.Board.Add(tile);

                if (!requiredAssets.Contains(tile.Asset))
                    requiredAssets.Add(tile.Asset);
            }

            level.Molecule = new BoardCollection<BoardTile>(levelViewModel.Molecule.RowsCount, levelViewModel.Molecule.ColumnsCount);
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
