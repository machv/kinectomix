using Microsoft.Kinect;
using System;
using System.Linq;

namespace Mach.Kinect
{
    /// <summary>
    /// Represents detected skeletons from the Kinect sensor.
    /// </summary>
    public partial class Skeletons
    {
        private long _timestamp;
        private Skeleton[] _items;
        private KinectSensor _sensor;
        private SkeletonTrackingType _trackingType;
        private int _trackedSkeletonId = -1;

        /// <summary>
        /// Gets raw <see cref="Skeleton"/>s from the <see cref="SkeletonFrame"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Skeleton"/>s from the <see cref="SkeletonFrame"/>.
        /// </value>
        public Skeleton[] Items
        {
            get { return _items; }
        }
        /// <summary>
        /// Gets or sets the strategy for tracking skeletons.
        /// </summary>
        /// <value>
        /// The strategy for tracking skeletons.
        /// </value>
        public SkeletonTrackingType SkeletonTrackingMode
        {
            get { return _trackingType; }
            set { _trackingType = value; }
        }
        /// <summary>
        /// Gets or sets the ID of the tracked skeleton.
        /// </summary>
        /// <value>
        /// The ID of the tracked skeleton.
        /// </value>
        public int TrackedSkeletonId
        {
            get { return _trackedSkeletonId; }
            set { _trackedSkeletonId = value; }
        }
        /// <summary>
        /// Gets the tracked skeleton by selected tracking strategy defined in <see cref="SkeletonTrackingMode" />.
        /// </summary>
        /// <value>
        /// The tracked skeleton.
        /// </value>
        /// <exception cref="System.InvalidOperationException">NearestForcedFullyTracked strategy works only if KinectSensor is passed via constructor.</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Skeletons"/> class.
        /// </summary>
        public Skeletons()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skeletons"/> class.
        /// </summary>
        /// <param name="trackingType">Type of the skeleton tracking.</param>
        public Skeletons(SkeletonTrackingType trackingType)
        {
            _trackingType = trackingType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skeletons"/> class.
        /// </summary>
        /// <param name="sensor">The sensor from will be skeletons detected.</param>
        /// <param name="trackingType">Type of the skeleton tracking.</param>
        public Skeletons(KinectSensor sensor, SkeletonTrackingType trackingType)
        {
            _sensor = sensor;
            _trackingType = trackingType;
        }

        /// <summary>
        /// Saves the skeleton data.
        /// </summary>
        /// <param name="skeletonData">The skeleton data from <see cref="SkeletonFrame"/>.</param>
        /// <param name="skeletonTimestamp">The time stamp of the <see cref="SkeletonFrame"/>.</param>
        public void SetSkeletonData(Skeleton[] skeletonData, long skeletonTimestamp)
        {
            _items = skeletonData;
            _timestamp = skeletonTimestamp;
        }

        /// <summary>
        /// Gets any skeleton that is tracked actively or passively.
        /// </summary>
        /// <returns>Any skeleton that is tracked actively or passively.</returns>
        public Skeleton GetAnyKnownkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked || s.TrackingState == SkeletonTrackingState.PositionOnly)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets any actively tracked skeleton.
        /// </summary>
        /// <returns>Any actively tracked skeleton.</returns>
        public Skeleton GetAnyTrackedSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the nearest skeleton that is actively or passively tracked.
        /// </summary>
        /// <returns>The nearest skeleton that is actively or passively tracked.</returns>
        public Skeleton GetNearestKnownSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked || s.TrackingState == SkeletonTrackingState.PositionOnly)
                .OrderBy(s => s.Position.Z)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the nearest actively tracked skeleton.
        /// </summary>
        /// <returns>The nearest actively tracked skeleton.</returns>
        public Skeleton GetNearestTrackedSkeleton()
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                .OrderBy(s => s.Position.Z)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the skeleton by its ID.
        /// </summary>
        /// <param name="trackingId">The ID of the tracked skeleton.</param>
        /// <returns>The skeleton by its ID.</returns>
        public Skeleton GetSkeleton(int trackingId)
        {
            if (Items == null)
                return null;

            return Items
                .Where(s => s.TrackingId == _trackedSkeletonId)
                .FirstOrDefault();
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
    }
}
