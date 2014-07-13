using Microsoft.Kinect;
using System;
using System.Linq;

namespace Mach.Kinect
{
    public partial class Skeletons
    {
        private long _timestamp;
        private Skeleton[] _items;
        private KinectSensor _sensor;

        public Skeleton[] Items
        {
            get { return _items; }
        }

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
                    case SkeletonTrackingType.NearestForcedFullyTracked:
                        skeleton = GetNearestKnownSkeleton();
                        if (skeleton != null && skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            if (_sensor == null)
                                throw new InvalidOperationException("NearestForcedFullyTracked strategy works only if KinectSensor is passed via constructor.");

                            if (!_sensor.SkeletonStream.AppChoosesSkeletons)
                            {
                                _sensor.SkeletonStream.AppChoosesSkeletons = true;
                            }

                            _sensor.SkeletonStream.ChooseSkeletons(skeleton.TrackingId);
                        }

                        return skeleton;
                    case SkeletonTrackingType.First:
                        return GetAnyKnownkeleton();
                    case SkeletonTrackingType.FirstFullyTracked:
                        return GetAnyTrackedSkeleton();
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

        public Skeletons(KinectSensor sensor, SkeletonTrackingType trackingType)
        {
            _sensor = sensor;
            _trackingType = trackingType;
        }

        public void SetSkeletonData(Skeleton[] skeletonData, long skeletonTimestamp)
        {
            _items = skeletonData;
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
