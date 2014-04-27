using AtomixData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelGenerator
{
    public class BoardTileViewModel : Mvvm.NotifyPropertyBase
    {
        protected static ImageSourceConverter SourceConverter = new ImageSourceConverter();

        BoardTile _tile;
        string _assetFile;

        internal string AssetFile
        {
            get { return _assetFile; }
            set { _assetFile = value; }
        }

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

        public BoardTileViewModel(BoardTile tile, string assetFile)
        {
            _tile = tile;
            _assetFile = assetFile;
            _assetSource = (ImageSource)(SourceConverter.ConvertFromString(assetFile));
        }
    }
}
