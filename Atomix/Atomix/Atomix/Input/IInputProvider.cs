using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public interface IInputProvider
    {
        IInputState GetState();
    }
}
