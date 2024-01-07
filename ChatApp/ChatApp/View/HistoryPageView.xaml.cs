using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows.Controls;

namespace ChatApp.View
{
    /// <summary>
    /// Interaction logic for HistoryPageView.xaml
    /// </summary>
    public partial class HistoryPageView : Page
    {
        public HistoryPageView(Frame frame, FileManager FileMgr)
        {
            InitializeComponent();
            this.DataContext = new HistoryPageViewModel(frame, FileMgr);
        }
    }
}
