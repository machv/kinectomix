using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class TilesViewModel : DependencyObject
    {
        public static readonly DependencyProperty PaintTileProperty = DependencyProperty.Register("PaintTile", typeof(BoardTileViewModel), typeof(TilesViewModel), new UIPropertyMetadata(null));

        public BoardTileViewModel PaintTile
        {
            get { return (BoardTileViewModel)GetValue(PaintTileProperty); }
            set { SetValue(PaintTileProperty, value); }
        }


        public static readonly DependencyProperty RowsCountProperty = DependencyProperty.Register("RowsCount", typeof(int), typeof(TilesViewModel), new UIPropertyMetadata(0));

        public int RowsCount
        {
            get { return (int)GetValue(RowsCountProperty); }
            set { SetValue(RowsCountProperty, value); }
        }

        public static readonly DependencyProperty ColumnsCountProperty = DependencyProperty.Register("ColumnsCount", typeof(int), typeof(TilesViewModel), new UIPropertyMetadata(0));

        public int ColumnsCount
        {
            get { return (int)GetValue(ColumnsCountProperty); }
            set { SetValue(ColumnsCountProperty, value); }
        }

        public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(ObservableCollection<BoardTileViewModel>), typeof(TilesViewModel), new UIPropertyMetadata(null));

        public ObservableCollection<BoardTileViewModel> Tiles
        {
            get { return (ObservableCollection<BoardTileViewModel>)GetValue(TilesProperty); }
            set { SetValue(TilesProperty, value); }
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
                    Tiles.Add(tile);
                }
            }
        }
    }
}
