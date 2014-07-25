using Mach.Wpf.Mvvm;
using Microsoft.Win32;
using System;
using System.IO;

namespace Mach.Kinectomix.LevelEditor.Model
{
    /// <summary>
    /// Dialog for opening/saving game definition file.
    /// </summary>
    public class GameLevelsFileDialog : IFileDialogService
    {
        private string _fileName;
        private string _initialDirectory;
        private string _lastDirectory;

        /// <summary>
        /// Gets the name of the selected file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Getting multiple file names is not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public string[] FileNames
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Getting the index is not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public int FilterIndex
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Mupltiple selection is not implemented.
        /// </summary>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public bool Multiselect
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLevelsFileDialog"/> class.
        /// </summary>
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

        /// <summary>
        /// Opens the file dialog.
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Kinectomix levels (*.atx)|*.atx";
            dialog.Title = "Open Kinectomix levels";
            dialog.InitialDirectory = _lastDirectory != null ? _lastDirectory : _initialDirectory;

            if (dialog.ShowDialog() == true)
            {
                UpdateLastDirectory(dialog.FileName);

                _fileName = dialog.FileName;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the file dialog.
        /// </summary>
        /// <returns></returns>
        public bool SaveFileDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Kinectomix levels (*.atx)|*.atx";
            dialog.Title = "Save Kinectomix levels";
            dialog.AddExtension = true;
            dialog.InitialDirectory = _lastDirectory != null ? _lastDirectory : _initialDirectory;
            dialog.FileName = "Definition.atx";

            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName == "")
                    return false;

                UpdateLastDirectory(dialog.FileName);

                _fileName = dialog.FileName;

                return true;
            }

            return false;
        }

        private void UpdateLastDirectory(string fileName)
        {
            string path = Path.GetDirectoryName(fileName);

            if (Directory.Exists(path))
            {
                _lastDirectory = path;
            }
        }
    }
}
