using System;
using System.Windows.Input;

namespace Kinectomix.LevelGenerator.Mvvm
{
//    public class DelegateCommand<T> : ICommand
//    {
//        public DelegateCommand(Action<T> action)
//        {
//            _action = action;
//        }

//        public DelegateCommand(Action<T> action, ICommandOnCanExecute canExecute)
//        {
//            _action = action;
//            _canExecute = canExecute;
//        }

//        private readonly Action<object> _action;
//        private readonly ICommandOnCanExecute _canExecute;

//        //public delegate void ICommandOnExecute(object parameter);
//        public delegate bool ICommandOnCanExecute(object parameter);


//        public bool CanExecute(T parameter)
//        {
//            if (_canExecute == null)
//            {
//                return true;
//            }

//            return _canExecute(parameter);
//        }

//        public void Execute(T parameter)
//        {
//            _action(parameter);
//        }

//        void Execute(object parameter)
//        {
//            _action(parameter);
//        }

//        bool CanExecute(object parameter)
//        {
//            return _canExecute(parameter);
//        }

//#pragma warning disable 67
//        public event EventHandler CanExecuteChanged;

//        public void RaiseCanExecuteChanged()
//        {
//            if (CanExecuteChanged != null)
//            {
//                CanExecuteChanged(this, EventArgs.Empty);
//            }
//        }
//#pragma warning restore 67
//    }
}
