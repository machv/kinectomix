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
        public BoardTileCollection Board;
        public Molecule Molecule;

        public Level() { }

        public Level(int rows, int columns)
        {
            Board = new BoardTileCollection(rows, columns);
        }
    }
}
