using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class FilterOldChatsCommand: ICommand
    {
        private HistoryPageViewModel parent;

        public FilterOldChatsCommand(HistoryPageViewModel parent)
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
            parent.TryFilterOldChats();
        }
    }
}
