using System.ComponentModel;

namespace TestHub
{
    public class User : INotifyPropertyChanged
    {
        private long _id;
        public long UserId
        {
            get => _id;
            set { _id = value; OnPropertyChanged("UserId"); }
        }
        
        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged("Login"); }
        }
        
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged("Name"); }
        }
        
        private string _surname;
        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged("Surname"); }
        }
        
        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged("Email"); }
        }
        
        private bool _hasCompletedTest;
        public bool HasCompletedTest
        {
            get => _hasCompletedTest || NotEnoughData();
            set { _hasCompletedTest = value; OnPropertyChanged("HasCompletedTest"); }
        }

        public bool NotEnoughData() => Login.Length == 0 || Name.Length == 0 || Surname.Length == 0 || Email.Length == 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

