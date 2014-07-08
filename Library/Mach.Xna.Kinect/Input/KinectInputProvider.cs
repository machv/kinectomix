using Mach.Xna.Kinect.Components;

namespace Mach.Xna.Input
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
