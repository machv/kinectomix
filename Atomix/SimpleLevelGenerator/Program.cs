using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace SimpleLevelGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            Level level = new Level(5, 5);

            for (int y = 0; y < level.RowsCount; y++)
            {
                BoardRow row = new BoardRow(level.ColumnsCount);
                for (int x = 0; x < level.ColumnsCount; x++)
                {
                    BoardTile tile = new BoardTile();
                    tile.Type = TileType.Wall;
                    
                    row.Columns[x] = tile;
                }

                level.Rows[y] = row;
            }

            // Save definition
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create("Level1.xml", settings))
            {
                IntermediateSerializer.Serialize(writer, level, null);
            }

        }
    }
}
