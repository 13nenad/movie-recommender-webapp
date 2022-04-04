namespace MovieRecommender.Models
{
    using Newtonsoft.Json;

    public class Rating
    {
        // This is the userId concatenated with movieId
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "movieId")]
        public int MovieId { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public double RatingScore { get; set; }
    }
}