using Atomix.ViewModel;
using Kinectomix.Logic;

namespace Atomix
{
    public class LevelFactory
    {
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
