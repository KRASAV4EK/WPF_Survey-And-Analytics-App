using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestHub
{
    /// <summary>
    /// Project as a project.
    /// </summary>
    public class Project : INotifyPropertyChanged
    {
        private long _id;
        private string _name;
        public long TestId
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string ProjTitle
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Question> _questions;
        public ObservableCollection<Question> ProjQuestions
        {
            get => _questions;
            set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuestionIsSelected));
            }
        }

        public Project()
        {
            ProjQuestions = [];
        }

        public void AddQuestion()
        {
            Question newQuestion = new()
            {
                QuestionId = 0,
                QuestionText = "Type your question.",
                Answers = { new Answer() { AnswerId = 0, AnswerText = "Type first answer" },
                            new Answer() { AnswerId = 0, AnswerText = "Type second answer"} }
            };
            ProjQuestions.Add(newQuestion);
        }

        public bool QuestionIsSelected
        {
            get => SelectedQuestion != null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
