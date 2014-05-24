using Kinectomix.Wpf.Mvvm;
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
            get
            {
                return _filterIndex;
            }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Atomix level (*.xnb, *.xml, *.atb, *.atx)|*.xnb;*.xml;*.atb;*.atx|All files|*.*";
            dialog.Title = "Open Kinectomix level definition";

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
            dialog.Filter = "Atomix xml serialization (*.atx)|*.atx|Atomix binary level (*.xnb)|*.xnb|Atomix custom binary (*.atb)|*.atb|Atomix level (*.xml)|*.xml";
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
