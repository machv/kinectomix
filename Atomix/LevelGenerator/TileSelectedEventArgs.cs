﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinectomix.LevelGenerator
{
    public class TileSelectedEventArgs : RoutedEventArgs
    {
        public AtomixData.BoardTile Tile { get; protected set; }

        public TileSelectedEventArgs() : base() { }
        
        public TileSelectedEventArgs(RoutedEvent routedEvent, AtomixData.BoardTile tile)
            : base(routedEvent)
        {
            Tile = tile;
        }
    }
}
