using AtomixData;
using Kinectomix.LevelGenerator.Model;
using Kinectomix.LevelGenerator.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class EditorViewModel : NotifyPropertyBase
    {
        private Tiles _tiles;
        
        private IOpenFileService _openFileService;
        public IOpenFileService OpenFileService
        {
            get { return _openFileService; }
            set { _openFileService = value; }
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

        public EditorViewModel()
        {
            _tiles = new Tiles();
            _openFileService = new OpenLevelFile();

            _availableTiles = _tiles.Board;
        }

        public ICommand LoadLevelCommand
        {
            get { return new DelegateCommand(LoadLevel); }
        }

        private void LoadLevel()
        {
            if (_openFileService.OpenFileDialog())
            {
                string path = _openFileService.FileName;
                //TODO
                //Load(dialog.FileName);
            }
        }
    }
}
