using Kinectomix.Logic;
using Kinectomix.LevelEditor.Model;
using System.IO;
using System.Windows.Input;
using Kinectomix.Wpf.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class EditorViewModel : NotifyPropertyBase
    {
        public const string DefaultLevelName = "Kinectomix Level";
        private int _newLevelIdnex;
        private Tiles _tiles;
        private ObservableCollection<LevelViewModel> _levels;
        //private LevelViewModel _currentLevel;

        //public LevelViewModel CurrentLevel
        //{
        //    get { return _currentLevel; }
        //    set
        //    {
        //        _currentLevel = value;
        //        OnPropertyChanged();
        //    }
        //}
        public ObservableCollection<LevelViewModel> Levels
        {
            get { return _levels; }
            set
            {
                _levels = value;
                OnPropertyChanged();
            }
        }

        private IFileDialogService _levelFileDialog;
        public IFileDialogService LevelFileDialog
        {
            get { return _levelFileDialog; }
            set { _levelFileDialog = value; }
        }

        private AvailableTilesViewModel _tileSelector;
        public AvailableTilesViewModel TileSelector
        {
            get { return _tileSelector; }
            set
            {
                _tileSelector = value;

                OnPropertyChanged();
            }
        }

        private LevelViewModel _level;
        public LevelViewModel Level
        {
            get { return _level; }
            set
            {
                _level = value;

                OnPropertyChanged();
            }
        }
        private DelegateCommand _importLevelCommand;
        private string _userAtomAssetsPath;
        private string _userFixedAssetsPath;

        public EditorViewModel()
        {
            _userAtomAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.AtomTilesDirectory);
            _userFixedAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.FixedTilesDirectory);

            _levelFileDialog = new LevelFileDialog();
            _saveAsLevelCommand = new DelegateCommand(SaveAsLevel, CanExecuteSaveAs);

            _tileSelector = new AvailableTilesViewModel();
            _tileSelector.TileSelected += Selector_TileSelected;

            _saveAsLevelsDefinitionCommand = new DelegateCommand(SaveAsLevelsDefinition, CanExecuteSaveAsLevelsDefinition);
            _loadLevelsDefinitionCommand = new DelegateCommand(LoadLevelsDefinition);
            _newLevelsDefinitionCommand = new DelegateCommand(NewLevelsDefinition);
            _addNewLevelCommand = new DelegateCommand(AddNewLevel, CanExecuteAddNewLevel);
            _importLevelCommand = new DelegateCommand(OpenLevelDialog);

            NewLevelsDefinition(); // Create new levels definition
        }

        private bool CanExecuteAddNewLevel(object parameter)
        {
            return _levels != null;
        }

        /// <summary>
        /// Adds new level to the list of levels and shows it in the editor.
        /// </summary>
        private void AddNewLevel()
        {
            LevelViewModel level = CreateNewLevel();
            level.Name = string.Format("{0} {1}", DefaultLevelName, ++_newLevelIdnex);
            
            Levels.Add(level);
            ShowLevel(level);
        }

        private bool _isLevelDefinitionChanged;
        private bool CanExecuteSaveAsLevelsDefinition(object parameter)
        {
            return _isLevelDefinitionChanged;
        }

        private void NewLevelsDefinition()
        {
            Levels = new ObservableCollection<LevelViewModel>();

            AddNewLevel(); // Add new level inside it

            _addNewLevelCommand.RaiseCanExecuteChanged();
        }

        private void LoadLevelsDefinition()
        {
            throw new NotImplementedException();
        }

        private void SaveAsLevelsDefinition()
        {
            throw new NotImplementedException();
        }

        private void Selector_TileSelected(object sender, TileSelectedEventArgs e)
        {
            if (Level == null)
                return;

            if (Level.Board != null)
                Level.Board.PaintTile = e.Tile;

            if (Level.Molecule != null)
                Level.Molecule.PaintTile = e.Tile;
        }

        private DelegateCommand _addNewLevelCommand;
        public ICommand AddNewLevelCommand
        {
            get { return _addNewLevelCommand; }
        }

        private DelegateCommand _newLevelsDefinitionCommand;
        public ICommand NewLevelsDefinitionCommand
        {
            get { return _newLevelsDefinitionCommand; }
        }

        private DelegateCommand _loadLevelsDefinitionCommand;
        public ICommand LoadLevelsDefinitionCommand
        {
            get { return _loadLevelsDefinitionCommand; }
        }

        private DelegateCommand _saveAsLevelsDefinitionCommand;
        public ICommand SaveAsLevelsDefinitionCommand
        {
            get { return _saveAsLevelsDefinitionCommand; }
        }

        public ICommand ImportLevelCommand
        {
            get { return _importLevelCommand; }
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

        private void ShowLevel(LevelViewModel levelViewModel)
        {
            //TODO check if field changed? if yes, allow save before new.

            _tileSelector.Tiles = levelViewModel.Tiles;
            _tiles = levelViewModel.Tiles;
            Level = levelViewModel;
            SelectedTab = 0; // Reset to Board tab

            _saveAsLevelCommand.RaiseCanExecuteChanged();
        }

        private LevelViewModel CreateNewLevel()
        {
            LevelViewModel level = new LevelViewModel();
            level.Tiles = new Tiles();
            level.Tiles.LoadUserAssets(Tiles.AssetType.Fixed, _userFixedAssetsPath);
            level.Tiles.LoadUserAssets(Tiles.AssetType.Atom, _userAtomAssetsPath);

            level.Board = new BoardViewModel(Properties.Settings.Default.DefaultBoardRows, Properties.Settings.Default.DefaultBoardColumns);
            level.Board.EmptyTileTemplate = level.Tiles["Empty"];
            level.Board.PopulateEmptyTiles();

            level.Molecule = new BoardViewModel(Properties.Settings.Default.DefaultMoleculeRows, Properties.Settings.Default.DefaultMoleculeColumns);
            level.Molecule.EmptyTileTemplate = level.Tiles["Empty"];
            level.Molecule.PopulateEmptyTiles();

            return level;
        }

        private int _selectedTab = 0;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;

                UpdateAvailableTiles(_selectedTab);
                //RaisePropertyChangedEvent();
            }
        }

        private void UpdateAvailableTiles(int _selectedTab)
        {
            switch (_selectedTab)
            {
                case 0:
                    _tileSelector.UpdateAvailableTiles(Tiles.TileType.Board);
                    break;
                case 1:
                    _tileSelector.UpdateAvailableTiles(Tiles.TileType.Molecule);
                    break;
            }
        }

        public ICommand LoadLevelsCommand
        {
            get { return new DelegateCommand(LoadLevels); }
        }

        public ICommand TestCommand
        {
            get { return new DelegateCommand(Test); }
        }

        public void Test()
        {

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
                        case 1: // Custom xml serialized
                            LevelFactory.SaveLevelXmlSerialized(level, stream);
                            break;
                        case 2: // Compiled
                            LevelFactory.SaveLevelCompiled(level, stream, _levelFileDialog.FileName);
                            break;
                        case 3: // Custom binary
                            LevelFactory.SaveLevelBinary(level, stream);
                            break;
                        case 4: // XML
                            LevelFactory.SaveLevelDefinition(level, stream);
                            break;
                    }
                }
            }
        }

        private void OpenLevelDialog()
        {
            if (_levelFileDialog.OpenFileDialog())
            {
                ImportLevel(_levelFileDialog.FileName);
            }
        }

        public void ImportLevel(string path)
        {
            Level level = LevelFactory.Load(path);
            LevelViewModel levelViewModel = LevelViewModel.FromLevel(level, _userFixedAssetsPath, _userAtomAssetsPath);

            Levels.Add(levelViewModel);
            ShowLevel(levelViewModel);

            _saveAsLevelCommand.RaiseCanExecuteChanged();
        }
    }
}
