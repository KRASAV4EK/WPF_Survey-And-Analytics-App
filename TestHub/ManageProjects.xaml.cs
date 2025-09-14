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
    /// Interaction logic for ManageProjects.xaml
    /// </summary>
    public partial class ManageProjects : Window, INotifyPropertyChanged
    {
        private int projToAddCounter = 1;

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
                OnPropertyChanged(nameof(ProjectIsSelected));
            }
        }

        public ManageProjects(ObservableCollection<Project> projects)
        {
            Projects = projects;
            projToAddCounter = projects.Count + 1;
         
            InitializeComponent();
        }

        private void CreateProjButton_Click(object sender, RoutedEventArgs e)
        {
            Project project = new Project { TestId = 0, ProjTitle = "proj#" + projToAddCounter++.ToString() };
            project.AddQuestion();
            Projects.Add(project);
        }
        private void RemoveProjButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjListBox.SelectedIndex >= 0)
            {
                Projects.RemoveAt(ProjListBox.SelectedIndex);
            }
        }

        private void UpdProjectTitleButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProject != null && ProjectTextBox.Text.Length > 0)
            {
                SelectedProject.ProjTitle = ProjectTextBox.Text;
            }
            else if (ProjectTextBox.Text.Length == 0)
            {
                MessageBox.Show("Type new project title!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedProject?.AddQuestion();
        }
        private void EditQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProject == null || SelectedProject.SelectedQuestion == null)
            {
                MessageBox.Show("Select some question!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Question originalQuestion = SelectedProject.SelectedQuestion;

            Question TempQuestion = new()
            {
                QuestionText = originalQuestion.QuestionText,
                Answers = new ObservableCollection<Answer>(originalQuestion.Answers)
            };

            var manageQuestionWindow = new ManageQuestion(TempQuestion);

            if (manageQuestionWindow.ShowDialog() == true)
            {
                originalQuestion.QuestionText = TempQuestion.QuestionText;

                originalQuestion.Answers.Clear();
                foreach (var answer in TempQuestion.Answers)
                {
                    originalQuestion.Answers.Add(answer);
                }
            }
        }
        private void RemoveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProject != null && SelectedProject.SelectedQuestion != null)
            {
                SelectedProject.ProjQuestions.Remove(SelectedProject.SelectedQuestion);
            } else
            {
                MessageBox.Show("Select some question!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        
        public bool ProjectIsSelected
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
