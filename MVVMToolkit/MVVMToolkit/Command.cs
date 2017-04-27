using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MVVMToolkit
{
    public class Command : ICommand
    {
        #region Constructor

        public Command(Action<object> action, Predicate<object> canExecuteEvaluator)
        {
            ExecuteDelegate = action;
            CanExecuteDelegate = canExecuteEvaluator;
        }

        public Command(Action<object> action)
        {
            ExecuteDelegate = action;

        }

        #endregion

        #region Properties

        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
            {
                return CanExecuteDelegate(parameter);
            }

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }

        #endregion
    }
}
