using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinectomix.GestureRecorder.Model
{
    /// <summary>
    /// This class is used to map points between skeleton and color/depth
    /// </summary>
    public class JointMapping
    {
        /// <summary>
        /// Gets or sets the joint at which we we are looking
        /// </summary>
        public Joint Joint { get; set; }

        /// <summary>
        /// Gets or sets the point mapped into the target display
        /// </summary>
        public Point MappedPoint { get; set; }
    }
}
