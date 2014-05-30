using System;
using Kinectomix.Wpf.Mvvm;
using Microsoft.Win32;

namespace Kinectomix.GestureRecorder.Model
{
    public class GestureFileDialog : IFileDialogService
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
            dialog.Filter = "Recorded gesture (*.gst)|*.gst|All files|*.*";
            dialog.Title = "Open recorded gesture";

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;
                _fileNames = dialog.FileNames;

                return true;
            }

            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Recorded gesture (*.gst)|*.gst";
            dialog.Title = "Save recorded gesture";

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
