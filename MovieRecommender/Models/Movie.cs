namespace MovieRecommender.Models
{
    using Newtonsoft.Json;

    public class Movie
    {
        [JsonProperty(PropertyName = "id")]
        public string MovieId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "genres")]
        public string Genres { get; set; }

        [JsonProperty(PropertyName = "posterLink")]
        public string PosterLink { get; set; }

        [JsonProperty(PropertyName = "avgRating")]
        public double AvgRating { get; set; }

        [JsonProperty(PropertyName = "released")]
        public int? Released { get; set; }

        [JsonProperty(PropertyName = "imdbId")]
        public string ImdbId { get; set; }

        [JsonProperty(PropertyName = "tmdbId")]
        public string TmdbId { get; set; }
    }
}