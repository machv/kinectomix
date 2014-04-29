using AtomixData;
using Kinectomix.LevelGenerator.Model;
using Kinectomix.LevelGenerator.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class EditorViewModel : NotifyPropertyBase
    {
        private Tiles _tiles;

        private IFileDialogService _levelFileDialog;
        public IFileDialogService LevelFileDialog
        {
            get { return _levelFileDialog; }
            set { _levelFileDialog = value; }
        }

        private IEnumerable<BoardTileViewModel> _availableTiles;
        public IEnumerable<BoardTileViewModel> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;

                RaisePropertyChangedEvent();
            }
        }

        private BoardTileViewModel _currentTileTemplate;
        public BoardTileViewModel CurrentTileTemplate
        {
            get { return _currentTileTemplate; }
            set
            {
                _currentTileTemplate = value;

                Level.Board.PaintTile = value;

                RaisePropertyChangedEvent();
            }
        }

        private BoardTileViewModel _currentBoardTile;
        public BoardTileViewModel CurrentBoardTile
        {
            get { return _currentBoardTile; }
            set
            {
                _currentBoardTile = value;

                RaisePropertyChangedEvent();

                UpdateCurrentTileByTemplate(CurrentBoardTile);
            }
        }

        private BoardTileViewModel _currentMoleculeTile;
        public BoardTileViewModel CurrentMoleculeTile
        {
            get { return _currentMoleculeTile; }
            set
            {
                _currentMoleculeTile = value;

                RaisePropertyChangedEvent();

                UpdateCurrentTileByTemplate(CurrentMoleculeTile);
            }
        }

        private void UpdateCurrentTileByTemplate(BoardTileViewModel _currentTile)
        {
            if (CurrentTileTemplate == null)
                return;

            _currentTile.Asset = CurrentTileTemplate.Asset;
            _currentTile.AssetSource = CurrentTileTemplate.AssetSource;
            _currentTile.AssetFile = CurrentTileTemplate.AssetFile;
        }

        private LevelViewModel _level;
        public LevelViewModel Level
        {
            get { return _level; }
            set
            {
                _level = value;

                RaisePropertyChangedEvent();
            }
        }

        private int _selectedTab = 0;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;

                UpdateAvailableTiles(_selectedTab);
                RaisePropertyChangedEvent();
            }
        }

        private void UpdateAvailableTiles(int selectedTab)
        {
            switch (selectedTab)
            {
                case 0:
                    AvailableTiles = _tiles.Board;
                    break;
                case 1:
                    AvailableTiles = _tiles.Molecule;
                    break;
            }
        }

        public EditorViewModel()
        {
            LoadDefaultAssets();

            _levelFileDialog = new LevelFileDialog();
            _saveAsLevelCommand = new DelegateCommand(SaveAsLevel, CanExecuteSaveAs);
        }

        private void LoadDefaultAssets()
        {
            BoardTile tile;
            BoardTileViewModel tileVm;

            _tiles = new Tiles();

            tile = new BoardTile() { IsFixed = true, IsEmpty = true, Asset = "Empty" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            _tiles.Add(tileVm, Tiles.TileType.Board);
            _tiles.Add(tileVm, Tiles.TileType.Molecule);

            tile = new BoardTile() { IsFixed = true, IsEmpty = false, Asset = "Wall" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            _tiles.Add(tileVm, Tiles.TileType.Board);

            string absolute = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.TilesDirectory);
            string[] tiles = Directory.GetFiles(absolute, "*.png");
            foreach (string tilePath in tiles)
            {
                string tileName = Path.GetFileNameWithoutExtension(tilePath);

                tile = new BoardTile() { IsFixed = false, IsEmpty = false, Asset = tileName };
                tileVm = new BoardTileViewModel(tile, tilePath);
                _tiles.Add(tileVm, Tiles.TileType.Board);
                _tiles.Add(tileVm, Tiles.TileType.Molecule);
            }
        }

        public ICommand LoadLevelCommand
        {
            get { return new DelegateCommand(LoadLevelDialog); }
        }

        private DelegateCommand _saveAsLevelCommand;
        public ICommand SaveAsLevelCommand
        {
            get { return _saveAsLevelCommand; }
        }

        private bool CanExecuteSaveAs(object parameter)
        {
            return Level != null;
        }

        public ICommand NewLevelCommand
        {
            get { return new DelegateCommand(NewLevel); }
        }

        private void NewLevel()
        {
            View.NewLevelWindow newLevelWindow = new View.NewLevelWindow();

            bool? result = newLevelWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                NewLevelViewModel newLevelVm = newLevelWindow.DataContext as NewLevelViewModel;

                LevelViewModel level = new LevelViewModel();
                level.Board = new BoardViewModel() { ColumnsCount = newLevelVm.BoardColumns, RowsCount = newLevelVm.BoardRows };
                level.Board.PopulateEmptyTiles(_tiles["Empty"]);

                level.Molecule = new MoleculeViewModel() { ColumnsCount = newLevelVm.MoleculeColumns, RowsCount = newLevelVm.MoleculeRows };
                level.Molecule.PopulateEmptyTiles(_tiles["Empty"]);

                LoadDefaultAssets();

                Level = level;
                SelectedTab = 0; // Reset to Board tab

                _saveAsLevelCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadLevelsCommand
        {
            get { return new DelegateCommand(LoadLevels); }
        }

        private void LoadLevels()
        {
            LevelsViewModel levels = new LevelsViewModel();
        }

        private void SaveAsLevel()
        {
            Level level = Level.ToLevel(_tiles);

            if (_levelFileDialog.SaveFileDialog())
            {
                FileMode mode = File.Exists(_levelFileDialog.FileName) ? FileMode.Truncate : FileMode.OpenOrCreate;
                using (Stream stream = File.Open(_levelFileDialog.FileName, mode))
                {
                    switch (_levelFileDialog.FilterIndex)
                    {
                        case 1: // XML
                            LevelFactory.SaveLevelDefinition(level, stream);
                            break;
                        case 2: // Compiled
                            LevelFactory.SaveLevelCompiled(level, stream, _levelFileDialog.FileName);
                            break;
                        case 3: // Custom binary
                            LevelFactory.SaveLevelBinary(level, stream);
                            break;
                        case 4: // Custom xml serialized
                            LevelFactory.SaveLevelXmlSerialized(level, stream);
                            break;
                    }
                }
            }
        }

        private void LoadLevelDialog()
        {
            if (_levelFileDialog.OpenFileDialog())
            {
                LoadLevel(_levelFileDialog.FileName);
            }
        }

        public void LoadLevel(string path)
        {
            Level level = LevelFactory.Load(path);
            Level = LevelViewModel.FromLevel(level);

            _saveAsLevelCommand.RaiseCanExecuteChanged();
        }
    }
}
