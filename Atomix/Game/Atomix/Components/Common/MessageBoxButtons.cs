﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Components.Common
{
    /// <summary>
    /// Specifies constants defining which buttons to display on a MessageBox.
    /// </summary>
    public enum MessageBoxButtons
    {
        /// <summary>
        /// The message box contains an OK button.
        /// </summary>
        OK,
        /// <summary>
        /// The message box contains OK and Cancel buttons.
        /// </summary>
        OKCancel,
        /// <summary>
        /// The message box contains Yes and No buttons.
        /// </summary>
        YesNo,
        /// <summary>
        /// The message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel,
    }
}
