using LiveCharts;

namespace TestHub
{
    public class QuestionPieSeries
    {
        public string QuestionText { get; set; }
        public SeriesCollection SeriesCollection { get; set; }

        public QuestionPieSeries()
        {
            SeriesCollection = [];
        }
    }
}
