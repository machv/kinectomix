using AtomixData;
using Kinectomix.LevelGenerator.Model;
using Kinectomix.LevelGenerator.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

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

        private IEnumerable<BoardTile> _availableTiles;
        public IEnumerable<BoardTile> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;

                RaisePropertyChangedEvent();
            }
        }

        private BoardTile _currentTileTemplate;
        public BoardTile CurrentTileTemplate
        {
            get { return _currentTileTemplate; }
            set
            {
                _currentTileTemplate = value;

                RaisePropertyChangedEvent();
            }
        }

        private BoardTileViewModel _currentTile;
        public BoardTileViewModel CurrentTile
        {
            get { return _currentTile; }
            set
            {
                _currentTile = value;

                RaisePropertyChangedEvent();

                UpdateCurrentTileByTemplate(CurrentTile);
            }
        }

        private void UpdateCurrentTileByTemplate(BoardTileViewModel _currentTile)
        {
            if (CurrentTileTemplate == null)
                return;

            _currentTile.Asset = CurrentTileTemplate.Asset;
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
            _tiles = new Tiles();
            _levelFileDialog = new LevelFileDialog();

            _availableTiles = _tiles.Board;

            _saveAsLevelCommand = new DelegateCommand(SaveAsLevel, CanExecuteSaveAs);
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
            Level level = Level.ToLevel();

            if (_levelFileDialog.SaveFileDialog())
            {
                using (Stream stream = File.Open(_levelFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    switch (_levelFileDialog.FilterIndex)
                    {
                        case 1: // XML
                            LevelFactory.SaveLevelDefinition(level, stream);
                            break;
                        case 2: // Compiled
                            LevelFactory.SaveLevelCompiled(level, stream, _levelFileDialog.FileName);
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
