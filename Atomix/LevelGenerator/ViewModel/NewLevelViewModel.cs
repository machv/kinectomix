using Kinectomix.LevelGenerator.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.ViewModel
{
    public class NewLevelViewModel : Mvvm.NotifyPropertyBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChangedEvent(); }
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; RaisePropertyChangedEvent(); }
        }

        public ICommand CreateLevelCommand
        {
            get { return new DelegateCommand(CreateLevel); }
        }

        private void CreateLevel()
        {
            DialogResult = true;
        }
    }
}