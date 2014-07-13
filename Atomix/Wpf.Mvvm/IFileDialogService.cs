namespace Kinectomix.Wpf.Mvvm
{
    /// <summary>
    /// Provides abstraction for File picker dialog.
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Gets or sets an option indicating whether OpenFileDialog allows users to select multiple files.
        /// </summary>
        /// <returns>true if multiple selections are allowed; otherwise, false. The default is false.</returns>
        bool Multiselect { get; set; }
        /// <summary>
        /// Gets a string containing the full path of the file selected in a file dialog. 
        /// </summary>
        /// <returns>A String that is the full path of the file selected in the file dialog. The default is Empty.</returns>
        string FileName { get; }
        /// <summary>
        /// Gets an array that contains one file name for each selected file.
        /// </summary>
        /// <returns>An array of String that contains one file name for each selected file. The default is an array with a single item whose value is Empty.</returns>
        string[] FileNames { get; }
        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <returns>The <see cref="Int32"/> that is the index of the selected filter. The default is 1.</returns>
        int FilterIndex { get; }
        /// <summary>
        /// Displays a save file dialog.
        /// </summary>
        /// <returns>If the user clicks the OK button of the SaveFileDialog that is displayed true is returned; otherwise, false.</returns>
        bool SaveFileDialog();
        /// <summary>
        /// Displays a open file dialog.
        /// </summary>
        /// <returns>If the user clicks the OK button of the OpenFileDialog that is displayed true is returned; otherwise, false.</returns>
        bool OpenFileDialog();
    }
}
