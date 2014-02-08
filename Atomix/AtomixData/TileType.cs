using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    public enum TileType
    {
        Carbon,
        Oxygen,
        Hydrogen,
        [TileProperties(IsFixed = true)]
        Empty,
        [TileProperties(IsFixed = true)]
        Wall,
        [TileProperties(ShowInEditor = false)]
        Up,
        [TileProperties(ShowInEditor = false)]
        Down,
        [TileProperties(ShowInEditor = false)]
        Left,
        [TileProperties(ShowInEditor = false)]
        Right,
    }
}
