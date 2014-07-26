using System.IO;
using System.Windows.Input;
using Mach.Wpf.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;
using Mach.Kinectomix.Logic;
using Mach.Kinectomix.LevelEditor.Model;
using Mach.Kinectomix.LevelEditor.Localization;

namespace Mach.Kinectomix.LevelEditor.ViewModel
{
    public class EditorViewModel : NotifyPropertyBase
    {
        //public const string DefaultLevelName = "Kinectomix Level";
        private int _newLevelIndex;
        private Tiles _tiles;
        private ObservableCollection<LevelViewModel> _levels;
        private bool _areLevelsChanged;
        private GameLevelsFileDialog _gameLevelsFileDialog;
        private LocalizedStrings _localization = new LocalizedStrings();

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
                if (_level != value)
                {
                    _level = value;
                    ShowLevel(value);
                    OnPropertyChanged();
                }
            }
        }
        private DelegateCommand _importLevelCommand;
        private string _userAtomAssetsPath;
        private string _userFixedAssetsPath;
        private DelegateCommand<LevelViewModel> _removeLevelCommand;

        public EditorViewModel()
        {
            _userAtomAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.AtomTilesDirectory);
            _userFixedAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.FixedTilesDirectory);

            _levelFileDialog = new LevelFileDialog(true);
            _gameLevelsFileDialog = new GameLevelsFileDialog();

            _tileSelector = new AvailableTilesViewModel();
            _tileSelector.TileSelected += Selector_TileSelected;

            _exportLevelCommand = new DelegateCommand<LevelViewModel>(ExportLevel);
            _saveAsLevelsDefinitionCommand = new DelegateCommand(SaveAsLevelsDefinition, CanExecuteSaveAsLevelsDefinition);
            _loadLevelsDefinitionCommand = new DelegateCommand(DoLoadLevelsDefinition);
            _newLevelsDefinitionCommand = new DelegateCommand(NewLevelsDefinition);
            _addNewLevelCommand = new DelegateCommand(AddNewLevel, CanExecuteAddNewLevel);
            _importLevelCommand = new DelegateCommand(OpenLevelDialog);
            _removeLevelCommand = new DelegateCommand<LevelViewModel>(RemoveLevel);

            NewLevelsDefinition(); // Create new levels definition
            ResetPendingChanges(); // When created first empty level, we do not accept this as created
        }

        public ICommand RemoveLevelCommand
        {
            get { return _removeLevelCommand; }
        }

        private void RemoveLevel(LevelViewModel level)
        {
            Levels.Remove(level);
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
            level.Name = string.Format("{0} {1}", EditorResources.DefaultLevelName, ++_newLevelIndex);
            level.IsChanged = false;

            Levels.Add(level);
            ShowLevel(level);

            _areLevelsChanged = true;
            _saveAsLevelsDefinitionCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteSaveAsLevelsDefinition(object parameter)
        {
            return _levels != null && _levels.Count > 0;
        }

        public bool IsAnyPendingChange
        {
            get
            {
                if (_areLevelsChanged == true)
                    return true;

                if (_levels == null)
                    return false;

                bool isAnyLevelChanged = false;
                foreach (LevelViewModel level in _levels)
                {
                    if (level.IsChanged)
                    {
                        isAnyLevelChanged = true;
                        break;
                    }
                }

                return isAnyLevelChanged;
            }
        }

        private void ResetPendingChanges()
        {
            _areLevelsChanged = false;

            foreach (LevelViewModel level in _levels)
            {
                level.IsChanged = false;
            }
        }

        private void NewLevelsDefinition()
        {
            if (IsAnyPendingChange)
            {
                MessageBoxResult result = MessageBox.Show(EditorResources.NewDefinitionPendingChangesText, EditorResources.NewDefinitionPendingChangesTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;
            }

            _newLevelIndex = 0;
            Levels = new ObservableCollection<LevelViewModel>();

            AddNewLevel(); // Add new level inside it
            ResetPendingChanges();

            _addNewLevelCommand.RaiseCanExecuteChanged();
            _saveAsLevelsDefinitionCommand.RaiseCanExecuteChanged();
        }

        public void LoadLevelsDefinition(string path)
        {
            try
            {
                XmlSerializer seralizer = new XmlSerializer(typeof(GameDefinition));

                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    Levels.Clear();
                    GameDefinition game = seralizer.Deserialize(stream) as GameDefinition;

                    foreach (Level level in game.Levels)
                    {
                        LevelViewModel levelViewModel = LevelViewModel.FromLevel(level, _userFixedAssetsPath, _userAtomAssetsPath);

                        Levels.Add(levelViewModel);
                    }

                    if (Levels.Count > 0)
                        Level = Levels[0]; // Select first level
                }
            }
            catch
            {
                MessageBox.Show("Unable to load definition from selected file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DoLoadLevelsDefinition()
        {
            if (IsAnyPendingChange)
            {
                MessageBoxResult result = MessageBox.Show(EditorResources.OpenDefinitionPendingChangesText, EditorResources.OpenDefinitionPendingChangesTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;
            }

            if (_gameLevelsFileDialog.OpenFileDialog())
            {
                LoadLevelsDefinition(_gameLevelsFileDialog.FileName);
            }
        }

        private void SaveAsLevelsDefinition()
        {
            if (_gameLevelsFileDialog.SaveFileDialog())
            {
                GameDefinition game = new GameDefinition(Levels.Count);

                foreach (LevelViewModel levelVm in Levels)
                {
                    Level level = levelVm.ToLevel();
                    game.AddLevel(level);
                }

                FileMode mode = File.Exists(_gameLevelsFileDialog.FileName) ? FileMode.Truncate : FileMode.OpenOrCreate;
                using (Stream stream = File.Open(_gameLevelsFileDialog.FileName, mode))
                {
                    XmlSerializer seralizer = new XmlSerializer(typeof(GameDefinition));
                    seralizer.Serialize(stream, game);
                }

                ResetPendingChanges();
            }
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

        private DelegateCommand<LevelViewModel> _exportLevelCommand;
        public ICommand ExportLevelCommand
        {
            get { return _exportLevelCommand; }
        }

        private void ShowLevel(LevelViewModel levelViewModel)
        {
            if (levelViewModel == null)
                return;

            _tileSelector.Tiles = levelViewModel.Tiles;
            _tiles = levelViewModel.Tiles;
            Level = levelViewModel;
            SelectedTab = _selectedTab; // Reset tab

            _exportLevelCommand.RaiseCanExecuteChanged();
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
                OnPropertyChanged();
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

        private void ExportLevel(LevelViewModel levelViewModel)
        {
            Level level = levelViewModel.ToLevel();

            if (_levelFileDialog.SaveFileDialog())
            {
                FileMode mode = File.Exists(_levelFileDialog.FileName) ? FileMode.Truncate : FileMode.OpenOrCreate;
                using (Stream stream = File.Open(_levelFileDialog.FileName, mode))
                {
                    LevelFactory.SaveLevelXmlSerialized(level, stream);
                }
            }
        }

        private void OpenLevelDialog()
        {
            if (_levelFileDialog.OpenFileDialog())
            {
                foreach (string fileName in _levelFileDialog.FileNames)
                    ImportLevel(fileName);
            }
        }

        public void ImportLevel(string path)
        {
            Level level = LevelFactory.Load(path);
            LevelViewModel levelViewModel = LevelViewModel.FromLevel(level, _userFixedAssetsPath, _userAtomAssetsPath);

            Levels.Add(levelViewModel);
            ShowLevel(levelViewModel);
        }
    }
}
