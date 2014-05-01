using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AtomixData;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class TilesViewModel : DependencyObject
    {
        public static readonly DependencyProperty RowsCountProperty = DependencyProperty.Register("RowsCount", typeof(int), typeof(TilesViewModel));
        public static readonly DependencyProperty ColumnsCountProperty = DependencyProperty.Register("ColumnsCount", typeof(int), typeof(TilesViewModel));
        public static readonly DependencyProperty PaintTileProperty = DependencyProperty.Register("PaintTile", typeof(BoardTileViewModel), typeof(TilesViewModel));

        private ObservableCollection<BoardTileViewModel> _tiles = new ObservableCollection<BoardTileViewModel>();

        public IEnumerable<BoardTileViewModel> Tiles
        {
            get { return new ReadOnlyObservableCollection<BoardTileViewModel>(_tiles); }
        }

        public int RowsCount
        {
            get { return (int)GetValue(RowsCountProperty); }
            set { SetValue(RowsCountProperty, value); }
        }
        public int ColumnsCount
        {
            get { return (int)GetValue(ColumnsCountProperty); }
            set { SetValue(ColumnsCountProperty, value); }
        }
        public BoardTileViewModel PaintTile
        {
            get { return (BoardTileViewModel)GetValue(PaintTileProperty); }
            set { SetValue(PaintTileProperty, value); }
        }

        public TilesViewModel()
        {
        }
        public TilesViewModel(int rowsCount, int columnsCount) : this()
        {
            ColumnsCount = columnsCount;
            RowsCount = rowsCount;
        }

        public TilesViewModel(TilesCollection<BoardTile> board)
        {
            ColumnsCount = board.ColumnsCount;
            RowsCount = board.RowsCount;

            foreach (BoardTile tile in board)
            {
                BoardTileViewModel tileViewModel = new BoardTileViewModel(tile);
                _tiles.Add(tileViewModel);
            }
        }

        public void PopulateEmptyTiles(BoardTileViewModel emptyTileTemplate)
        {
            _tiles.Clear();

            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    BoardTileViewModel tile = new BoardTileViewModel(new AtomixData.BoardTile() { IsEmpty = emptyTileTemplate.IsEmpty, Asset = emptyTileTemplate.Asset, IsFixed = emptyTileTemplate.IsFixed });
                    tile.AssetSource = emptyTileTemplate.AssetSource;
                    tile.AssetFile = emptyTileTemplate.AssetFile;
                    _tiles.Add(tile);
                }
            }
        }
    }
}
