using Kinectomix.Logic;
using Kinectomix.LevelGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class AvailableTilesViewModel : Mvvm.NotifyPropertyBase
    {
        private Tiles _tiles;

        public delegate void TileSelectedEventHandler(object sender, TileSelectedEventArgs e);
        public event TileSelectedEventHandler TileSelected;

        protected void OnTileSelected(BoardTileViewModel tile)
        {
            if (TileSelected != null)
                TileSelected(this, new TileSelectedEventArgs(tile));
        }

        private IEnumerable<BoardTileViewModel> _availableTiles;
        public IEnumerable<BoardTileViewModel> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;

                OnPropertyChanged();
            }
        }

        private BoardTileViewModel _selectedTile;
        public BoardTileViewModel SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                _selectedTile = value;

                OnTileSelected(_selectedTile);
                OnPropertyChanged();
            }
        }

        public void UpdateAvailableTiles(Tiles.TileType type)
        {
            switch (type)
            {
                case Tiles.TileType.Board:
                    AvailableTiles = _tiles.Board;
                    break;
                case Tiles.TileType.Molecule:
                    AvailableTiles = _tiles.Molecule;
                    break;
            }
        }

        public AvailableTilesViewModel(Tiles tiles)
        {
            _tiles = tiles;
        }
    }
}
