using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestHub
{
    /// <summary>
    /// Answer to a question in a test.
    /// </summary>
    public class Answer : INotifyPropertyChanged
    {
        private long _id;
        public long AnswerId
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _text;
        public string AnswerText
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
