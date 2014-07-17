using Mach.Wpf.Mvvm;
using Microsoft.Win32;
using System;
using System.IO;

namespace Kinectomix.LevelEditor.Model
{
    public class GameLevelsFileDialog : IFileDialogService
    {
        private string _fileName;
        private string _initialDirectory;
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

        public GameLevelsFileDialog()
        {
            _initialDirectory = Path.Combine
                (
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SavedGames",
                    Properties.Settings.Default.GameName,
                    "Game",
                    "AllPlayers"
                );

            if (!Directory.Exists(_initialDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_initialDirectory);
                }
                catch
                { }
            }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Kinectomix levels (*.atx)|*.atx";
            dialog.Title = "Open Kinectomix levels";
            dialog.InitialDirectory = _initialDirectory;

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
            dialog.AddExtension = true;
            dialog.InitialDirectory = _initialDirectory;
            dialog.FileName = "Definition.atx";

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
