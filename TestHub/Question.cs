using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestHub
{
    /// <summary>
    /// Question for a test.
    /// </summary>
    public class Question : INotifyPropertyChanged
    {
        private long _id;
        public long QuestionId
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string QuestionText
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Answer> _answers;
        public ObservableCollection<Answer> Answers
        {
            get => _answers;
            set
            {
                _answers = value;
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

                CanSubmitChanged?.Invoke();
            }
        }

        public Action CanSubmitChanged { get; set; }

        public Question()
        {
            Answers = [];
        }

        public void AddAnswer(string answer)
        {
            Answers.Add(new Answer() { AnswerId = 0, AnswerText = answer });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
