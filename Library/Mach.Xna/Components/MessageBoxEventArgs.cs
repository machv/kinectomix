﻿using System;

namespace Mach.Xna.Components
{
    /// <summary>
    /// Provides data for the <see cref="MessageBoxBase.Changed"/> event.
    /// </summary>
    public class MessageBoxEventArgs : EventArgs
    {
        private MessageBoxResult _result;

        /// <summary>
        /// Gets what result was generated by a message box.
        /// </summary>
        /// <returns>Result from a message box.</returns>
        public MessageBoxResult Result
        {
            get { return _result; }
        }

        /// <summary>
        /// Initializes a new instance of the MessageBoxEventArgs class.
        /// </summary>
        /// <param name="result">Result from a message box.</param>
        public MessageBoxEventArgs(MessageBoxResult result)
        {
            _result = result;
        }
    }
}
