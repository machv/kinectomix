using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class Skeletons
    {
        public enum TrackingType { Nearest, FixById }

        public Skeleton[] Items { get; set; }

        public TrackingType Tracking { get; set; }

        private int trackedId = -1;
        public Skeleton TrackedSkeleton
        {
            get
            {
                if (Tracking == TrackingType.FixById)
                {
                    if (trackedId == -1)
                    {
                        FixToFirstTrackedSkeleton();
                    }

                    Skeleton s = GetSkeleton(trackedId);
                    if (s == null)
                    {
                        FixToFirstTrackedSkeleton();
                        return GetSkeleton(trackedId);
                    }

                    return s;
                }
                else
                {
                    return GetNearestTrackedSkeleton();
                }
            }
        }

        private void FixToFirstTrackedSkeleton()
        {
            Skeleton skeleton = GetAnyTrackedSkeleton();
            if (skeleton != null)
            {
                trackedId = skeleton.TrackingId;
            }
        }

        public Skeleton GetAnyTrackedSkeleton()
        {
            return Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
        }

        public Skeleton GetNearestTrackedSkeleton()
        {
            return Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).OrderBy(s => s.Position.Z).FirstOrDefault();
        }

        public Skeleton GetSkeleton(int trackingId)
        {
            return Items.Where(s => s.TrackingId == trackedId).FirstOrDefault();
        }
    }
}
