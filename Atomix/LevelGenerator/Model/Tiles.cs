﻿using Kinectomix.Logic;
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

        public BoardTileViewModel this[string name]
        {
            get { return _tiles.Where(t => t.Asset == name).FirstOrDefault(); }
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
            tile = new BoardTile() { IsFixed = true, IsEmpty = true, Asset = "Empty" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            Add(tileVm, TileType.Board);
            Add(tileVm, TileType.Molecule);

            tile = new BoardTile() { IsFixed = true, IsEmpty = false, Asset = "Wall" };
            tileVm = new BoardTileViewModel(tile) { AssetSource = BitmapFrame.Create(new Uri(string.Format("pack://application:,,,/Board/{0}.png", tile.Asset))) };
            Add(tileVm, TileType.Board);
        }

        public void LoadUserAssets(string path)
        {
            string[] tiles = Directory.GetFiles(path, "*.png");
            foreach (string tilePath in tiles)
            {
                string tileName = Path.GetFileNameWithoutExtension(tilePath);
                BoardTile tile = new BoardTile() { IsFixed = false, IsEmpty = false, Asset = tileName };
                BoardTileViewModel tileVm = new BoardTileViewModel(tile, tilePath);

                Add(tileVm, TileType.Board);
                Add(tileVm, TileType.Molecule);
            }
        }

        public void LoadLevelAssets(Level level)
        {
            foreach (LevelAsset asset in level.Assets.Where(a => a.HasBonds == false))
            {
                BoardTile tile = new BoardTile() { IsFixed = false, IsEmpty = false, Asset = asset.AssetName };
                BoardTileViewModel tileVm = new BoardTileViewModel(tile, asset);

                Add(tileVm, TileType.Board);
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
