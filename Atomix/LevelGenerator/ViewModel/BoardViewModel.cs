using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AtomixData;
using Kinectomix.LevelGenerator.Model;
using System.Windows.Input;
using Kinectomix.LevelGenerator.Mvvm;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public enum ResizeMode
    {
        TopAdd,
        TopRemove,
    }

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

            _insertRowToTopCommand = new DelegateCommand(InsertRowToTop, CanInsertRowToTop);
            _insertRowToBottomCommand = new DelegateCommand(InsertRowToBottom, CanInsertRowToBottom);
            _removeRowFromTopCommand = new DelegateCommand(RemoveRowFromTop, CanRemoveRowFromTop);
            _removeRowFromBottomCommand = new DelegateCommand(RemoveRowFromBottom, CanRemoveRowFromBottom);
            _insertColumnToLeftCommand = new DelegateCommand(InsertColumnToLeft, CanInsertColumnToLeft);
            _insertColumnToRightCommand = new DelegateCommand(InsertColumnToRight, CanInsertColumnToRight);
            _removeColumnFromLeftCommand = new DelegateCommand(RemoveColumnFromLeft, CanRemoveColumnFromLeft);
            _removeColumnFromRightCommand = new DelegateCommand(RemoveColumnFromRight, CanRemoveColumnFromRight);
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

        private DelegateCommand _insertRowToTopCommand;
        private DelegateCommand _insertRowToBottomCommand;
        private DelegateCommand _removeRowFromTopCommand;
        private DelegateCommand _removeRowFromBottomCommand;
        private DelegateCommand _insertColumnToLeftCommand;
        private DelegateCommand _insertColumnToRightCommand;
        private DelegateCommand _removeColumnFromLeftCommand;
        private DelegateCommand _removeColumnFromRightCommand;

        public ICommand InsertRowToTopCommand
        {
            get { return _insertRowToTopCommand; }
        }
        public ICommand InsertRowToBottomCommand
        {
            get { return _insertRowToBottomCommand; }
        }
        public ICommand RemoveRowFromTopCommand
        {
            get { return _removeRowFromTopCommand; }
        }
        public ICommand RemoveRowFromBottomCommand
        {
            get { return _removeRowFromBottomCommand; }
        }
        public ICommand InsertColumnToLeftCommand
        {
            get { return _insertColumnToLeftCommand; }
        }
        public ICommand InsertColumnToRightCommand
        {
            get { return _insertColumnToRightCommand; }
        }
        public ICommand RemoveColumnFromLeftCommand
        {
            get { return _removeColumnFromLeftCommand; }
        }
        public ICommand RemoveColumnFromRightCommand
        {
            get { return _removeColumnFromRightCommand; }
        }

        protected void InsertRowToTop()
        {
            Tiles.InsertRow(0);

            RaiseChanged();
        }
        private bool CanInsertRowToTop(object parameter)
        {
            return true;
        }

        protected void InsertRowToBottom()
        {
            Tiles.InsertRow(Tiles.RowsCount);

            RaiseChanged();
        }
        private bool CanInsertRowToBottom(object parameter)
        {
            return true;
        }

        protected void RemoveRowFromTop()
        {
            Tiles.RemoveRow(0);

            RaiseChanged();
        }
        private bool CanRemoveRowFromTop(object parameter)
        {
            return Tiles.RowsCount > 1;
        }

        protected void RemoveRowFromBottom()
        {
            Tiles.RemoveRow(Tiles.RowsCount - 1);

            RaiseChanged();
        }
        private bool CanRemoveRowFromBottom(object parameter)
        {
            return Tiles.RowsCount > 1;
        }
        protected void InsertColumnToLeft()
        {
            Tiles.InsertRow(0);

            RaiseChanged();
        }
        private bool CanInsertColumnToLeft(object parameter)
        {
            return true;
        }

        protected void InsertColumnToRight()
        {
            Tiles.InsertColumn(Tiles.ColumnsCount);

            RaiseChanged();
        }
        private bool CanInsertColumnToRight(object parameter)
        {
            return true;
        }

        protected void RemoveColumnFromLeft()
        {
            Tiles.RemoveColumn(0);

            RaiseChanged();
        }
        private bool CanRemoveColumnFromLeft(object parameter)
        {
            return Tiles.ColumnsCount > 1;
        }

        protected void RemoveColumnFromRight()
        {
            Tiles.RemoveColumn(Tiles.ColumnsCount - 1);

            RaiseChanged();
        }
        private bool CanRemoveColumnFromRight(object parameter)
        {
            return Tiles.ColumnsCount > 1;
        }

        private void RaiseChanged()
        {
            _removeRowFromTopCommand.RaiseCanExecuteChanged();
            _removeRowFromBottomCommand.RaiseCanExecuteChanged();
            _insertRowToTopCommand.RaiseCanExecuteChanged();
            _insertRowToBottomCommand.RaiseCanExecuteChanged();
            _removeColumnFromLeftCommand.RaiseCanExecuteChanged();
            _removeColumnFromRightCommand.RaiseCanExecuteChanged();
            _insertColumnToLeftCommand.RaiseCanExecuteChanged();
            _insertColumnToRightCommand.RaiseCanExecuteChanged();
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
