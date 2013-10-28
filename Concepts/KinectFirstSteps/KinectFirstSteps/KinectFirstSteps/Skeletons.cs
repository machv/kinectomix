using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectFirstSteps
{
    public class Skeletons
    {
        public Skeleton[] Items { get; set; }

        public Skeleton TrackedSkeleton { get; set; }
    }
}
