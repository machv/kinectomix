using System;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.Mvvm
{
    public class DelegateCommandBase : ICommand
    {
        private readonly Action _action;
        private readonly Func<object, bool> _canExecute;

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

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
