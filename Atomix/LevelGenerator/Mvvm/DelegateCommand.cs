using System;
using System.Windows.Input;

namespace Kinectomix.LevelEditor.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;
        private readonly ICommandOnCanExecute _canExecute;

        //public delegate void ICommandOnExecute(object parameter);
        public delegate bool ICommandOnCanExecute(object parameter);

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public DelegateCommand(Action action, ICommandOnCanExecute canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _action();
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
#pragma warning restore 67
    }
}
