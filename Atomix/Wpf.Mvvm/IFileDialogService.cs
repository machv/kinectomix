namespace Kinectomix.Wpf.Mvvm
{
    public interface IFileDialogService
    {
        bool Multiselect { get; set; }
        string FileName { get; }
        string[] FileNames { get; }
        int FilterIndex { get; }
        bool SaveFileDialog();
        bool OpenFileDialog();
    }
}
