using Mach.Kinectomix.LevelEditor.ViewModel;
using System;

namespace Mach.Kinectomix.LevelEditor
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
