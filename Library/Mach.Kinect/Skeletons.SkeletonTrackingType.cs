﻿namespace Mach.Kinect
{
    public partial class Skeletons
    {
        public enum SkeletonTrackingType
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
    }
}
