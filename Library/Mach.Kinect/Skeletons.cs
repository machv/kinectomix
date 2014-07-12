using Microsoft.Kinect;
using System.Linq;

namespace Mach.Kinect
{
    public partial class Skeletons
    {
        private long _timestamp;

        public Skeleton[] Items { get; private set; }

        private SkeletonTrackingType _trackingType;
        public SkeletonTrackingType SkeletonTrackingMode
        {
            get { return _trackingType; }
            set { _trackingType = value; }
        }

        private int _trackedSkeletonId = -1;

        public int TrackedSkeletonId
        {
            get { return _trackedSkeletonId; }
            set { _trackedSkeletonId = value; }
        }

        public Skeleton TrackedSkeleton
        {
            get
            {
                Skeleton skeleton;

                switch (_trackingType)
                {
                    case SkeletonTrackingType.Nearest:
                        return GetNearestKnownSkeleton();
                    case SkeletonTrackingType.NearestFullyTracked:
                        return GetNearestTrackedSkeleton();
                    case SkeletonTrackingType.FixById:
                        return GetSkeleton(_trackedSkeletonId);
                    case SkeletonTrackingType.FixByIdOrFirst:
                        skeleton = GetSkeleton(_trackedSkeletonId);
                        if (skeleton == null)
                        {
                            FixToFirstKnownSkeleton();
                            return GetSkeleton(_trackedSkeletonId);
                        }

                        return skeleton;
                    case SkeletonTrackingType.FixByIdOrFirstFullyTracked:
                        skeleton = GetSkeleton(_trackedSkeletonId);
                        if (skeleton == null)
                        {
                            FixToFirstTrackedSkeleton();
                            return GetSkeleton(_trackedSkeletonId);
                        }

                        return skeleton;
                }

                return null;
            }
        }

        public Skeletons()
        {

        }

        public Skeletons(SkeletonTrackingType trackingType)
        {
            _trackingType = trackingType;
        }

        public void SetSkeletonData(Skeleton[] skeletonData, long skeletonTimestamp)
        {
            Items = skeletonData;
            _timestamp = skeletonTimestamp;
        }

        private void FixToFirstKnownSkeleton()
        {
            Skeleton skeleton = GetAnyKnownkeleton();
            if (skeleton != null)
            {
                _trackedSkeletonId = skeleton.TrackingId;
            }
        }

        private void FixToFirstTrackedSkeleton()
        {
            Skeleton skeleton = GetAnyTrackedSkeleton();
            if (skeleton != null)
            {
                _trackedSkeletonId = skeleton.TrackingId;
            }
        }

        public Skeleton GetAnyKnownkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked || s.TrackingState == SkeletonTrackingState.PositionOnly)
                .FirstOrDefault();
        }

        public Skeleton GetAnyTrackedSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                .FirstOrDefault();
        }

        public Skeleton GetNearestKnownSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked || s.TrackingState == SkeletonTrackingState.PositionOnly)
                .OrderBy(s => s.Position.Z)
                .FirstOrDefault();
        }

        public Skeleton GetNearestTrackedSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                .OrderBy(s => s.Position.Z)
                .FirstOrDefault();
        }

        public Skeleton GetSkeleton(int trackingId)
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingId == _trackedSkeletonId)
                .FirstOrDefault();
        }
    }
}
