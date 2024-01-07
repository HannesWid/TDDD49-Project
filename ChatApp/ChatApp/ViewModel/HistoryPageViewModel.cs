using ChatApp.Model;
using ChatApp.ViewModel.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    internal class HistoryPageViewModel : BaseViewModel
    {
        private Frame UiFrame;
        private FileManager FileMgr;

        private ICommand? closeHistory;
        private ICommand? showOldChat;
        private ICommand? filterOldChats;

        public ChatLogModel CurrentChatLog { get; set; }
        public string ChatSearchString { get; set; }

        private ObservableCollection<ChatLogModel> _OldChats;
        public ObservableCollection<ChatLogModel>? OldChats
        {
            get
            {
                return _OldChats;
            }
            set
            {
                if (_OldChats != value)
                {
                    _OldChats = value;
                    OnPropertyChanged(nameof(OldChats));
                }
            }
        }

        public HistoryPageViewModel(Frame frame, FileManager fileMgr)
        {
            UiFrame = frame;
            FileMgr = fileMgr;
            try
            {
                FileMgr.ReadDatabase();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);}
            ChatSearchString = "";
            OldChats = new ObservableCollection<ChatLogModel>(FileMgr._chatLogs);
        }

        public ICommand CloseHistory
        {
            get
            {
                closeHistory ??= new CloseHistoryCommand(this);
                return closeHistory;
            }
            set
            {
                closeHistory = value;
            }
        }

        public void TryCloseHistory()
        {
            if (UiFrame.CanGoBack)
            {
                UiFrame.GoBack();
            }
        }

        public ICommand ShowOldChat
        {
            get
            {
                showOldChat ??= new ShowOldChatCommand(this);
                return showOldChat;
            }
            set
            {
                showOldChat = value;
            }
        }

        public void TryShowOldChat()
        {
            if (CurrentChatLog != null)
            {
                string outputString = "Messages: \n";
                foreach (var message in CurrentChatLog.Messages)
                {
                    outputString += message.Sender + " " + message.MessageDateTime + Environment.NewLine;
                    outputString += message.Message + Environment.NewLine;
                }
                MessageBox.Show(outputString);
            }
            else
            {
                MessageBox.Show("No old chat log chosen!");
            }
        }

        public ICommand FilterChats
        {
            get
            {
                filterOldChats ??= new FilterOldChatsCommand(this);
                return filterOldChats;
            }
            set
            {
                filterOldChats = value;
            }
        }

        public void TryFilterOldChats()
        {
            OldChats = new ObservableCollection<ChatLogModel>(FileMgr._chatLogs.Where(s => s.ChatLogName.Contains(ChatSearchString)));
        }
    }
}
