using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows.Controls;

namespace ChatApp.View
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class StartPageView : Page
    {
        public StartPageView(StartPageViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
