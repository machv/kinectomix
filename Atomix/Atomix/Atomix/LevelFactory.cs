using Atomix.ViewModel;
using Kinectomix.Logic;
using Microsoft.Xna.Framework;
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

        public static LevelViewModel ToViewModel(Level level)
        {
            LevelViewModel levelVm = new LevelViewModel();

            if (level.Board != null)
            {
                levelVm.Board = new TilesCollection<BoardTileViewModel>(level.Board.RowsCount, level.Board.ColumnsCount);

                foreach (BoardTile tile in level.Board)
                    levelVm.Board.Add(new BoardTileViewModel(tile));
            }

            if (level.Molecule != null)
            {
                levelVm.Molecule = new TilesCollection<BoardTileViewModel>(level.Molecule.RowsCount, level.Molecule.ColumnsCount);

                foreach (BoardTile tile in level.Molecule)
                    levelVm.Molecule.Add(new BoardTileViewModel(tile));
            }

            return levelVm;
        }
    }
}
