using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.Mvvm
{
    // http://stackoverflow.com/questions/1619505/wpf-openfiledialog-with-the-mvvm-pattern
    public interface IOpenFileService
    {
        string FileName { get; }
        bool OpenFileDialog();
    }
}
