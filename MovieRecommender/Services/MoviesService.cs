using Microsoft.Azure.Cosmos;
using MovieRecommender.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRecommender.Services
{
    public class MoviesService
    {
        private Container _container;

        public MoviesService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Movie>(new QueryDefinition(queryString));
            List<Movie> results = new List<Movie>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Movie> GetMovieAsync(string movieId)
        {
            try
            {
                ItemResponse<Movie> response = await _container.ReadItemAsync<Movie>(movieId, new PartitionKey(movieId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        // This method is a backup in case I can't convert the "poster ids" given by MovieLens to actual
        // poster links. This is done via the PosterScraper class in the Helpers folder.
        public async Task UpdateMovieAsync(Movie movie)
        {
            await _container.UpsertItemAsync(movie, new PartitionKey(movie.MovieId));
        }
    }
}