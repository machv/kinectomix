using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Mach.Kinectomix.Logic
{
    [Serializable]
    public class LevelHighscore
    {
        private TimeSpan _time;
        private int _movesCount;
        private DateTime _when;

        [XmlIgnore]
        public TimeSpan Time
        {
            get { return _time; }
            set { _time = value; }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("Time")]
        public long TimeTicks
        {
            get { return _time.Ticks; }
            set { _time = new TimeSpan(value); }
        }
        public int Moves
        {
            get { return _movesCount; }
            set { _movesCount = value; }
        }
        public DateTime When
        {
            get { return _when; }
            set { _when = value; }
        }

        public LevelHighscore()
        {
            _movesCount = int.MaxValue;
        }

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
