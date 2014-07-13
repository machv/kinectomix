using Microsoft.Kinect;
using System;
using System.Linq;

namespace Mach.Kinect.Gestures
{
    /// <summary>
    /// Provides implementation of Dynamic Time Warping algorithm for comparing two recorded <see cref="Gesture"/>s.
    /// </summary>
    public static class DynamicTimeWarping
    {
        /// <summary>
        /// Calculates Euclidian distance between two <see cref="SkeletonPoint"/>s.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <param name="dimension">Tracking dimension for points.</param>
        /// <returns>Euclidian distance between <see cref="SkeletonPoint"/>s.</returns>
        public static double EuclidianDistance(SkeletonPoint point1, SkeletonPoint point2, TrackingDimension dimension)
        {
            return dimension == TrackingDimension.Two ?
                Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2)) :
                Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z - point2.Z, 2)) ;
        }

        /// <summary>
        /// Calculates Euclidian distance between two <see cref="GestureFrame"/>s.
        /// </summary>
        /// <param name="frame1">First frame.</param>
        /// <param name="frame2">Second frame.</param>
        /// <param name="dimension">Tracking dimension for points.</param>
        /// <returns>distance between two <see cref="GestureFrame"/>s.</returns>
        public static double FrameDistance(GestureFrame frame1, GestureFrame frame2, TrackingDimension dimension)
        {
            double accumulatedDistance = 0;

            for (int i = 0; i < Math.Min(frame1.Count, frame2.Count); i++)
            {
                accumulatedDistance += EuclidianDistance(frame1[i], frame2[i], dimension);
            }

            return accumulatedDistance;
        }

        /// <summary>
        /// Returns minimal value from passed values.
        /// </summary>
        /// <param name="values">Values to evaluate.</param>
        /// <returns>Minimal value.</returns>
        public static double Minimum(params double[] values)
        {
            return Enumerable.Min(values);
        }

        /// <summary>
        /// Calculates DTW distance between two <see cref="Gesture"/>s.
        /// </summary>
        /// <param name="gesture1">First gesture.</param>
        /// <param name="gesture2">Second gesture.</param>
        /// <returns>DTW distance between two <see cref="Gesture"/>s.</returns>
        public static double CalculateDistance(Gesture gesture1, Gesture gesture2)
        {
            double[,] matrix = new double[gesture1.Sequence.Count + 1, gesture2.Sequence.Count + 1];

            for (int i = 1; i <= gesture1.Sequence.Count; i++)
                matrix[i, 0] = double.PositiveInfinity;

            for (int i = 1; i <= gesture2.Sequence.Count; i++)
                matrix[0, i] = double.PositiveInfinity;

            matrix[0, 0] = 0;

            for (int i = 1; i < gesture1.Sequence.Count + 1; i++)
            {
                for (int j = 1; j < gesture2.Sequence.Count + 1; j++)
                {
                    matrix[i, j] = FrameDistance(gesture1.Sequence[i - 1], gesture2.Sequence[j - 1], gesture1.Dimension) +
                        Minimum(matrix[i - 1, j],
                                matrix[i, j - 1],
                                matrix[i - 1, j - 1]);
                }
            }

            return matrix[gesture1.Sequence.Count, gesture2.Sequence.Count];
        }
    }
}
