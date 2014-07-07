using Atomix.Components;
using Mach.Xna.Input;
using Mach.Xna.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Xna.Input
{
    public class KinectInputProvider : IInputProvider
    {
        protected KinectCursor _cursor;

        public KinectInputProvider(KinectCursor cursor)
        {
            _cursor = cursor;
        }

        public IInputState GetState()
        {
            return new KinectInputState(_cursor);
        }
    }
}
