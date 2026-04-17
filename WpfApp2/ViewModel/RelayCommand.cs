using System;
using System.Windows.Input;

namespace WpfApp2.ViewModel
{
    public class RelayCommand(Action execute,
    Func<bool>? canExecute = null) : ICommand
    {
        private readonly Action _execute = execute;
        private readonly Func<bool>? _canExecute = canExecute;
        public bool CanExecute(object? parameter = null)
        => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter)) _execute.Invoke();
        }
        public event EventHandler? CanExecuteChanged;
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class RelayCommand<T>(Action<object?> execute,
    Predicate<object?>? canExecute = null) : ICommand
    {
        private readonly Action<object?> _execute = execute ?? throw new
        ArgumentNullException(nameof(execute));
        private readonly Predicate<object?>? _canExecute = canExecute;
        public bool CanExecute(object? parameter)
        => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter)
        {
            if (CanExecute(parameter)) _execute.Invoke(parameter);
        }
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}