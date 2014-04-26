using Atomix.ViewModel;
using AtomixData;

namespace Atomix
{
    public class LevelFactory
    {
        public static LevelViewModel ToViewModel(Level level)
        {
            LevelViewModel levelVm = new LevelViewModel();
            levelVm.Board = new BoardCollection<BoardTileViewModel>(level.Board.RowsCount, level.Board.ColumnsCount);
            levelVm.Molecule = new BoardCollection<BoardTileViewModel>(level.Molecule.RowsCount, level.Molecule.ColumnsCount);

            foreach (BoardTile tile in level.Board)
                levelVm.Board.Add(new BoardTileViewModel(tile));

            foreach (BoardTile tile in level.Molecule)
                levelVm.Molecule.Add(new BoardTileViewModel(tile));

            return levelVm;
        }
    }
}
