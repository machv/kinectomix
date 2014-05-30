using System;

namespace Kinectomix.Logic
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TilePropertiesAttribute : Attribute
    {
        bool _isFixed = false;
        bool _showInBoardEditor = true;
        bool _showInMoleculeEditor = false;

        public TilePropertiesAttribute() { }

        public TilePropertiesAttribute(bool isFixed, bool showInEditor)
        {
            _isFixed = isFixed;
            _showInBoardEditor = showInEditor;
        }

        public bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }

        public bool ShowInBoardEditor
        {
            get { return _showInBoardEditor; }
            set { _showInBoardEditor = value; }
        }

        public bool ShowInMoleculeEditor
        {
            get { return _showInMoleculeEditor; }
            set { _showInMoleculeEditor = value; }
        }
    }
}
