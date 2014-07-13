using System;
using Mach.Wpf.Mvvm;
using Microsoft.Win32;

namespace Kinectomix.LevelEditor.Model
{
    public class LevelFileDialog : IFileDialogService
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        private int _filterIndex;
        public int FilterIndex
        {
            get { return _filterIndex; }
        }

        private string[] _fileNames;
        public string[] FileNames
        {
            get { return _fileNames; }
        }

        private bool _multiselect;
        public bool Multiselect
        {
            get { return _multiselect; }
            set { _multiselect = value; }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = _multiselect;
            dialog.Filter = "Atomix level (*.xnb, *.xml, *.atb, *.atxl)|*.xnb;*.xml;*.atb;*.atxl|All files|*.*";
            dialog.Title = "Open Kinectomix level definition";

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;
                _fileNames = dialog.FileNames;

                return true;
            }

            return false;
        }

        public LevelFileDialog(bool multiselect)
        {
            _multiselect = multiselect;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Atomix xml serialization (*.atxl)|*.atxl|Atomix binary level (*.xnb)|*.xnb|Atomix custom binary (*.atb)|*.atb|Atomix level (*.xml)|*.xml";
            dialog.Title = "Save Kinectomix level definition";

            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName == "")
                    return false;

                _fileName = dialog.FileName;
                _filterIndex = dialog.FilterIndex;

                return true;
            }

            return false;
        }
    }
}
