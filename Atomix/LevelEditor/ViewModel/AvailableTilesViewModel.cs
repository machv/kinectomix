using System.Collections.Generic;
using Mach.Wpf.Mvvm;
using Mach.Kinectomix.LevelEditor.Model;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Mach.Kinectomix.LevelEditor.ViewModel
{
    public class AvailableTilesViewModel : NotifyPropertyBase
    {
        private Tiles _tiles;
        private ICommand _openAtomsAssetsDirectoryCommand;
        private ICommand _openFixedAssetsDirectoryCommand;

        public Tiles Tiles
        {
            get { return _tiles; }
            set
            {
                _tiles = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenAtomsAssetsDirectoryCommand
        {
            get { return _openAtomsAssetsDirectoryCommand; }
        }
        public ICommand OpenFixedAssetsDirectoryCommand
        {
            get { return _openFixedAssetsDirectoryCommand; }
        }

        public delegate void TileSelectedEventHandler(object sender, TileSelectedEventArgs e);
        public event TileSelectedEventHandler TileSelected;

        protected void OnTileSelected(BoardTileViewModel tile)
        {
            if (TileSelected != null)
                TileSelected(this, new TileSelectedEventArgs(tile));
        }

        private IEnumerable<BoardTileViewModel> _availableTiles;
        public IEnumerable<BoardTileViewModel> AvailableTiles
        {
            get { return _availableTiles; }
            set
            {
                _availableTiles = value;

                OnPropertyChanged();
            }
        }

        private BoardTileViewModel _selectedTile;
        public BoardTileViewModel SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                _selectedTile = value;

                OnTileSelected(_selectedTile);
                OnPropertyChanged();
            }
        }

        public void UpdateAvailableTiles(Tiles.TileType type)
        {
            if (_tiles == null)
            {
                AvailableTiles = null;
                return;
            }

            switch (type)
            {
                case Tiles.TileType.Board:
                    AvailableTiles = _tiles.Board;
                    break;
                case Tiles.TileType.Molecule:
                    AvailableTiles = _tiles.Molecule;
                    break;
            }
        }

        public AvailableTilesViewModel()
        {
            _openAtomsAssetsDirectoryCommand = new DelegateCommand(OpenAtomsAssetsDirectory);
            _openFixedAssetsDirectoryCommand = new DelegateCommand(OpenFixedAssetsDirectory);
        }

        public void OpenAtomsAssetsDirectory()
        {
            OpenDirectory(Properties.Settings.Default.AtomTilesDirectory);
        }

        public void OpenFixedAssetsDirectory()
        {
            OpenDirectory(Properties.Settings.Default.FixedTilesDirectory);
        }

        private void OpenDirectory(string path)
        {
            string pathToOpen = null;

            string atomsPath = path;
            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (Path.IsPathRooted(atomsPath))
            {
                if (Directory.Exists(atomsPath))
                {
                    pathToOpen = atomsPath;
                }
            }
            else
            {
                if (Directory.Exists(Path.Combine(executablePath, atomsPath)))
                {
                    pathToOpen = Path.Combine(executablePath, atomsPath);
                }
            }

            if (pathToOpen != null)
            {
                try
                {
                    Process.Start(pathToOpen);
                }
                catch
                { }
            }
        }



        public AvailableTilesViewModel(Tiles tiles)
            : this()
        {
            _tiles = tiles;
        }
    }
}
