using AtomixData;
using Kinectomix.LevelGenerator.Model;
using Kinectomix.LevelGenerator.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

                RaisePropertyChangedEvent("AvailableTiles");
            }
        }

        private BoardTile _currentTileTemplate;
        public BoardTile CurrentTileTemplate
        {
            get { return _currentTileTemplate; }
            set
            {
                _currentTileTemplate = value;

                RaisePropertyChangedEvent("CurrentTileTemplate");
            }
        }

        private BoardTileViewModel _currentTile;
        public BoardTileViewModel CurrentTile
        {
            get { return _currentTile; }
            set
            {
                _currentTile = value;

                RaisePropertyChangedEvent("CurrentTile");

                UpdateCurrentTileByTemplate(CurrentTile);
            }
        }

        private void UpdateCurrentTileByTemplate(BoardTileViewModel _currentTile)
        {
            if (CurrentTileTemplate == null)
                return;

            _currentTile.Type = CurrentTileTemplate.Type;
        }

        private LevelViewModel _level;
        public LevelViewModel Level
        {
            get { return _level; }
            set
            {
                _level = value;

                RaisePropertyChangedEvent("Level");
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
                RaisePropertyChangedEvent("SelectedTab");
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
        }

        public ICommand LoadLevelCommand
        {
            get { return new DelegateCommand(LoadLevel); }
        }

        public ICommand SaveLevelCommand
        {
            get { return new DelegateCommand(SaveLevel); }
        }

        private void SaveLevel()
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

        private void LoadLevel()
        {
            if (_levelFileDialog.OpenFileDialog())
            {
                Level level = LevelFactory.Load(_levelFileDialog.FileName);
                Level = LevelViewModel.FromLevel(level);
            }
        }
    }
}
