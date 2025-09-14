using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestHub
{
    /// <summary>
    /// Interaction logic for ManageUsers.xaml
    /// </summary>
    public partial class ManageUsers : Window, INotifyPropertyChanged
    {
        private int userToAddCounter = 1;
        
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }
        
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UserIsSelected));
            }
        }
        public ManageUsers(ObservableCollection<User> users)
        {
            Users = users;
            userToAddCounter = users.Count + 1;
            
            InitializeComponent();
        }

        private void AddUser()
        {
            Users.Add(new User { UserId = 0, Login = "user#" + userToAddCounter++.ToString(),
                                 Name = "", Surname = "", Email = ""});
        }

        // Button
        private void CreateUsrButton_Click(object sender, RoutedEventArgs e)
        {
            AddUser();
        }
        private void RemoveUsrButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex >= 0)
            {
                Users.RemoveAt(UsersListBox.SelectedIndex);
            }
        }
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }

        public bool UserIsSelected
        {
            get => SelectedUser != null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
