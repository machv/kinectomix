using Kinectomix.LevelEditor.Model;
using System.Windows.Input;
using Mach.Wpf.Mvvm;
using Mach.Kinectomix.Logic;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class BoardViewModel : NotifyPropertyBase
    {
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

        public bool IsChanged
        {
            get
            {
                foreach (BoardTileViewModel tile in _tiles)
                {
                    if (tile != null && tile.IsChanged == true)
                        return true;
                }

                return false;
            }
            set
            {
                foreach (BoardTileViewModel tile in _tiles)
                    if (tile != null)
                        tile.IsChanged = value;
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

        private BoardTileViewModel _emptyTileTemplate;
        public BoardTileViewModel EmptyTileTemplate
        {
            get { return _emptyTileTemplate; }
            set
            {
                _emptyTileTemplate = value;
                OnPropertyChanged();
            }
        }

        public BoardViewModel()
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>();

            _insertRowToTopCommand = new DelegateCommand(InsertRowToTop);
            _insertRowToBottomCommand = new DelegateCommand(InsertRowToBottom);
            _removeRowFromTopCommand = new DelegateCommand(RemoveRowFromTop, CanRemoveRow);
            _removeRowFromBottomCommand = new DelegateCommand(RemoveRowFromBottom, CanRemoveRow);
            _insertColumnToLeftCommand = new DelegateCommand(InsertColumnToLeft);
            _insertColumnToRightCommand = new DelegateCommand(InsertColumnToRight);
            _removeColumnFromLeftCommand = new DelegateCommand(RemoveColumnFromLeft, CanRemoveColumn);
            _removeColumnFromRightCommand = new DelegateCommand(RemoveColumnFromRight, CanRemoveColumn);
        }
        public BoardViewModel(int rowsCount, int columnsCount) : this()
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(rowsCount, columnsCount);
        }
        public BoardViewModel(TilesCollection<BoardTile> board, Tiles tiles) : this()
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(board.RowsCount, board.ColumnsCount);

            foreach (BoardTile tile in board)
            {
                BoardTileViewModel tileViewModel;
                if (tile == null)
                {
                    tileViewModel = new BoardTileViewModel(new BoardTile() { Asset = "Clean" });
                    tileViewModel.IsClear = true;
                }
                else
                {
                    tileViewModel = new BoardTileViewModel(tile);
                }

                tileViewModel.AssetSource = tiles[tileViewModel.Asset].AssetSource;

                Tiles.Add(tileViewModel);
            }
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
            FillRow(0, _emptyTileTemplate);

            RaiseChanged();
        }
        protected void InsertRowToBottom()
        {
            int rowIndex = Tiles.RowsCount;
            Tiles.InsertRow(rowIndex);
            FillRow(rowIndex, _emptyTileTemplate);

            RaiseChanged();
        }
        private bool CanRemoveRow(object parameter)
        {
            return Tiles.RowsCount > 1;
        }
        protected void RemoveRowFromTop()
        {
            Tiles.RemoveRow(0);

            RaiseChanged();
        }
        protected void RemoveRowFromBottom()
        {
            Tiles.RemoveRow(Tiles.RowsCount - 1);

            RaiseChanged();
        }
        protected void InsertColumnToLeft()
        {
            Tiles.InsertColumn(0);
            FillColumn(0, _emptyTileTemplate);

            RaiseChanged();
        }
        protected void InsertColumnToRight()
        {
            int columnIndex = Tiles.ColumnsCount;
            Tiles.InsertColumn(columnIndex);
            FillColumn(columnIndex, _emptyTileTemplate);

            RaiseChanged();
        }
        private bool CanRemoveColumn(object parameter)
        {
            return Tiles.ColumnsCount > 1;
        }
        protected void RemoveColumnFromLeft()
        {
            Tiles.RemoveColumn(0);

            RaiseChanged();
        }
        protected void RemoveColumnFromRight()
        {
            Tiles.RemoveColumn(Tiles.ColumnsCount - 1);

            RaiseChanged();
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

        public void PopulateEmptyTiles()
        {
            PopulateEmptyTiles(_emptyTileTemplate);
        }

        public void PopulateEmptyTiles(BoardTileViewModel emptyTileTemplate)
        {
            Tiles = new ObservableTilesCollection<BoardTileViewModel>(_tiles.RowsCount, _tiles.ColumnsCount);

            for (int i = 0; i < _tiles.RowsCount; i++)
                FillRow(i, emptyTileTemplate);
        }

        private void FillColumn(int columnIndex, BoardTileViewModel tileTemplate)
        {
            for (int i = 0; i < Tiles.RowsCount; i++)
            {
                BoardTileViewModel tile = new BoardTileViewModel(new BoardTile() { IsEmpty = tileTemplate.IsEmpty, Asset = tileTemplate.Asset, IsFixed = tileTemplate.IsFixed });
                tile.AssetSource = tileTemplate.AssetSource;
                tile.AssetFile = tileTemplate.AssetFile;

                Tiles[i, columnIndex] = tile;
            }
        }

        private void FillRow(int rowIndex, BoardTileViewModel tileTemplate)
        {
            for (int i = 0; i < Tiles.ColumnsCount; i++)
            {
                BoardTileViewModel tile = new BoardTileViewModel(new BoardTile() { IsEmpty = tileTemplate.IsEmpty, Asset = tileTemplate.Asset, IsFixed = tileTemplate.IsFixed });
                tile.AssetSource = tileTemplate.AssetSource;
                tile.AssetFile = tileTemplate.AssetFile;

                Tiles[rowIndex, i] = tile;
            }
        }

    }
}
