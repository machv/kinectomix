using Microsoft.Kinect;
using System.Linq;

namespace Mach.Kinect
{
    public class Skeletons
    {
        private long _timestamp;

        public enum SkeletonType
        {
            /// <summary>
            /// The skeleton that is first returned from the sensor.
            /// </summary>
            First,
            /// <summary>
            /// The nearest skeleton that has at least known position from the sensor.
            /// </summary>
            Nearest,
            /// <summary>
            /// The nearest fully tracked skeleton from the sensor.
            /// </summary>
            NearestFullyTracked,
            /// <summary>
            /// The skeleton with corresponding ID.
            /// </summary>
            FixById,
            /// <summary>
            /// The skeleton with corresponding ID and if that ID is not tracked it tries to find first alternative with at least known position.
            /// </summary>
            FixByIdOrFirst,
            /// <summary>
            /// The skeleton with corresponding ID and if that ID is not tracked it tries to find first fully tracked alternative.
            /// </summary>
            FixByIdOrFirstFullyTracked,
        }

        public Skeleton[] Items { get; private set; }

        private SkeletonType _trackingType;
        public SkeletonType TrackingType
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
                    case SkeletonType.Nearest:
                        return GetNearestKnownSkeleton();
                    case SkeletonType.NearestFullyTracked:
                        return GetNearestTrackedSkeleton();
                    case SkeletonType.FixById:
                        return GetSkeleton(_trackedSkeletonId);
                    case SkeletonType.FixByIdOrFirst:
                        skeleton = GetSkeleton(_trackedSkeletonId);
                        if (skeleton == null)
                        {
                            FixToFirstKnownSkeleton();
                            return GetSkeleton(_trackedSkeletonId);
                        }

                        return skeleton;
                    case SkeletonType.FixByIdOrFirstFullyTracked:
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

        public Skeletons(SkeletonType trackingType)
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
