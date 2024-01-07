using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class CloseServerCommand : ICommand
    {
        private ChatPageViewModel parent;

        public CloseServerCommand(ChatPageViewModel parent)
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
            parent.CloseServerAndGoBack();
        }
    }
}

