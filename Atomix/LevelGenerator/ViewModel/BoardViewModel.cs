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
        //public static readonly DependencyProperty PaintTileProperty = DependencyProperty.Register("PaintTile", typeof(BoardTileViewModel), typeof(BoardViewModel));

        //public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(TilesCollection<BoardTileViewModel>), typeof(BoardViewModel), new UIPropertyMetadata(null));

        //public TilesCollection<BoardTileViewModel> Tiles
        //{
        //    get { return (TilesCollection<BoardTileViewModel>)GetValue(TilesProperty); }
        //    set { SetValue(TilesProperty, value); }
        //}

        private ObservableTilesCollection<BoardTileViewModel> _tiles = new ObservableTilesCollection<BoardTileViewModel>();

        public ObservableTilesCollection<BoardTileViewModel> Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                OnPropertyChanged();
            }
        }

        private BoardTileViewModel _paintTile; 
        public BoardTileViewModel PaintTile
        {
            get { return _paintTile; }
            set
            {
                _paintTile = value;
                OnPropertyChanged();
            }
        }

        public BoardViewModel()
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>();
        }
        public BoardViewModel(int rowsCount, int columnsCount) : this()
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(rowsCount, columnsCount);
        }
        public BoardViewModel(TilesCollection<BoardTile> board, Tiles tiles)
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(board.RowsCount, board.ColumnsCount);

            foreach (BoardTile tile in board)
            {
                BoardTileViewModel tileViewModel = new BoardTileViewModel(tile);
                tileViewModel.AssetSource = tiles[tile.Asset].AssetSource;

                Tiles.Add(tileViewModel);
            }
        }

        public void AddRow(BoardTileViewModel emptyTileTemplate)
        {
            Tiles.RemoveColumn(1);

            //for (int i = 0; i < ColumnsCount; i++)
            //{
            //    BoardTileViewModel tile = new BoardTileViewModel(new AtomixData.BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
            //    tile.AssetSource = emptyTileTemplate.AssetSource;
            //    tile.AssetFile = emptyTileTemplate.AssetFile;

            //    tiles[RowsCount - 1, i] = tile;
            //}
        }

        public void PopulateEmptyTiles(BoardTileViewModel emptyTileTemplate)
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(_tiles.RowsCount, _tiles.ColumnsCount);

            for (int i = 0; i < _tiles.RowsCount; i++)
            {
                for (int j = 0; j < _tiles.ColumnsCount; j++)
                {
                    BoardTileViewModel tile = new BoardTileViewModel(new BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
                    tile.AssetSource = emptyTileTemplate.AssetSource;
                    tile.AssetFile = emptyTileTemplate.AssetFile;
                    Tiles.Add(tile);
                }
            }
        }
    }
}
