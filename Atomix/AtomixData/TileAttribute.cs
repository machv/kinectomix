using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomixData
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TilePropertiesAttribute : Attribute
    {
        bool isFixed = false;
        bool showInEditor = true;

        public TilePropertiesAttribute() { }

        // This is a positional argument
        public TilePropertiesAttribute(bool isFixed, bool showInEditor)
        {
            this.isFixed = isFixed;
            this.showInEditor = showInEditor;
        }

        public bool IsFixed// { get; set; }
        {
            get { return isFixed; }
            set { isFixed = value; }
        }
        public bool ShowInEditor
        {
            get { return showInEditor; }
            set { showInEditor = value; }
        }
    }
}
