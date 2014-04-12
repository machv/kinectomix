﻿using AtomixData;
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

                RaisePropertyChangedEvent("Board");
            }
        }

        public MoleculeViewModel Molecule
        {
            get { return _molecule; }
            set
            {
                _molecule = value;

                RaisePropertyChangedEvent("Molecule");
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
    }
}
