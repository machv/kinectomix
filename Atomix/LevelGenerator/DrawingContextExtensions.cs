﻿using KinectomixLogic;
using System.Windows;
using System.Windows.Media;

namespace Kinectomix.LevelGenerator
{
    public static class DrawingContextExtensions
    {
        public static void DrawBond(this DrawingContext drawingContext, Size Dimensions, BondType type, int angle)
        {
            int arity = (int)type;
            if (arity > 0)
            {
                drawingContext.PushTransform(new RotateTransform(angle, Dimensions.Width / 2, Dimensions.Height / 2));

                double penWidth = 2;
                double gap = 2;
                Pen pen = new Pen(new SolidColorBrush(Colors.DarkGray), penWidth);
                double rel = Dimensions.Width - Dimensions.Height * Dimensions.Width;
                double centerY = Dimensions.Height / 2;
                double width = arity * penWidth + (arity - 1) * gap;
                double start = Dimensions.Width / 2 - (width / 2);

                for (int i = 0; i < arity; i++)
                {
                    Point point1 = new Point(start, rel);
                    Point point2 = new Point(start, centerY);
                    if (angle > 90)
                    {
                        point1.X += penWidth;
                        point2.X += penWidth;
                    }
                    drawingContext.DrawLine(pen, point1, point2);

                    start += penWidth + gap;
                }

                drawingContext.Pop();
            }
        }
    }
}
