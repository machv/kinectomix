﻿using Microsoft.Kinect;
using System.Linq;

namespace Mach.Kinect
{
    public class Skeletons
    {
        private long _timestamp;

        public enum TrackingType { Nearest, FixById }

        public Skeleton[] Items { get; private set; }

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

        public void SetSkeletonData(Skeleton[] skeletonData, long skeletonTimestamp)
        {
            Items = skeletonData;
            _timestamp = skeletonTimestamp;
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
            if (Items == null)
                return null;

            return Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
        }

        public Skeleton GetNearestTrackedSkeleton()
        {
            if (Items == null)
                return null;

            return Items.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).OrderBy(s => s.Position.Z).FirstOrDefault();
        }

        public Skeleton GetSkeleton(int trackingId)
        {
            if (Items == null)
                return null;

            return Items.Where(s => s.TrackingId == trackedId).FirstOrDefault();
        }
    }
}