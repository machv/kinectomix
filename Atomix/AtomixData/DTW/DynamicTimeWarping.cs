using Kinectomix.Logic.DTW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.DTW
{
    public class DynamicTimeWarping
    {
        public static double EuclidianDistance(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public static double EuclidianDistance(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public static double AccumulatedEuclidianDistance(GestureFrame a, GestureFrame b, GestureTrackingDimension dimension)
        {
            double accumulatedDistance = 0;

            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            {
                double x = a[i].X - b[i].X;
                double y = a[i].Y - b[i].Y;
                double z = a[i].Z - b[i].Z;

                accumulatedDistance += dimension == GestureTrackingDimension.Two ? 
                    EuclidianDistance(x, y) : 
                    EuclidianDistance(x, y, z);
            }

            return accumulatedDistance;
        }

        public static double Minimum(params double[] values)
        {
            return Enumerable.Min(values);
        }

        public static double CalculateDtw(Gesture gesture1, Gesture gesture2)
        {
            double[,] matrix = new double[gesture1.GestureSequence.Count + 1, gesture2.GestureSequence.Count + 1];

            for (int i = 1; i <= gesture1.GestureSequence.Count; i++)
                matrix[i, 0] = double.PositiveInfinity;

            for (int i = 1; i <= gesture2.GestureSequence.Count; i++)
                matrix[0, i] = double.PositiveInfinity;

            matrix[0, 0] = 0;

            for (int i = 1; i < gesture1.GestureSequence.Count + 1; i++)
            {
                for (int j = 1; j < gesture2.GestureSequence.Count + 1; j++)
                {
                    matrix[i, j] = AccumulatedEuclidianDistance(gesture1.GestureSequence[i - 1], gesture2.GestureSequence[j - 1], gesture1.Dimension) +
                        Minimum(matrix[i - 1, j],
                                matrix[i, j - 1],
                                matrix[i - 1, j - 1]);
                }
            }

            return matrix[gesture1.GestureSequence.Count, gesture2.GestureSequence.Count];
        }
    }
}
