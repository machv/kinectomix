using Kinectomix.Logic;
using Kinectomix.LevelEditor.Model;
using Kinectomix.LevelEditor.Mvvm;
using System.IO;
using System.Windows.Input;

namespace Kinectomix.LevelEditor.ViewModel
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

        private string _userAtomAssetsPath;
        private string _userFixedAssetsPath;

        public EditorViewModel()
        {
            _userAtomAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.AtomTilesDirectory);
            _userFixedAssetsPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.FixedTilesDirectory);
            _tiles = new Tiles();
            _tiles.LoadUserAssets(Tiles.AssetType.Fixed, _userFixedAssetsPath);
            _tiles.LoadUserAssets(Tiles.AssetType.Atom, _userAtomAssetsPath);

            _levelFileDialog = new LevelFileDialog();
            _saveAsLevelCommand = new DelegateCommand(SaveAsLevel, CanExecuteSaveAs);

            _tileSelector = new AvailableTilesViewModel(_tiles);
            _tileSelector.TileSelected += Selector_TileSelected;
        }

        private void Selector_TileSelected(object sender, TileSelectedEventArgs e)
        {
            if (Level.Board != null)
                Level.Board.PaintTile = e.Tile;

            if (Level.Molecule != null)
                Level.Molecule.PaintTile = e.Tile;
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
            //TODO check if field changed? if yes, allow save before new.

            LevelViewModel level = new LevelViewModel();
            level.Board = new BoardViewModel(Properties.Settings.Default.DefaultBoardRows, Properties.Settings.Default.DefaultBoardColumns);
            level.Board.EmptyTileTemplate = _tiles["Empty"];
            level.Board.PopulateEmptyTiles();

            level.Molecule = new BoardViewModel(Properties.Settings.Default.DefaultMoleculeRows, Properties.Settings.Default.DefaultMoleculeColumns);
            level.Molecule.EmptyTileTemplate = _tiles["Empty"];
            level.Molecule.PopulateEmptyTiles();

            _tiles.Clear();
            _tiles.LoadSystemAssets();
            _tiles.LoadUserAssets(Tiles.AssetType.Fixed, _userFixedAssetsPath);
            _tiles.LoadUserAssets(Tiles.AssetType.Atom, _userAtomAssetsPath);

            Level = level;

            // Reset to Board tab
            SelectedTab = 0;

            _saveAsLevelCommand.RaiseCanExecuteChanged();
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

            _tiles.Clear();
            _tiles.LoadSystemAssets();
            _tiles.LoadLevelAssets(level);
            _tiles.LoadUserAssets(Tiles.AssetType.Fixed, _userFixedAssetsPath);
            _tiles.LoadUserAssets(Tiles.AssetType.Atom, _userAtomAssetsPath);

            Level = LevelViewModel.FromLevel(level, _tiles);
            SelectedTab = 0;

            _saveAsLevelCommand.RaiseCanExecuteChanged();
        }
    }
}
