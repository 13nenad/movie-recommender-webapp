namespace MovieRecommender.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Predictions
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "movieIds")]
        public List<int> MovieIds { get; set; }

        [JsonProperty(PropertyName = "predictedRatings")]
        public List<double> PredictedRatings { get; set; }
    }
}