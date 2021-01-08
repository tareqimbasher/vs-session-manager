using System;
using System.Windows.Input;

namespace SessionManagerExtension.WPF
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _onExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> onExecute)
        {
            _onExecute = onExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _onExecute(parameter);
        }
    }
}