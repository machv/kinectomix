using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class TilesViewModel : Mvvm.NotifyPropertyBase
    {
        ObservableCollection<BoardTileViewModel> _atoms = new ObservableCollection<BoardTileViewModel>();

        private int _columnsCount;
        public int ColumnsCount
        {
            get { return _columnsCount; }
            set
            {
                _columnsCount = value;

                RaisePropertyChangedEvent();
            }
        }

        private BoardTileViewModel _paintTile;
        public BoardTileViewModel PaintTile
        {
            get { return _paintTile; }
            set
            {
                _paintTile = value;

                RaisePropertyChangedEvent();
            }
        }

        private int _rowsCount;
        public int RowsCount
        {
            get { return _rowsCount; }
            set
            {
                _rowsCount = value;

                RaisePropertyChangedEvent();
            }
        }

        public ObservableCollection<BoardTileViewModel> Tiles
        {
            get { return _atoms; }
            set
            {
                _atoms = value;

                RaisePropertyChangedEvent();
            }
        }

        public TilesViewModel() { }
        public TilesViewModel(int rowsCount, int columnsCount)
        {
            ColumnsCount = columnsCount;
            RowsCount = rowsCount;
        }

        public void PopulateEmptyTiles(BoardTileViewModel emptyTileTemplate)
        {
            _atoms.Clear();

            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    BoardTileViewModel tile = new BoardTileViewModel(new AtomixData.BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
                    tile.AssetSource = emptyTileTemplate.AssetSource;
                    tile.AssetFile = emptyTileTemplate.AssetFile;
                    _atoms.Add(tile);
                }
            }
        }
    }
}
