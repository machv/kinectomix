﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix
{
    public class MouseInputState : IInputState
    {
        protected MouseState _state;

        public int X
        {
            get { return _state.X; }
        }

        public int Y
        {
            get { return _state.Y; }
        }

        public bool IsSelected
        {
            get { return _state.LeftButton == ButtonState.Pressed; }
        }

        public MouseInputState(MouseState state)
        {
            _state = state;
        }
    }
}