namespace Kinectomix.Wpf.Mvvm
{
    public interface IFileDialogService
    {
        string FileName { get; }
        string[] FileNames { get; }
        int FilterIndex { get; }
        bool SaveFileDialog();
        bool OpenFileDialog();
    }
}
