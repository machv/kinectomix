using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AtomixData
{
    public static class Bresenham
    {
        private static void Swap<T>(ref T first, ref T second)
        {
            T temp;
            temp = first;
            first = second;
            second = temp;
        }

        public static IEnumerable<Point> GetLinePoints(Point start, Point end)
        {
            bool isSteep = Math.Abs(end.Y - start.Y) > Math.Abs(end.X - end.Y);
            if (isSteep)
            {
                Swap<int>(ref start.X, ref start.Y);
                Swap<int>(ref end.X, ref end.Y);
            }

            if (start.X > end.X)
            {
                Swap<Point>(ref start, ref end);
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
     //   function line(x0, y0, x1, y1)
     //boolean steep := abs(y1 - y0) > abs(x1 - x0)
     //if steep then
     //    swap(x0, y0)
     //    swap(x1, y1)
     //if x0 > x1 then
     //    swap(x0, x1)
     //    swap(y0, y1)
     //int deltax := x1 - x0
     //int deltay := abs(y1 - y0)
     //int error := deltax / 2
     //int ystep
     //int y := y0
     //if y0<y1 then ystep := 1 else ystep := -1
     //for x from x0 to x1
     //    if steep then plot(y,x) else plot(x, y)
     //    error := error - deltay
     //    if error< 0 then
     //        y := y + ystep
     //        error := error + deltax
    }
}
