using System.ComponentModel;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.ViewModel.Command;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.ViewModel
{
    internal class ChatPageViewModel : BaseViewModel
    {

        private NetworkManager NetworkMgr { get; set; }
        private FileManager FileMgr { get; set; }
        private readonly Frame UiFrame;

        private ICommand? closeServer;
        private ICommand? sendMessage;
        private ICommand? closeConnection;
        private ICommand? sendBuzz;
        private ICommand? saveChat;
        public ObservableCollection<MessageModel> Messages { get { return NetworkMgr._Messages; } }

        private string _message = "";

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        public string ClientName
        {
            get { return NetworkMgr.OtherUser; }
            set { NetworkMgr.OtherUser = value; OnPropertyChanged("ClientName"); }
        }

        public string UserName
        {
            get { return User.UserName; }
            set { User.UserName = value; OnPropertyChanged("UserName"); }
        }

        public UserModel User { get; set; }


        public ChatPageViewModel(Frame frame, NetworkManager manager, FileManager fileMgr, UserModel user)
        {
            UiFrame = frame;
            NetworkMgr = manager;
            NetworkMgr.PropertyChanged += ClientNameChanged;
            User = user;
            FileMgr = fileMgr;


            if (NetworkMgr.IsServer)
            {
                HandleServerConnection();
            }
            else
            {
                ListenForMessages();
            }
        }

        public ICommand CloseServer
        {
            get
            {
                closeServer = new CloseServerCommand(this);
                return closeServer;
            }
            set
            {
                closeServer = value;
            }
        }

        public ICommand SendMessage
        {
            get
            {
                sendMessage = new SendMessageCommand(this);
                return sendMessage;
            }
            set
            {
                sendMessage = value;
            }
        }

        public ICommand CloseConnection
        {
            get
            {
                closeConnection = new CloseConnectionCommand(this);
                return closeConnection;
            }
            set
            {
                closeConnection = value;
            }
        }

        public ICommand SendBuzz
        {
            get
            {
                sendBuzz = new SendBuzzCommand(this);
                return sendBuzz;
            }
            set
            {
                sendBuzz = value;
            }
        }

        public ICommand SaveChat
        {
            get
            {
                saveChat = new SaveChatCommand(this);
                return saveChat;
            }
            set
            {
                saveChat = value;
            }
        }

        public async void TryCloseConnection()
        {
            await NetworkMgr.CloseConnection(User);
        }

        private async void HandleServerConnection()
        {
            bool running = true;
            while (running)
            {
                // Wait for connecting user
                string sender = await NetworkMgr.GetConnectingUser();

                if (!string.IsNullOrEmpty(sender))
                {
                    // Get input if to accept connection or deny
                    bool acceptConnection = MessageBox.Show($"{sender} wants to chat, do you accept?",
                            "Chat request",
                            MessageBoxButton.YesNo)
                            == MessageBoxResult.Yes;
                    if (acceptConnection)
                    {
                        await NetworkMgr.AnswerConnectionRequest(sender, User, true);
                        await NetworkMgr.ListenForMessages(User);
                    }
                    else
                    {
                        await NetworkMgr.AnswerConnectionRequest(sender, User, false);
                    }
                }
            }
        }

        private async void ListenForMessages()
        {
            await NetworkMgr.ListenForMessages(User);
        }
        public async void CloseServerAndGoBack()
        {
            if (UiFrame.CanGoBack)
            {
                await NetworkMgr.CloseServer(User);
                UiFrame.GoBack();
            }
        }

        //Tries to send a message to the recieving part if Message is not Null or WhiteSpace.
        public void SendTextMessage()
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                MessageModel msg = new(MessageType.Message, Message, User.UserName);
                TrySendMessage(msg);
                Message = "";
            }
        }
        public void SendBuzzMessage()
        {
            MessageModel msg = new(MessageType.Buzz, "", User.UserName);
            TrySendMessage(msg);
        }

        public async void TrySendMessage(MessageModel msg)
        {

            bool success = await NetworkMgr.SendMessage(msg);
            if (!success)
            {
                MessageBox.Show("Message or buzz could not be sent, no other user connected!");
            }

        }

        public void TrySaveChat()
        {
            if (NetworkMgr._Messages.Count < 1 || string.IsNullOrEmpty(NetworkMgr.LatestUser))
            {
                MessageBox.Show("No conversation available to save unfortunately.");
            }
            else
            {
                FileMgr.SaveChatToFile(Messages, User.UserName, NetworkMgr.LatestUser);
            }
        }

        private void ClientNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OtherUser")
            {
                ClientName = NetworkMgr.OtherUser;
            }
        }
    }
}
