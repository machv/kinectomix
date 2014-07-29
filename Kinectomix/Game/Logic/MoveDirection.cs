using System;

namespace Mach.Kinectomix
{
    /// <summary>
    /// Flag indicating in which directions can be tile moved on the board.
    /// </summary>
    [Flags]
    public enum MoveDirection
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }
}
