using Atomix.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
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
