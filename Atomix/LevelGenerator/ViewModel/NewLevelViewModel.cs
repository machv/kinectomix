using Kinectomix.Wpf.Mvvm;
using System.Windows.Input;

namespace Kinectomix.LevelEditor.ViewModel
{
    public class NewLevelViewModel : NotifyPropertyBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private int _boardRows = 5;
        public int BoardRows
        {
            get { return _boardRows; }
            set { _boardRows = value; OnPropertyChanged(); }
        }

        private int _boardColumns = 5;
        public int BoardColumns
        {
            get { return _boardColumns; }
            set { _boardColumns = value; OnPropertyChanged(); }
        }

        private int _moleculeRows = 2;
        public int MoleculeRows
        {
            get { return _moleculeRows; }
            set { _moleculeRows = value; OnPropertyChanged(); }
        }

        private int _moleculeColumns = 2;
        public int MoleculeColumns
        {
            get { return _moleculeColumns; }
            set { _moleculeColumns = value; OnPropertyChanged(); }
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; OnPropertyChanged(); }
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