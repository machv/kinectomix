using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using System.ComponentModel;

namespace AtomixData
{
    public class Level
    {
        public string Name { get; set; }

        public BoardCollection<BoardTile> Board { get; set; }
        public BoardCollection<BoardTile> Molecule { get; set; }

        public Level() { }

        public Level(int rows, int columns)
        {
            Board = new BoardCollection<BoardTile>(rows, columns);
        }
    }
}
