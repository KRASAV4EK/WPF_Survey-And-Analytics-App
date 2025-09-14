using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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

        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
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
                ResetScrollViewer();
                OnPropertyChanged(nameof(AddResltButton_Enabled));
            }
        }

        public ObservableCollection<QuestionPieSeries> QuestionPieSeriesCollections { get; set; }

        public MainWindow()
        {
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            string DatabasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.db");
            Console.WriteLine($"Database {DatabasePath}\n");

            Users = LoadSaveDB.LoadUsersFromDatabase();
            Projects = LoadSaveDB.LoadProjectsFromDatabase();
            QuestionPieSeriesCollections = [];

            InitializeComponent();
        }

        // Button
        private void AddResltButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProject == null) return;

            var selectUserWindow = new SelectUser(Users, SelectedProject);
            selectUserWindow.ShowDialog();

            UpdateProjResults();
        }
        private void MangProjButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a temporary copy of the Projects collection
            var tempProjects = new ObservableCollection<Project>(Projects.Select(project => new Project
            {
                TestId = project.TestId,
                ProjTitle = project.ProjTitle,
                ProjQuestions = new ObservableCollection<Question>(project.ProjQuestions.Select(question => new Question
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    Answers = new ObservableCollection<Answer>(question.Answers.Select(answer => new Answer
                    {
                        AnswerId = answer.AnswerId,
                        AnswerText = answer.AnswerText
                    }))
                }))
            }));

            // Open the ManageProjects window
            var manageProjectsWindow = new ManageProjects(tempProjects);

            if (manageProjectsWindow.ShowDialog() == true)
            {
                // Correct exit, update database
                foreach (var project in Projects)
                {
                    if (tempProjects.All(p => p.TestId != project.TestId))
                    {
                        LoadSaveDB.DeleteProjFromDatabase(project); // Remove projects that are not in the new list
                    }
                    else
                    {
                        // Check for deleted questions in the existing project
                        var updatedProject = tempProjects.First(p => p.TestId == project.TestId);
                        foreach (var question in project.ProjQuestions)
                        {
                            if (updatedProject.ProjQuestions.All(q => q.QuestionId != question.QuestionId))
                            {
                                LoadSaveDB.DeleteQuestionFromDatabase(question); // Remove questions that are not in the updated list
                            }
                            else
                            {
                                // Check for deleted answers in the existing question
                                var updatedQuestion = updatedProject.ProjQuestions.First(q => q.QuestionId == question.QuestionId);
                                foreach (var answer in question.Answers)
                                {
                                    if (updatedQuestion.Answers.All(a => a.AnswerId != answer.AnswerId))
                                    {
                                        LoadSaveDB.DeleteAnswerFromDatabase(answer); // Remove answers that are not in the updated list
                                    }
                                }
                            }
                        }
                    }
                }

                Projects.Clear();
                foreach (var project in tempProjects)
                {
                    Projects.Add(project);
                    LoadSaveDB.SaveProjToDatabase(project); // Save updated project to the database
                }

                if (SelectedProject == null)
                {
                    QuestionPieSeriesCollections.Clear();
                } else
                {
                    UpdateProjResults();
                }
            }
        }
        private void MangUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a temporary copy of the Users collection
            var tempUsers = new ObservableCollection<User>(Users.Select(u => new User
            {
                UserId = u.UserId,
                Login = u.Login,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email
            }));

            var manageUsersWindow = new ManageUsers(tempUsers);

            if (manageUsersWindow.ShowDialog() == true)
            {
                // Check for duplicate logins
                var duplicateLoginUsers = tempUsers.GroupBy(u => u.Login)
                                                   .Where(g => g.Count() > 1)
                                                   .Select(g => g.Key)
                                                   .ToList();

                if (duplicateLoginUsers.Any())
                {
                    string duplicateLogins = string.Join(", ", duplicateLoginUsers);
                    MessageBox.Show($"Users with duplicate logins found: {duplicateLogins}. Please resolve this issue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Correct exit, update database
                foreach (var user in Users)
                {
                    if (tempUsers.All(u => u.UserId != user.UserId))
                    {
                        LoadSaveDB.DeleteUserFromDatabase(user); // Remove users that are not in the new list
                        continue;
                    }
                }

                Users.Clear();
                foreach (var user in tempUsers)
                {
                    // Check if login already exists in the original Users collection (excluding the same user)
                    if (Users.Any(u => u.Login == user.Login && u.UserId != user.UserId))
                    {
                        MessageBox.Show($"The login '{user.Login}' is already in use. Please choose a different login.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    Users.Add(user);
                    LoadSaveDB.SaveUserToDatabase(user); // Save updated user to the database
                }
            }

            UpdateProjResults();
        }

        // Graph
        private void TestsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             LoadPieChartsForQuestions();
        }
        private void LoadPieChartsForQuestions()
        {
            if (SelectedProject == null) { return; }

            QuestionPieSeriesCollections.Clear();

            foreach (var question in SelectedProject.ProjQuestions)
            {
                var questionSeries = new QuestionPieSeries
                {
                    QuestionText = question.QuestionText
                };

                foreach (var answer in question.Answers)
                {
                    int count = LoadAnswerCountFromDatabase(answer.AnswerId);

                    questionSeries.SeriesCollection.Add(new PieSeries
                    {
                        Title = answer.AnswerText,
                        Values = new ChartValues<int> { count },
                        DataLabels = true
                    });
                }

                QuestionPieSeriesCollections.Add(questionSeries);
            }
        }
        private static int LoadAnswerCountFromDatabase(long answerId)
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM Results WHERE AnswerId = @AnswerId";

            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AnswerId", answerId);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return count;
        }

        // Scrolling
        private void ScrollViewer_ScrollSpeed(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                double scrollStep = 404;
                if (e.Delta > 0)
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - scrollStep);
                else
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollStep);

                e.Handled = true;
            }
        }
        private void ResetScrollViewer()
        {
            if (MainContentScrollViewer != null)
            {
                MainContentScrollViewer.ScrollToVerticalOffset(0);
                MainContentScrollViewer.ScrollToHorizontalOffset(0);
            }
        }

        /// <summary>
        /// Update graphical representation of test
        /// </summary>
        private void UpdateProjResults()
        {
            var tempProject = SelectedProject;
            SelectedProject = null;
            SelectedProject = tempProject;
        }

        public bool AddResltButton_Enabled
        {
            get => SelectedProject != null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}