using Mach.Kinect.Gestures;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kinectomix.Logic.Game
{
    public class KnownGesture
    {
        public string Filename { get; set; }
        public GestureType Type { get; set; }

        [ContentSerializerIgnore]
        public Gesture Instance { get; set; }

    }

    public class KnownGestures : Collection<KnownGesture>
    {
        public KnownGestures(KnownGesture[] gestures)
        {
            Array.ForEach(gestures, g => Items.Add(g));
        }
        public KnownGesture this[GestureType type]
        {
            get
            {
                return Items.Where(g => g.Type == type).FirstOrDefault();
            }
        }
    }
}
