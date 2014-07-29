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
        /// Multiple selection is not implemented.
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
        }

        /// <summary>
        /// Opens the file dialog.
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Localization.DialogResources.DefinitionFileName + " (*.atx)|*.atx|" + Localization.DialogResources.AllFiles + "|*.*";
            dialog.Title = Localization.DialogResources.DefinitionOpenFileDialogTitle;

            string directory = _lastDirectory != null ? _lastDirectory : _initialDirectory;
            if (Directory.Exists(directory))
            {
                dialog.InitialDirectory = directory;
            }

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
            CreateInitialDirectory();

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = Localization.DialogResources.DefinitionFileName + " (*.atx)|*.atx";
            dialog.Title = Localization.DialogResources.DefinitionSaveFileDialogTitle;
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

        private void CreateInitialDirectory()
        {
            try
            {
                if (!Directory.Exists(_initialDirectory))
                {
                    Directory.CreateDirectory(_initialDirectory);
                }
            }
            catch
            { }
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
