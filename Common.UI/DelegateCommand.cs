using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.UI
{
    /// <summary>
    /// Class helps in implementing the Generic command class
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Private fields

        /// <summary>
        /// Delegate method which will execute, CanExecute method in ICommand
        /// </summary>
        private readonly Predicate<object> criteriaMethod;

        /// <summary>
        /// Delegate method which will execute, Execute method in ICommand
        /// </summary>
        private readonly Action<object> executionMethod;

        #endregion Private fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            executionMethod = execute;
            criteriaMethod = canExecute;
        }

        #endregion Constructor

        #region Event

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion Event

        #region Private Methods

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (criteriaMethod == null)
            {
                return true;
            }

            return criteriaMethod(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            executionMethod(parameter);
        }

        #endregion Private Methods
    }
}
