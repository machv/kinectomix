using AtomixData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    public class BoardTileViewModel : Mvvm.NotifyPropertyBase
    {
        BoardTile _tile;

        public BoardTile Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public string Asset
        {
            get { return _tile.Asset; }
            set
            {
                _tile.Asset = value;
                RaisePropertyChangedEvent();
            }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set
            {
                _tile.IsFixed = value;
                RaisePropertyChangedEvent();
            }
        }

        public bool IsEmpty
        {
            get { return _tile.IsEmpty; }
            set
            {
                _tile.IsEmpty = value;
                RaisePropertyChangedEvent();
            }
        }

        private ImageSource _assetSource;
        public ImageSource AssetSource
        {
            get { return _assetSource; }
            set
            {
                _assetSource = value;
                RaisePropertyChangedEvent();
            }
        }

        public BoardTileViewModel(BoardTile tile)
        {
            _tile = tile;
        }
    }
}
