using Kinectomix.LevelEditor.Model;
using System.Collections.Generic;
using Kinectomix.Wpf.Mvvm;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class AvailableTilesViewModel : NotifyPropertyBase
    {
        private Tiles _tiles;

        public Tiles Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                OnPropertyChanged();
            }
        }

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
            if (_tiles == null)
            {
                AvailableTiles = null;
                return;
            }

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

        public AvailableTilesViewModel()
        {

        }

        public AvailableTilesViewModel(Tiles tiles)
        {
            _tiles = tiles;
        }
    }
}
