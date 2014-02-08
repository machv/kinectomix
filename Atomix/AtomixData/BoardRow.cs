﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    [Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }

    public class BoardTile : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            // zjistíme, zda je někdo k události přihlášen
            // musí existovat nějaký delegát, který bude event zpracovávat
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private TileType type;
        public TileType Type
        {
            get { return type; }
            set
            {
                type = value; OnPropertyChanged("Type");
            }
        }
        public bool IsFixed { get; set; }
        public bool IsSelected { get; set; }
        public Direction Movements;
        public Vector2 RenderPosition;

        public BoardTile()
        {
            IsFixed = true;
        }
    }

    public class BoardRow
    {
        public BoardTile[] Columns;

        public BoardRow() { }

        public BoardRow(int columns)
        {
            Columns = new BoardTile[columns];
        }
    }
}
