using Kinectomix.LevelGenerator.Mvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.Model
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
            dialog.Filter = "Atomix level (*.xml)|*.xml|Atomix binary level (*.xnb)|*.xnb|Atomix custom binary (*.atb)|*.atb|Atomix xml serialization (*.atx)|*.atx";
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
