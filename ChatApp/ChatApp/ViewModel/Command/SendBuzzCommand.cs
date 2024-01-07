using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    class SendBuzzCommand: ICommand
    {
        private ChatPageViewModel parent;

        public SendBuzzCommand(ChatPageViewModel parent)
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
            parent.SendBuzzMessage();
        }
    }
}
