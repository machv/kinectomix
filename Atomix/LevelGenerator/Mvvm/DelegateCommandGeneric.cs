using System;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.Mvvm
{
    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        public DelegateCommand(Action<T> action, Func<T, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public bool CanExecute(T parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(T parameter)
        {
            _action(parameter);
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
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
