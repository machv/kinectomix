using Kinectomix.LevelGenerator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinectomix.LevelGenerator
{
    public class TileSelectedEventArgs : EventArgs
    {
        public BoardTileViewModel Tile { get; protected set; }

        public TileSelectedEventArgs() : base() { }
        
        public TileSelectedEventArgs(BoardTileViewModel tile)
            : base()
        {
            Tile = tile;
        }
    }
}
