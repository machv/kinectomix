using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public interface IInputState
    {
        int X { get; }

        int Y { get; }

        bool IsSelected { get; }
    }
}
