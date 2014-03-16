using Atomix.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class KinectInputState : IInputState
    {
        protected KinectCursor _cursor;

        public int X
        {
            get { return (int)_cursor.HandPosition.X; }
        }

        public int Y
        {
            get { return (int)_cursor.HandPosition.Y; }
        }

        public bool IsSelected
        {
            get { return _cursor.IsHandClosed || _cursor.IsHandPressed; }
        }

        public KinectInputState(KinectCursor cursor)
        {
            _cursor = cursor;
        }
    }
}
