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
    /// Interaction logic for ManageQuestion.xaml
    /// </summary>
    public partial class ManageQuestion : Window, INotifyPropertyChanged
    {
        private Question _question;
        public Question QuestionPub
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged();
            }
        }

        private Answer _selectedAnswer;
        public Answer SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                _selectedAnswer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AnswerIsSelected));
            }
        }

        public ManageQuestion(Question question)
        {
            QuestionPub = question;

            InitializeComponent();
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnswerTextBox.Text.Length == 0)
            {
                QuestionPub.Answers.Add(new Answer() { AnswerId = 0, AnswerText = "Type your answer" });

            } else if (AnswerTextBox.Text.Length > 0)
            {
                QuestionPub.Answers.Add(new Answer() { AnswerId = 0, AnswerText = AnswerTextBox.Text });
                AnswerTextBox.Text = "";
            }
        }
        private void UpdateAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAnswer != null && AnswerTextBox.Text.Length > 0)
            {
                SelectedAnswer.AnswerText = AnswerTextBox.Text;
                AnswerTextBox.Text = "";
            } else
            {
                MessageBox.Show("Type new answer into the field!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnswersListBox.SelectedIndex >= 0)
            {
                QuestionPub.Answers.RemoveAt(AnswersListBox.SelectedIndex);
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

        public bool AnswerIsSelected
        {
            get => SelectedAnswer != null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
