using Kinectomix.Logic;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Atomix.ViewModel
{
    /// <summary>
    /// View Model for level definition.
    /// </summary>
    public class LevelViewModel
    {
        private Level _level;

        /// <summary>
        /// Gets <see cref="Level"/> model for which is this View Model created.
        /// </summary>
        /// <returns>Model</returns>
        public Level Level
        {
            get { return _level; }
        }

        /// <summary>
        /// Gets or sets friendly name of the level.
        /// </summary>
        /// <returns>Friendly name of the level.</returns>
        public string Name
        {
            get { return _level.Name; }
            set { _level.Name = value; }
        }

        public TilesCollection<BoardTileViewModel> Board { get; set; }

        public TilesCollection<BoardTileViewModel> Molecule { get; set; }

        public Dictionary<string, Texture2D> Assets { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="LevelViewModel"/>.
        /// </summary>
        /// <param name="level">Model class for this View Model.</param>
        public LevelViewModel(Level level)
        {
            _level = level;

            if (level.Board != null)
            {
                Board = new TilesCollection<BoardTileViewModel>(level.Board.RowsCount, level.Board.ColumnsCount);

                foreach (BoardTile tile in level.Board)
                {
                    BoardTileViewModel tileVm = tile != null ? new BoardTileViewModel(tile) : null;
                    Board.Add(tileVm);
                }
            }

            if (level.Molecule != null)
            {
                Molecule = new TilesCollection<BoardTileViewModel>(level.Molecule.RowsCount, level.Molecule.ColumnsCount);

                foreach (BoardTile tile in level.Molecule)
                {
                    BoardTileViewModel tileVm = tile != null ? new BoardTileViewModel(tile) : null;
                    Molecule.Add(tileVm);
                }
            }
        }

        public bool CanGoUp(int x, int y)
        {
            if (x > 0 && Board[x - 1, y] != null && Board[x - 1, y].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoDown(int x, int y)
        {
            if (x + 1 < Board.RowsCount && Board[x + 1, y] != null && Board[x + 1, y].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoLeft(int x, int y)
        {
            if (y > 0 && Board[x, y - 1] != null && Board[x, y - 1].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoRight(int x, int y)
        {
            if (y + 1 < Board.ColumnsCount && Board[x, y + 1] != null && Board[x, y + 1].IsEmpty)
                return true;

            return false;
        }

        public static LevelViewModel FromModel(Level level, GraphicsDevice graphicsDevice)
        {
            LevelViewModel levelVm = new LevelViewModel(level);

            // Load assets
            levelVm.Assets = new Dictionary<string, Texture2D>();
            foreach (var asset in level.Assets)
            {
                using (Stream stream = new MemoryStream(asset.DecodedAssetContent))
                {
                    Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);

                    string key = asset.Code != null ? asset.Code : asset.Name;
                    levelVm.Assets.Add(key, texture);
                }
            }

            return levelVm;
        }
    }
}
