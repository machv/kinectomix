using System;
using System.Windows.Media;

namespace Mach.Kinectomix.LevelEditor.Model
{
    public class TileAsset
    {
        public string Name { get; set; }
        public ImageSource Source { get; set; }
        public string FilePath { get; set; }
        public bool IsResource { get; set; }
        public Uri Uri { get; set; }
    }
}