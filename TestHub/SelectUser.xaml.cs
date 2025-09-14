using Microsoft.Data.Sqlite;
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
    /// Interaction logic for SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window, INotifyPropertyChanged
    {
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
                OnPropertyChanged(nameof(OKButton_Enabled));
            }
        }
        
        private Project _selectedProject;
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }
        
        public SelectUser(ObservableCollection<User> users, Project project)
        {
            Users = users;
            SelectedProject = project;

            UpdateUserTestStatus(SelectedProject.TestId);

            InitializeComponent();
        }

        private void EditUsrButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a temporary copy of the Users collection
            ObservableCollection<User> tempUsers = new(Users.Select(u => new User
            {
                UserId = u.UserId,
                Login = u.Login,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
            })); 

            var manageUsersWindow = new ManageUsers(tempUsers);
            if (manageUsersWindow.ShowDialog() == true)
            {
                // Correct exit, update database
                foreach (var user in Users)
                {
                    if (tempUsers.All(u => u.UserId != user.UserId))
                    {
                        LoadSaveDB.DeleteUserFromDatabase(user); // Remove users that are not in the new list
                    }
                }

                Users.Clear();
                foreach (var user in tempUsers)
                {
                    Users.Add(user);
                    LoadSaveDB.SaveUserToDatabase(user); // Save updated user to the database
                }
            }
            UpdateUserTestStatus(SelectedProject.TestId);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser.NotEnoughData())
            {
                MessageBox.Show("Add missing user data!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedUser.HasCompletedTest)
            {
                var msgRet = MessageBox.Show("Do you really want to update selected user's answers?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRet.ToString() == "Yes")
                {
                    var testWindow = new TestWindow(SelectedProject, SelectedUser);
                    DialogResult = true;
                    testWindow.ShowDialog();

                } 
                else return;
            }
            else
            {
                var msgRet = MessageBox.Show("Do you really want to start the test?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRet.ToString() == "Yes")
                {
                    var testWindow = new TestWindow(SelectedProject, SelectedUser);
                    DialogResult = true;
                    testWindow.ShowDialog();
                }
                else return;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void UpdateUserTestStatus(long selectedTestId)
        {
            // Check if user had already done selected test
            string query = @"
                SELECT COUNT(*) FROM Results 
                WHERE UserId = @UserId AND QuestionId IN 
                (SELECT QuestionId FROM Questions WHERE TestId = @TestId)";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();

                foreach (var user in Users)
                {
                    if (user.NotEnoughData())
                    {
                        user.HasCompletedTest = false;
                    }

                    using var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@TestId", selectedTestId);

                    user.HasCompletedTest = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        public bool OKButton_Enabled => SelectedUser != null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
