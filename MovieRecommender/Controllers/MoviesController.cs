using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Models;
using MovieRecommender.Services;
using System.Net;

namespace MovieRecommender.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MoviesService _moviesService;
        private readonly RatingsService _ratingService;
        private readonly UserPredictedRatingsService _predictionsService;

        public MoviesController(MoviesService moviesService, RatingsService ratingsService, UserPredictedRatingsService recoService)
        {
            _moviesService = moviesService;
            _ratingService = ratingsService;
            _predictionsService = recoService;
        }

        [ActionName("Login")]
        public IActionResult Login() { return View(); }

        [ActionName("Index")]
        public async Task<IActionResult> Index(int userId, string genre, string searchTerm)
        {
            if (userId == 0)
                return View(new MoviesViewModel() { Exception = "User does not exist" });

            var vm = new MoviesViewModel()
            {
                UserId = userId,
                GenreFilter = genre,
                SearchTermFilter = searchTerm,
                UsersRatings = await _ratingService.GetCurrentUserRatingsAsync(userId),
                Predictions = await _predictionsService.GetCurrentUserPredictionsAsync(userId)
            };
           
            List<Movie>? filteredMovies;
            if (genre != null)
            {
                if (genre == "No genres listed") genre = "(no genres listed)";

                filteredMovies = await _moviesService.GetMoviesAsync(
                    "SELECT * FROM c WHERE CONTAINS(c.genres, '" + genre + "')") as List<Movie>;
            }
            else if (searchTerm != null)
            {
                filteredMovies = await _moviesService.GetMoviesAsync(
                    "SELECT * FROM c WHERE CONTAINS(LOWER(c.title), '" + searchTerm.ToLower() + "')") as List<Movie>;
            }
            else
            {
                filteredMovies = await _moviesService.GetMoviesAsync(
                    "SELECT * FROM c WHERE c.released = 2018 OR c.released = 2017") as List<Movie>;
            }

            if (filteredMovies != null) vm.FilteredMovies = filteredMovies;

            if (vm.Predictions == null) // check if this is a new user
            {
                // If so, get movies that have the best average ratings
                if (await _moviesService.GetMoviesAsync("SELECT TOP 20 * FROM c WHERE c.avgRating = 5") is List<Movie> movies) 
                    vm.Recommendations = movies;
            }
            else
            {
                for (int i = 0; i < BaseViewModel.NumberOfRecommendations; i++)
                    vm.Recommendations.Add(await _moviesService.GetMovieAsync(vm.Predictions.MovieIds[i].ToString()));
            }
                             
            return View(vm);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(int id, int userId)
        {
            var vm = new MovieDetailsViewModel() { UserId = userId };
            vm.Movie = await _moviesService.GetMovieAsync(id.ToString());
            if (vm.Movie == null)
            {
                vm.Exception = "Movie could not be found";
                return View(vm);
            }

            var predictions = await _predictionsService.GetCurrentUserPredictionsAsync(userId);
            vm.PredictedRating = predictions.PredictedRatings[predictions.MovieIds.IndexOf(id)];

            vm.Rating = await _ratingService.GetRatingAsync(userId + "-" + id, userId);
            vm.NumOfRatings = await _ratingService.GetNumRatingsForMovieAsync(id);

            return View(vm);
        }

        [ActionName("UpdateRating")]
        public async Task<string> UpdateRatingAsync(int userId, int movieId, double ratingScore)
        {
            if (userId == 0)
                return "userId is 0";
            else if (movieId == 0)
                return "movieId is 0";
            else if (ratingScore == 0)
                return "ratingScore is 0";
            else if (ratingScore > 5)
                return "ratingScore is above 5";

            var rating = new Rating() { Id = userId.ToString() + "-" + movieId.ToString(), UserId = userId, MovieId = movieId, RatingScore = ratingScore };
            await _ratingService.UpdateRatingAsync(rating);

            return "Success";
        }

        [ActionName("DeleteRating")]
        public async Task<string> DeleteRatingAsync(string id)
        {
            await _ratingService.DeleteRatingAsync(id, int.Parse(id.Split("-")[0]));

            return "Success";
        }

        public async Task<IActionResult> AutoCompleteSuggestions(string searchTerm)
        {
            List<SearchResult> sr = new();
            var matchingMovies = await _moviesService.GetMoviesAsync(
                    "SELECT TOP 10 * FROM c WHERE CONTAINS(LOWER(c.title), '" + searchTerm.ToLower() + "')") as List<Movie>;

            if (matchingMovies != null)
                foreach (Movie movie in matchingMovies.Take(10))
                    sr.Add(new SearchResult { Name = movie.Title, MovieId = movie.MovieId });

            return new JsonResult(sr);
        }

        public string StartDatabricksJob(int id)
        {
            if (id == 0) return "UserId cannot be 0";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://australiasoutheast.azuredatabricks.net/api/2.0/jobs/run-now");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("Authorization", "Bearer " + "dapi4f49ef033c3635b700906c961a9a69ae");
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"job_id\": 461617387142607, \"notebook_params\": {\"userId\": " + id + "}}";
                streamWriter.Write(json);
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
            return "Success";
        }

        /* //This method is just a back up in case I can't get Spark to scrape all the poster link URLs
        private async void ScrapeTask(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                int result = 0;
                if (Int32.TryParse(_allMovies[i].PosterLink, out result))
                {
                    _allMovies[i].PosterLink = PosterScraper.ScrapeData(_allMovies[i].PosterLink);
                    await _moviesService.UpdateMovieAsync(_allMovies[i]);
                }
            }
        }*/
    }
}