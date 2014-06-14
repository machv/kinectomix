using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectomix.Xna.Input
{
    public class MouseInputProvider : IInputProvider
    {
        public IInputState GetState()
        {
            MouseState mouseState = Mouse.GetState();

            return new MouseInputState(mouseState);
        }
    }
}
