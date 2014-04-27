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

        private int _boardRows = 5;
        public int BoardRows
        {
            get { return _boardRows; }
            set { _boardRows = value; RaisePropertyChangedEvent(); }
        }

        private int _boardColumns = 5;
        public int BoardColumns
        {
            get { return _boardColumns; }
            set { _boardColumns = value; RaisePropertyChangedEvent(); }
        }

        private int _moleculeRows = 2;
        public int MoleculeRows
        {
            get { return _moleculeRows; }
            set { _moleculeRows = value; RaisePropertyChangedEvent(); }
        }

        private int _moleculeColumns = 2;
        public int MoleculeColumns
        {
            get { return _moleculeColumns; }
            set { _moleculeColumns = value; RaisePropertyChangedEvent(); }
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