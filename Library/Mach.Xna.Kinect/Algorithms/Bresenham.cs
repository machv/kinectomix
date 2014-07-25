using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Mach.Xna.Kinect.Algorithms
{
    /// <summary>
    /// Contains implementation of Bresenham's line algorithm for integer rendering.
    /// </summary>
    public static class Bresenham
    {
        private static void Swap<T>(ref T first, ref T second)
        {
            T temp;
            temp = first;
            first = second;
            second = temp;
        }

        /// <summary>
        /// Gets the integer points for line between start and end points.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public static IEnumerable<Point> GetLinePoints(Point start, Point end)
        {
            bool isSteep = Math.Abs(end.Y - start.Y) > Math.Abs(end.X - end.Y);
            if (isSteep)
            {
                Swap(ref start.X, ref start.Y);
                Swap(ref end.X, ref end.Y);
            }

            if (start.X > end.X)
            {
                Swap(ref start, ref end);
            }

            int deltaX = end.X - start.X;
            int deltaY = Math.Abs(end.Y - start.Y);
            int error = deltaX / 2;
            int y = start.Y;
            int yStep = start.Y < end.Y ? 1 : -1;

            for (int x = start.X; x < end.X; x++)
            {
                if (isSteep)
                    yield return new Point(y, x);
                else
                    yield return new Point(x, y);

                error -= deltaY;
                if (error < 0)
                {
                    y += yStep;
                    error += deltaX;
                }
            }
        }
    }
}
