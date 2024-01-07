using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class StartServerCommand : ICommand
    {
        private StartPageViewModel parent;


        public StartServerCommand(StartPageViewModel parent)
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
            parent.TryStartServer();
        }
    }
}
