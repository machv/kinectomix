using Kinectomix.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelEditor.ViewModel
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

        private LevelAsset _levelAsset;
        internal LevelAsset LevelAsseet
        {
            get { return _levelAsset; }
            set { _levelAsset = value; }
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
                OnPropertyChanged();
            }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set
            {
                _tile.IsFixed = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get { return _tile.IsEmpty; }
            set
            {
                _tile.IsEmpty = value;
                OnPropertyChanged();
            }
        }
        public BondType TopLeftBond
        {
            get { return _tile.TopLeftBond; }
            set
            {
                _tile.TopLeftBond = value;
                OnPropertyChanged();
            }
        }
        public BondType TopBond
        {
            get { return _tile.TopBond; }
            set
            {
                _tile.TopBond = value;
                OnPropertyChanged();
            }
        }
        public BondType TopRightBond
        {
            get { return _tile.TopRightBond; }
            set
            {
                _tile.TopRightBond = value;
                OnPropertyChanged();
            }
        }
        public BondType RightBond
        {
            get { return _tile.RightBond; }
            set
            {
                _tile.RightBond = value;
                OnPropertyChanged();
            }
        }
        public BondType BottomRightBond
        {
            get { return _tile.BottomRightBond; }
            set
            {
                _tile.BottomRightBond = value;
                OnPropertyChanged();
            }
        }
        public BondType BottomBond
        {
            get { return _tile.BottomBond; }
            set
            {
                _tile.BottomBond = value;
                OnPropertyChanged();
            }
        }
        public BondType BottomLeftBond
        {
            get { return _tile.BottomLeftBond; }
            set
            {
                _tile.BottomLeftBond = value;
                OnPropertyChanged();
            }
        }
        public BondType LeftBond
        {
            get { return _tile.LeftBond; }
            set
            {
                _tile.LeftBond = value;
                OnPropertyChanged();
            }
        }

        private bool _isPreview;
        public bool IsPreview
        {
            get { return _isPreview; }
            set
            {
                _isPreview = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _assetSource;
        public ImageSource AssetSource
        {
            get { return _assetSource; }
            set
            {
                _assetSource = value;
                OnPropertyChanged();
            }
        }

        public BoardTileViewModel() { }

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

        public BoardTileViewModel(BoardTile tile, LevelAsset asset)
        {
            _tile = tile;
            _assetFile = null;
            _levelAsset = asset;

            using (MemoryStream memoryStream = new MemoryStream(_levelAsset.DecodedAssetContent))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.None;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = memoryStream;
                bi.EndInit();

                _assetSource = bi;
            }
        }

        public string GetAssetCode()
        {
            return GetAssetCode(this);
        }

        public static string GetAssetCode(BoardTileViewModel tile)
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
}
