using Kinectomix.Wpf.Mvvm;
using Microsoft.Win32;
using System;

namespace Kinectomix.LevelEditor.Model
{
    public class GameLevelsFileDialog : IFileDialogService
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        public string[] FileNames
        {
            get { throw new NotImplementedException(); }
        }

        public int FilterIndex
        {
            get { throw new NotImplementedException(); }
        }

        public bool Multiselect
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Kinectomix levels (*.atx)|*.atx";
            dialog.Title = "Open Kinectomix levels";

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;

                return true;
            }

            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Kinectomix levels (*.atx)|*.atx";
            dialog.Title = "Save Kinectomix levels";

            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName == "")
                    return false;

                _fileName = dialog.FileName;

                return true;
            }

            return false;
        }
    }
}
