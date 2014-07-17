using Mach.Wpf.Mvvm;
using Mach.Kinectomix.Logic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mach.Kinectomix.LevelEditor.ViewModel
{
    public class BoardTileViewModel : NotifyPropertyBase
    {
        protected static ImageSourceConverter SourceConverter = new ImageSourceConverter();

        private BoardTile _tile;
        private string _assetFile;
        private bool _isChanged;

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
                if (_tile.Asset != value)
                {
                    _tile.Asset = value;

                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return _tile.Name; }
            set
            {
                if (_tile.Name != value)
                {
                    _tile.Name = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set
            {
                if (_tile.IsFixed != value)
                {
                    _tile.IsFixed = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool IsEmpty
        {
            get { return _tile.IsEmpty; }
            set
            {
                if (_tile.IsEmpty != value)
                {
                    _tile.IsEmpty = value;

                    OnPropertyChanged();
                }
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

        public bool HasBonds
        {
            get { return Tile.HasBonds; }
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

        private bool _isClear = false;
        public bool IsClear
        {
            get { return _isClear; }
            set
            {
                _isClear = value;
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

        public bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; }
        }

        public BoardTileViewModel()
        {
            PropertyChanged += WhenPropertyChanged;
        }

        private void WhenPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _isChanged = true;
        }

        public BoardTileViewModel(BoardTile tile) : this()
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

            using (MemoryStream stream = new MemoryStream(_levelAsset.DecodedAssetContent))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapFrame frame = decoder.Frames.First();
                frame.Freeze();

                _assetSource = frame;
            }
        }

        public string GetAssetCode(string code)
        {
            return GetAssetCode(this, code);
        }

        public static string GetAssetCode(BoardTileViewModel tile, string code)
        {
            return string.Format("{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}_{0}", code,
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
