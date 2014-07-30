using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Kinect.Extensions
{
    /// <summary>
    /// Extension methods for SkeletonPoint. 
    /// </summary>
    public static class SkeletonPointExtensions
    {
        /// <summary>
        /// Converts SkeletonPoint to Vector3.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static Vector3 ToVector3(this SkeletonPoint point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// Converts SkeletonPoint to Vector2.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static Vector2 ToVector2(this SkeletonPoint point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
