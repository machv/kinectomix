﻿using Mach.Xna.Kinect.Components;

namespace Mach.Xna.Input
{
    public struct KinectInputState : IInputState
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public bool IsStateActive { get; private set; }

        public KinectInputState(KinectCursor cursor)
            : this()
        {
            X = (int)cursor.HandPosition.X;
            Y = (int)cursor.HandPosition.Y;
            IsStateActive = cursor.IsHandClosed; // || _cursor.IsHandPressed; 
        }
    }
}