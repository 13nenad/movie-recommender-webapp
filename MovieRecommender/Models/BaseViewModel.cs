namespace MovieRecommender.Models
{
    public class BaseViewModel
    {
        public string[] GenresColOne = { "Action", "Adventure", "Children", "Comedy", "Crime", "Documentary" };

        public string[] GenresColTwo = { "Drama", "Fantasy", "Film-Noir", "Horror", "Musical", "Mystery" };

        public string[] GenresColThree = { "Romance", "Sci-Fi", "Thriller", "War", "Western", "No genres listed" };

        public const int NumberOfRecommendations = 20; // the number of recommendations we want shown on the UI

        public int UserId { get; set; }

        public string? Exception { get; set; }
    }
}