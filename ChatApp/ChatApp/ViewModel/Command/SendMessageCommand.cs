using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    class SendMessageCommand: ICommand
    {
        private ChatPageViewModel parent;


        public SendMessageCommand(ChatPageViewModel parent)
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
            parent.SendTextMessage();
        }
    }
}
