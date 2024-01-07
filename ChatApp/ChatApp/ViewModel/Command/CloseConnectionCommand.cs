using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class CloseConnectionCommand: ICommand
    {
        private ChatPageViewModel parent;

        public CloseConnectionCommand(ChatPageViewModel parent)
        {
            this.parent = parent;

        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //Check so that there is an allowed username, ip and port
            return true;
        }

        public void Execute(object parameter)
        {
            parent.TryCloseConnection(); ;
        }
    }
}
