﻿using AtomixData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator
{
    public class BoardTileViewModel : Mvvm.NotifyPropertyBase
    {
        BoardTile _tile;

        public BoardTile Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public TileType Type
        {
            get { return _tile.Type; }
            set
            {
                _tile.Type = value;
                RaisePropertyChangedEvent();
            }
        }

        public bool IsFixed
        {
            get { return _tile.IsFixed; }
            set
            {
                _tile.IsFixed = value;
                RaisePropertyChangedEvent();
            }
        }

        public BoardTileViewModel(BoardTile tile)
        {
            _tile = tile;
        }
    }
}
