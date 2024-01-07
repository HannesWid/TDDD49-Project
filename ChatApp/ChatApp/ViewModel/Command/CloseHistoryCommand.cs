using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class CloseHistoryCommand: ICommand
    {
        private HistoryPageViewModel parent;

        public CloseHistoryCommand(HistoryPageViewModel parent)
        {
            this.parent = parent;

        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.TryCloseHistory();
        }
    }
}
