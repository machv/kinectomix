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
        public List<LevelAsset> Assets { get; set; }

        public string Name { get; set; }

        public BoardCollection<BoardTile> Board { get; set; }
        public BoardCollection<BoardTile> Molecule { get; set; }

        public Level()
        {
            Assets = new List<LevelAsset>();
        }

        public Level(int rows, int columns) : this()
        {
            Board = new BoardCollection<BoardTile>(rows, columns);

        }
    }
}
