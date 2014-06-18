using Kinectomix.Logic;
using Kinectomix.LevelEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Kinectomix.LevelEditor.Model
{
    public class Tiles
    {
        public enum TileType
        {
            Board,
            Molecule,
        }

        public enum AssetType
        {
            Atom,
            Fixed,
        }

        private ObservableCollection<BoardTileViewModel> _tiles = new ObservableCollection<BoardTileViewModel>();
        private ObservableCollection<BoardTileViewModel> _boardTiles = new ObservableCollection<BoardTileViewModel>();
        private ObservableCollection<BoardTileViewModel> _moleculeTiles = new ObservableCollection<BoardTileViewModel>();
        private Dictionary<string, TileAsset> _assets = new Dictionary<string, TileAsset>();

        public ObservableCollection<BoardTileViewModel> Items { get { return _tiles; } }

        public ObservableCollection<BoardTileViewModel> Board { get { return _boardTiles; } }

        public ObservableCollection<BoardTileViewModel> Molecule { get { return _moleculeTiles; } }

        public Dictionary<string, TileAsset> Assets { get { return _assets; } }

        public void Add(BoardTileViewModel tile, TileType destination)
        {
            if (!_tiles.Contains(tile))
            {
                _tiles.Add(tile);
            }

            switch (destination)
            {
                case TileType.Board:
                    _boardTiles.Add(tile);
                    break;
                case TileType.Molecule:
                    _moleculeTiles.Add(tile);
                    break;
            }
        }

        public BoardTileViewModel this[string code]
        {
            get { return _tiles.Where(t => t.Asset == code).FirstOrDefault(); }
        }

        public Tiles()
        {
            LoadSystemAssets();
        }

        public void LoadSystemAssets()
        {
            BoardTile tile;
            BoardTileViewModel tileVm;

            // Add default system tiles
            tile = new BoardTile() { Asset = "Clean", Name = "Clean" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))), IsClear = true };
            Add(tileVm, TileType.Board);
            Add(tileVm, TileType.Molecule);

            tile = new BoardTile() { IsFixed = true, IsEmpty = true, Asset = "Empty", Name = "Empty" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            Add(tileVm, TileType.Board);
            Add(tileVm, TileType.Molecule);
        }

        public void LoadUserAssets(AssetType type, string path)
        {
            string[] tiles = Directory.GetFiles(path, "*.png");
            foreach (string tilePath in tiles)
            {
                string tileName = Path.GetFileNameWithoutExtension(tilePath);
                BoardTile tile = new BoardTile() { IsFixed = type == AssetType.Fixed, IsEmpty = false, Asset = tileName, Name = tileName };
                BoardTileViewModel tileVm = new BoardTileViewModel(tile, tilePath);

                Add(tileVm, TileType.Board);

                if (type == AssetType.Atom)
                    Add(tileVm, TileType.Molecule);
            }
        }

        /// <summary>
        /// Loads assets from level definition and skips assets with rendered bonds (as they will be generated during saving).
        /// </summary>
        /// <param name="level"></param>
        public void LoadLevelAssets(Level level)
        {
            foreach (LevelAsset asset in level.Assets.Where(a => a.HasBonds == false))
            {
                BoardTile tile = new BoardTile() { IsFixed = asset.IsFixed, IsEmpty = false, Asset = asset.Code, Name = asset.Name };
                BoardTileViewModel tileVm = new BoardTileViewModel(tile, asset);

                Add(tileVm, TileType.Board);

                if (!asset.IsFixed)
                    Add(tileVm, TileType.Molecule);
            }
        }

        public void Clear()
        {
            _tiles.Clear();
            _boardTiles.Clear();
            _moleculeTiles.Clear();
        }
    }
}
