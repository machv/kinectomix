using Atomix.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Xna.Input
{
    public struct KinectInputState : IInputState
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public bool IsSelected { get; private set; }

        public KinectInputState(KinectCursor cursor)
            : this()
        {
            X = (int)cursor.HandPosition.X;
            Y = (int)cursor.HandPosition.Y;
            IsSelected = cursor.IsHandClosed; // || _cursor.IsHandPressed; 
        }
    }
}
