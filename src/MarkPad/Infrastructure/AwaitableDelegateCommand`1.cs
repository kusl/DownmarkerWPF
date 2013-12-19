using System;
using System.Threading.Tasks;

namespace MarkPad.Infrastructure
{
    public class AwaitableDelegateCommand<T> : IAsyncCommand<T>
    {
        readonly Func<T, Task> executeMethod;
        readonly DelegateCommand<T> underlyingCommand;
        bool isExecuting;

        public AwaitableDelegateCommand(Func<T, Task> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
        }

        public async Task ExecuteAsync(T obj)
        {
            try
            {
                isExecuting = true;
                RaiseCanExecuteChanged();
                await executeMethod(obj);
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(object parameter)
        {
            return !isExecuting && underlyingCommand.CanExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { underlyingCommand.CanExecuteChanged += value; }
            remove { underlyingCommand.CanExecuteChanged -= value; }
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync((T) parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            underlyingCommand.RaiseCanExecuteChanged();
        }
    }
}