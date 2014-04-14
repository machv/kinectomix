using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator.Mvvm
{
    public interface IFileDialogService
    {
        string FileName { get; }
        int FilterIndex { get; }
        bool SaveFileDialog();
        bool OpenFileDialog();
    }
}
