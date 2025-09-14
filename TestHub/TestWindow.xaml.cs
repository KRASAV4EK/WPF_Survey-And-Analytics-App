using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TestHub
{
    public partial class TestWindow : Window, INotifyPropertyChanged
    {
        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged();
            }
        }

        public TestWindow(Project project, User user)
        {
            Project = project;
            User = user;

            foreach (var question in Project.ProjQuestions)
            {
                question.CanSubmitChanged = () => OnPropertyChanged(nameof(CanSubmit));
            }

            InitializeComponent();
        }

        public bool CanSubmit => Project.ProjQuestions.All(q => q.SelectedAnswer != null);
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadSaveDB.SaveResultToDatabase(User, Project);
                MessageBox.Show("Your answers have been saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach(Question question in Project.ProjQuestions) { question.SelectedAnswer = null; }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving answers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Question question in Project.ProjQuestions) { question.SelectedAnswer = null; }
            DialogResult = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
