using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Mach.Kinectomix.Logic;
using Mach.Kinectomix.LevelEditor.ViewModel;

namespace Mach.Kinectomix.LevelEditor.Model
{
    /// <summary>
    /// Provides collections of assets by their type.
    /// </summary>
    public class Tiles
    {
        /// <summary>
        /// Where can be tile used.
        /// </summary>
        public enum TileType
        {
            /// <summary>
            /// Tile that can be placed on the board.
            /// </summary>
            Board,
            /// <summary>
            /// Thile that can be used in the molecule.
            /// </summary>
            Molecule,
        }

        /// <summary>
        /// Type of the resulting graphical asset.
        /// </summary>
        public enum AssetType
        {
            /// <summary>
            /// Asset is atom.
            /// </summary>
            Atom,
            /// <summary>
            /// Asset is not movable.
            /// </summary>
            Fixed,
        }

        private ObservableCollection<BoardTileViewModel> _tiles = new ObservableCollection<BoardTileViewModel>();
        private ObservableCollection<BoardTileViewModel> _boardTiles = new ObservableCollection<BoardTileViewModel>();
        private ObservableCollection<BoardTileViewModel> _moleculeTiles = new ObservableCollection<BoardTileViewModel>();

        /// <summary>
        /// Gets the all containing tiles.
        /// </summary>
        /// <value>
        /// The all tiles.
        /// </value>
        public ObservableCollection<BoardTileViewModel> Items
        {
            get { return _tiles; }
        }

        /// <summary>
        /// Gets the tiles that can be placed on the board.
        /// </summary>
        /// <value>
        /// The board tiles.
        /// </value>
        public ObservableCollection<BoardTileViewModel> Board
        {
            get { return _boardTiles; }
        }

        /// <summary>
        /// Gets the tiles that can be used in molecule.
        /// </summary>
        /// <value>
        /// The molecule tiles.
        /// </value>
        public ObservableCollection<BoardTileViewModel> Molecule
        {
            get { return _moleculeTiles; }
        }

        /// <summary>
        /// Adds the specified tile to the collections.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        /// <param name="type">Type of the tile.</param>
        public void Add(BoardTileViewModel tile, TileType type)
        {
            if (!_tiles.Contains(tile))
            {
                _tiles.Add(tile);
            }

            switch (type)
            {
                case TileType.Board:
                    _boardTiles.Add(tile);
                    break;
                case TileType.Molecule:
                    _moleculeTiles.Add(tile);
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="BoardTileViewModel"/> with the specified code.
        /// </summary>
        /// <value>
        /// The <see cref="BoardTileViewModel"/>.
        /// </value>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public BoardTileViewModel this[string code]
        {
            get { return _tiles.Where(t => t.Asset == code).FirstOrDefault(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tiles"/> class.
        /// </summary>
        public Tiles()
        {
            LoadSystemAssets();
        }

        /// <summary>
        /// Loads the assets from resources.
        /// </summary>
        public void LoadSystemAssets()
        {
            BoardTile tile;
            BoardTileViewModel tileVm;

            // Add default system tiles
            tile = new BoardTile() { Asset = "Clean", Name = Localization.TileResources.Clean };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))), IsClear = true };
            Add(tileVm, TileType.Board);
            Add(tileVm, TileType.Molecule);

            tile = new BoardTile() { IsFixed = true, IsEmpty = true, Asset = "Empty", Name = Localization.TileResources.Empty };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            Add(tileVm, TileType.Board);
            Add(tileVm, TileType.Molecule);
        }

        /// <summary>
        /// Loads assets from local directory.
        /// </summary>
        /// <param name="type">The type of assets in specified directory.</param>
        /// <param name="path">The path of the directory containing assets.</param>
        public void LoadUserAssets(AssetType type, string path)
        {
            try
            {
                string[] tiles = Directory.GetFiles(path, "*.png");
                foreach (string tilePath in tiles)
                {
                    string tileName = Path.GetFileNameWithoutExtension(tilePath);
                    string localizedTileName = TryLocalizeTile(tileName);

                    BoardTile tile = new BoardTile() { IsFixed = type == AssetType.Fixed, IsEmpty = false, Asset = tileName, Name = localizedTileName };
                    BoardTileViewModel tileVm = new BoardTileViewModel(tile, tilePath);

                    Add(tileVm, TileType.Board);

                    if (type == AssetType.Atom)
                        Add(tileVm, TileType.Molecule);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Loads assets from level definition and skips assets with rendered bonds (as they will be generated during saving).
        /// </summary>
        /// <param name="level">Level from which will be assets loaded.</param>
        public void LoadLevelAssets(Level level)
        {
            foreach (LevelAsset asset in level.Assets.Where(a => a.HasBonds == false))
            {
                BoardTile tile = new BoardTile() { IsFixed = asset.IsFixed, IsEmpty = false, Asset = asset.Code, Name = TryLocalizeTile(asset.Name) };
                BoardTileViewModel tileVm = new BoardTileViewModel(tile, asset);
                tileVm.IsFromLevel = true;

                Add(tileVm, TileType.Board);

                if (!asset.IsFixed)
                    Add(tileVm, TileType.Molecule);
            }
        }

        /// <summary>
        /// Clears all collections containing tiles.
        /// </summary>
        public void Clear()
        {
            _tiles.Clear();
            _boardTiles.Clear();
            _moleculeTiles.Clear();
        }

        private string TryLocalizeTile(string tileName)
        {
            string tile = Localization.TileResources.ResourceManager.GetString(tileName, Localization.TileResources.Culture);
            if (tile != null)
            {
                return tile;
            }

            return tileName;
        }
    }
}
