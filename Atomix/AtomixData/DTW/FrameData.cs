using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Logic.DTW
{
    public class FrameData
    {
        public SkeletonPoint[] SkeletonJoints { get; set; }

        public int Capacity { get; private set; }

        public FrameData(int capacity)
        {
            Capacity = capacity;
            SkeletonJoints = new SkeletonPoint[capacity];
        }
    }
}
