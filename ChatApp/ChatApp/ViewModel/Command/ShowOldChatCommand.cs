using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class ShowOldChatCommand : ICommand
    {
        private HistoryPageViewModel parent;

        public ShowOldChatCommand(HistoryPageViewModel parent)
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
            parent.TryShowOldChat();
        }
    }
}
