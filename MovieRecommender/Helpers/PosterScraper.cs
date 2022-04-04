using System.Text.RegularExpressions;

namespace MovieRecommender.Helpers
{
    public class PosterScraper
    {
        const string MovieLensUrl = "https://www.imdb.com/title/tt";

        public static string ScrapeData(string posterId)
        {
            var response = new HttpClient().GetStringAsync(MovieLensUrl + posterId);
            Match m;

            try
            {
                m = Regex.Match(response.Result, "https://m.media-amazon.com/images/M([a-zA-Z0-9:/'@._,-]+)");
            }
            catch (AggregateException)
            {
                //throw new AggregateException("404 for: " + posterId);
                return "Unknown";
            }

            if (!m.Success)
            {
                //throw new Exception("Could not scrape posterId: " + posterId);
                return "Unknown";
            }

            return m.Groups[0].Value;
        }
    }
}
