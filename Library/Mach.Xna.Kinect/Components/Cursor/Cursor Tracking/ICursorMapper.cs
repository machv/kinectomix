using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Mach.Xna.Kinect.Components
{
    /// <summary>
    /// Maps cursor position to defined rectangle.
    /// </summary>
    public interface ICursorMapper
    {
        /// <summary>
        /// Gets mapped cursor position with respect to bounding limits based current <see cref="Skeleton"/> data.
        /// </summary>
        /// <param name="skeleton">Skeleton data to parse.</param>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="width">Width of the mapping area.</param>
        /// <param name="height">Height of the mapping area.</param>
        /// <returns>Mapped cursor position.</returns>
        Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height);
        /// <summary>
        /// Gets mapped cursor position with respect to bounding limits based current <see cref="Skeleton"/> data.
        /// </summary>
        /// <param name="skeleton">Skeleton data to parse.</param>
        /// <param name="leftHanded">True if left hand is tracked.</param>
        /// <param name="width">Width of the mapping area.</param>
        /// <param name="height">Height of the mapping area.</param>
        /// <param name="isHandTracked">Output parameter containing true if selected hand is active.</param>
        /// <returns>Mapped cursor position.</returns>
        Vector2 GetCursorPosition(Skeleton skeleton, bool leftHanded, int width, int height, out bool isHandTracked);
    }
}
