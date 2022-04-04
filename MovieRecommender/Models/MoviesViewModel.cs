namespace MovieRecommender.Models
{
    using System.Collections.Generic;

    public class MoviesViewModel : BaseViewModel
    {
        public MoviesViewModel()
        {
            Recommendations = new List<Movie>();
            FilteredMovies = new List<Movie>();
        }

        public List<Movie> FilteredMovies { get; set; }

        public List<Movie> Recommendations { get; set; }

        public Predictions? Predictions { get; set; }

        public IEnumerable<Rating>? UsersRatings { get; set; }

        public string? SearchTermFilter { get; set; }

        public string? GenreFilter { get; set; }
    }
}