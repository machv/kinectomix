using AtomixData;
using Kinectomix.LevelGenerator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class EditorViewModel : Mvvm.NotifyPropertyBase
    {
        private Tiles _tiles;

        private IEnumerable<BoardTile> _availableTiles;
        public IEnumerable<BoardTile> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;

                RaisePropertyChangedEvent("AvailableTiles");
            }
        }

        private BoardTile _currentTileTemplate;
        public BoardTile CurrentTileTemplate
        {
            get { return _currentTileTemplate; }
            set
            {
                _currentTileTemplate = value;

                RaisePropertyChangedEvent("CurrentTileTemplate");
            }
        }

        public EditorViewModel()
        {
            _tiles = new Tiles();

            _availableTiles = _tiles.Board;
        }
    }
}
