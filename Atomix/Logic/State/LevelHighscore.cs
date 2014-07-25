using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
{
    /// <summary>
    /// Manages high score of one level.
    /// </summary>
    [Serializable]
    public class LevelHighscore
    {
        private TimeSpan _time;
        private int _movesCount;
        private DateTime _when;

        /// <summary>
        /// Gets or sets the time needed to finish the level.
        /// </summary>
        /// <value>
        /// The time needed to finish the level.
        /// </value>
        [XmlIgnore]
        public TimeSpan Time
        {
            get { return _time; }
            set { _time = value; }
        }
        /// <summary>
        /// Gets or sets the time ticks needed to finish the level.
        /// </summary>
        /// <value>
        /// The time ticks needed to finish the level.
        /// </value>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("Time")]
        public long TimeTicks
        {
            get { return _time.Ticks; }
            set { _time = new TimeSpan(value); }
        }
        /// <summary>
        /// Gets or sets the count of moves needed to finish the level.
        /// </summary>
        /// <value>
        /// The count of moves needed to finish the level.
        /// </value>
        public int Moves
        {
            get { return _movesCount; }
            set { _movesCount = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when high score was updated.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime"/> when high score was updated.
        /// </value>
        public DateTime When
        {
            get { return _when; }
            set { _when = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelHighscore"/> class.
        /// </summary>
        public LevelHighscore()
        {
            _movesCount = int.MaxValue;
        }

        /// <summary>
        /// Updates the high score of this level with current results if they are better.
        /// </summary>
        /// <param name="moves">The new for moves.</param>
        /// <param name="time">The new value for time.</param>
        /// <returns></returns>
        public bool UpdateIfBetter(int moves, TimeSpan time)
        {
            bool isUpdated = false;

            // If no time is specified -> update
            if (_time == TimeSpan.Zero)
            {
                _time = time;
                isUpdated = true;
            }

            // If moves count is the same and time is better -> update
            if (moves == _movesCount && time < _time)
            {
                _time = time;
                isUpdated = true;
            }

            // If moves count is better -> update
            if (moves < _movesCount)
            {
                _movesCount = moves;
                _time = time;

                isUpdated = true;
            }

            if (isUpdated)
                _when = DateTime.Now;

            return isUpdated;
        }
    }
}
