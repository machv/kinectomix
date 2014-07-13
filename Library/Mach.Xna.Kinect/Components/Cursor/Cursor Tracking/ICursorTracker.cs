using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Kinect.Components
{
    public interface ICursorTracker
    {
        Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height);
        Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height, out bool isHandTracked);
    }
}
