using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows.Controls;

namespace ChatApp.View
{
    public partial class ChatPageView : Page
    {
        public ChatPageView(Frame frame,
            NetworkManager networkMgr,
            FileManager fileMgr,
            UserModel user)
        {
            InitializeComponent();
            this.DataContext = new ChatPageViewModel(frame, networkMgr, fileMgr, user);

        }
    }
}
