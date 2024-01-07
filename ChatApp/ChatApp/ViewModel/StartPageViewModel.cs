using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Command;
using System.Windows.Controls;
using System.Windows;
using System.Media;

namespace ChatApp.ViewModel
{
    public class StartPageViewModel : BaseViewModel
    {

        public NetworkManager NetworkMgr { get; set; }
        public FileManager FileMgr { get; set; }

        private ICommand startServer;
        private ICommand connectToServer;
        private ICommand showHistory;
        
        private Frame UiFrame;

        public UserModel User { get; set; }


        public StartPageViewModel(NetworkManager manager, Frame frame)
        {
            NetworkMgr = manager;
            FileMgr = new FileManager();

            //Initialize UserModel
            User ??= new UserModel();
            

            UiFrame = frame;
        }

        public string UserName
        {
            get { return User.UserName; }
            set { User.UserName = value; OnPropertyChanged("UserName"); }
        }

        public string Ip
        {
            get { return User.IP; }
            set { User.IP = value; OnPropertyChanged("IP"); }
        }
        public string PortNumber
        {
            get { return User.PortNumber; }
            set
            {
                User.PortNumber = value; OnPropertyChanged("PortNumber");
            }
        }

        public ICommand StartServer
        {
            get
            {
                startServer ??= new StartServerCommand(this);
                return startServer;
            }
            set
            {
                startServer = value;
            }
        }

        public ICommand ConnectToServer
        {
            get
            {
                connectToServer ??= new ConnectToServerCommand(this);
                return connectToServer;
            }
            set
            {
                connectToServer = value;
            }
        }

        public ICommand ShowHistory
        {
            get
            {
                showHistory ??= new ShowHistoryCommand(this);
                return showHistory;
            }
            set
            {
                showHistory = value;
            }
        }

        private ICommand closeWindowCommand;

        public ICommand CloseWindowCommand
        {
            get
            {
                closeWindowCommand ??= new RelayCommand(param => this.CloseWindow(), null);
                return closeWindowCommand;
            }
        }


        private async void CloseWindow()
        {
            NetworkMgr.CloseServer(User);
        }

        public void TryShowHistory()
        {
            HistoryPageView page = new HistoryPageView(UiFrame, FileMgr);
            UiFrame.NavigationService.Navigate(page);
        }

        public async void TryConnectToServer()
        {
            if (string.IsNullOrEmpty(User.UserName))
            {
                MessageBox.Show("Please enter a username!");
            }
            else
            {
                bool result = await NetworkMgr.ConnectToServer(User);
                if (result)
                {
                    ChatPageView page = new ChatPageView(UiFrame, NetworkMgr, FileMgr, User);
                    UiFrame.NavigationService.Navigate(page);
                }
                else
                {
                    MessageBox.Show(NetworkMgr.ErrorMessage);
                }
            }
        }

        public async void TryStartServer()
        {
            if (string.IsNullOrEmpty(User.UserName))
            {
                MessageBox.Show("Please enter a username!");
            }
            else
            {
                bool result = await NetworkMgr.StartConnection(User);
                if (result)
                {
                    ChatPageView page = new ChatPageView(UiFrame, NetworkMgr, FileMgr, User);
                    UiFrame.NavigationService.Navigate(page);
                }
                else
                {
                    MessageBox.Show(NetworkMgr.ErrorMessage);
                }
            }
        }
    }
}
