using Atomix.ViewModel;
using Kinectomix.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Atomix
{
    public static class LevelFactory
    {
        public static Level Load(string path)
        {
            XmlSerializer seralizer = new XmlSerializer(typeof(Level));

            using (Stream stream = TitleContainer.OpenStream(path))
                return seralizer.Deserialize(stream) as Level;
        }

        public static LevelViewModel ToViewModel(Level level, GraphicsDevice graphicsDevice)
        {
            LevelViewModel levelVm = new LevelViewModel();

            if (level.Board != null)
            {
                levelVm.Board = new TilesCollection<BoardTileViewModel>(level.Board.RowsCount, level.Board.ColumnsCount);

                foreach (BoardTile tile in level.Board)
                {
                    BoardTileViewModel tileVm = tile != null ? new BoardTileViewModel(tile) : null;
                    levelVm.Board.Add(tileVm);
                }
            }

            if (level.Molecule != null)
            {
                levelVm.Molecule = new TilesCollection<BoardTileViewModel>(level.Molecule.RowsCount, level.Molecule.ColumnsCount);

                foreach (BoardTile tile in level.Molecule)
                {
                    BoardTileViewModel tileVm = tile != null ? new BoardTileViewModel(tile) : null;
                    levelVm.Molecule.Add(tileVm);
                }
            }

            // Load assets
            levelVm.Assets = new Dictionary<string, Texture2D>();
            foreach (var asset in level.Assets)
            {
                using (Stream stream = new MemoryStream(asset.DecodedAssetContent))
                {
                    Texture2D texture = Texture2D.FromStream(graphicsDevice, stream);

                    string key = asset.AssetCode != null ? asset.AssetCode : asset.AssetName;
                    levelVm.Assets.Add(key, texture);
                }
            }

            return levelVm;
        }
    }
}
