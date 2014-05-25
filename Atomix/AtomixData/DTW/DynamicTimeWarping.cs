using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.DynamicTimeWarping
{
    public class DynamicTimeWarping
    {
        public static double Distance(double a, double b)
        {
            return Math.Abs(a - b);
        }

        public static double Minimum(params double[] values)
        {
            return Enumerable.Min(values);
        }

        public static double CalculateDtw(double[] sequence1, double[] sequence2)
        {
            double[,] matrix = new double[sequence1.Length + 1, sequence2.Length + 1];

            for (int i = 1; i <= sequence1.Length; i++)
                matrix[i, 0] = double.PositiveInfinity;

            for (int i = 1; i <= sequence2.Length; i++)
                matrix[0, i] = double.PositiveInfinity;

            matrix[0, 0] = 0;

            for (int i = 1; i <= sequence1.Length; i++)
            {
                for (int j = 1; j <= sequence2.Length; j++)
                {
                    matrix[i, j] = Distance(sequence1[i], sequence2[j]) +
                        Minimum(matrix[i - 1, j],
                                matrix[i, j - 1],
                                matrix[i - 1, j - 1]);
                }
            }

            return matrix[sequence1.Length, sequence2.Length];
        }

        /*
        int DTWDistance(s: array[1..n], t: array[1..m])
        {
            DTW:= array[0..n, 0..m]

    for i := 1 to n
        DTW[i, 0] := infinity
    for i := 1 to m
        DTW[0, i] := infinity
    DTW[0, 0] := 0

    for i := 1 to n
        for j := 1 to m
            cost:= d(s[i], t[j])
            DTW[i, j] := cost + minimum(DTW[i - 1, j],    // insertion
                                        DTW[i, j - 1],    // deletion
                                        DTW[i - 1, j - 1])    // match

    return DTW[n, m]
}*/
    }
}
