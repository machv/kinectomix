using Kinectomix.LevelGenerator.Mvvm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.Model
{
    public class OpenLevelFile : IOpenFileService
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Atomix level (*.xnb, *.xml)|*.xnb;*.xml|All files|*.*";

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;

                return true;
            }

            return false;
        }
    }
}
