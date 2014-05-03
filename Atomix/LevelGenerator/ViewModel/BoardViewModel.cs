using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AtomixData;
using Kinectomix.LevelGenerator.Model;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class BoardViewModel : Mvvm.NotifyPropertyBase // DependencyObject
    {
        //public static readonly DependencyProperty RowsCountProperty = DependencyProperty.Register("RowsCount", typeof(int), typeof(BoardViewModel));
        //public static readonly DependencyProperty ColumnsCountProperty = DependencyProperty.Register("ColumnsCount", typeof(int), typeof(BoardViewModel));
        //public static readonly DependencyProperty PaintTileProperty = DependencyProperty.Register("PaintTile", typeof(BoardTileViewModel), typeof(BoardViewModel));

        //public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(TilesCollection<BoardTileViewModel>), typeof(BoardViewModel), new UIPropertyMetadata(null));

        //public TilesCollection<BoardTileViewModel> Tiles
        //{
        //    get { return (TilesCollection<BoardTileViewModel>)GetValue(TilesProperty); }
        //    set { SetValue(TilesProperty, value); }
        //}


        //private ObservableCollection<BoardTileViewModel> _tiles = new ObservableCollection<BoardTileViewModel>();
        private TilesCollection<BoardTileViewModel> _tiles = new TilesCollection<BoardTileViewModel>();

        public TilesCollection<BoardTileViewModel> Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                RaisePropertyChangedEvent();
            }
            //get { return new ReadOnlyObservableCollection<BoardTileViewModel>(_tiles); }
        }

        public int RowsCount
        {
            get { return _tiles.RowsCount; }
            set
            {
                _tiles.RowsCount = value;
                RaisePropertyChangedEvent();
            }
        }
        public int ColumnsCount
        {
            get { return _tiles.ColumnsCount; }
            set
            {
                _tiles.ColumnsCount = value;
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

        public BoardViewModel()
        {
            Tiles = new TilesCollection<BoardTileViewModel>();
        }
        public BoardViewModel(int rowsCount, int columnsCount) : this()
        {
            Tiles = new TilesCollection<BoardTileViewModel>(rowsCount, columnsCount);
        }
        public BoardViewModel(TilesCollection<BoardTile> board, Tiles tiles)
        {
            Tiles = new TilesCollection<BoardTileViewModel>(board.RowsCount, board.ColumnsCount);

            foreach (BoardTile tile in board)
            {
                BoardTileViewModel tileViewModel = new BoardTileViewModel(tile);
                tileViewModel.AssetSource = tiles[tile.Asset].AssetSource;

                Tiles.Add(tileViewModel);
            }
        }

        public void AddRow(BoardTileViewModel emptyTileTemplate)
        {
            var tiles = Tiles;
            tiles.AddRow();

            for (int i = 0; i < ColumnsCount; i++)
            {
                BoardTileViewModel tile = new BoardTileViewModel(new AtomixData.BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
                tile.AssetSource = emptyTileTemplate.AssetSource;
                tile.AssetFile = emptyTileTemplate.AssetFile;

                tiles[RowsCount - 1, i] = tile;
            }

            Tiles = null;
            Tiles = tiles;
            RowsCount = _tiles.RowsCount;
        }

        public void PopulateEmptyTiles(BoardTileViewModel emptyTileTemplate)
        {
            Tiles = new TilesCollection<BoardTileViewModel>(RowsCount, ColumnsCount);

            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    BoardTileViewModel tile = new BoardTileViewModel(new AtomixData.BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
                    tile.AssetSource = emptyTileTemplate.AssetSource;
                    tile.AssetFile = emptyTileTemplate.AssetFile;
                    Tiles.Add(tile);
                }
            }
        }
    }
}
