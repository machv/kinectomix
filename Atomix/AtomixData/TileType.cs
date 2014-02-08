using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public enum TileType
    {
        [TileProperties(ShowInBoardEditor = true, ShowInMoleculeEditor = false, IsFixed = true)]
        Empty,
        [TileProperties(ShowInBoardEditor = true, ShowInMoleculeEditor = false, IsFixed = true)]
        Wall,
        [TileProperties(ShowInBoardEditor = false, ShowInMoleculeEditor = false)]
        Up,
        [TileProperties(ShowInBoardEditor = false, ShowInMoleculeEditor = false)]
        Down,
        [TileProperties(ShowInBoardEditor = false, ShowInMoleculeEditor = false)]
        Left,
        [TileProperties(ShowInBoardEditor = false, ShowInMoleculeEditor = false)]
        Right,
        [TileProperties(ShowInBoardEditor = true, ShowInMoleculeEditor = true)]
        Carbon,
        [TileProperties(ShowInBoardEditor = true, ShowInMoleculeEditor = true)]
        Oxygen,
        [TileProperties(ShowInBoardEditor = true, ShowInMoleculeEditor = true)]
        Hydrogen,
    }
}
