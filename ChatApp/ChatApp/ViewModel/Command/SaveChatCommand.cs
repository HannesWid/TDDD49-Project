using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class SaveChatCommand : ICommand
    {
        private ChatPageViewModel parent;

        public SaveChatCommand(ChatPageViewModel parent)
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
            parent.TrySaveChat();
        }
    }
}

