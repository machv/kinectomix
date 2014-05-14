using Kinectomix.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.ViewModel
{
    public class LevelViewModel
    {
        public TilesCollection<BoardTileViewModel> Board { get; set; }

        public TilesCollection<BoardTileViewModel> Molecule { get; set; }

        public bool CanGoUp(int x, int y)
        {
            if (x > 0 && Board[x - 1, y].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoDown(int x, int y)
        {
            if (x + 1 < Board.RowsCount && Board[x + 1, y].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoLeft(int x, int y)
        {
            if (y > 0 && Board[x, y - 1].IsEmpty)
                return true;

            return false;
        }

        public bool CanGoRight(int x, int y)
        {
            if (y + 1 < Board.ColumnsCount && Board[x, y + 1].IsEmpty)
                return true;

            return false;
        }
    }
}
