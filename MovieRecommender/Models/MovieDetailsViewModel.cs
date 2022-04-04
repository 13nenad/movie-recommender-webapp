namespace MovieRecommender.Models
{
    public class MovieDetailsViewModel : BaseViewModel
    {
        public Movie Movie { get; set; }

        public Rating Rating { get; set; }

        public double PredictedRating { get; set; }

        public int NumOfRatings { get; set; }
    }
}