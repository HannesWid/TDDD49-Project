using System.ComponentModel;

namespace ChatApp.Model
{
    public class UserModel : INotifyPropertyChanged
    {
        public UserModel()
        {
            _userName = "";
            _portNumber = "";
            _ip = "";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        private string _portNumber;
        public string PortNumber
        {
            get { return _portNumber; }
            set { _portNumber = value; OnPropertyChanged(nameof(PortNumber)); }
        }


        private string _ip;
        public string IP
        {
            get { return _ip; }
            set { _ip = value; OnPropertyChanged(nameof(IP)); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
