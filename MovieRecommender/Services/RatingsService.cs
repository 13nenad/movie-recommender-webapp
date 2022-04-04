using Microsoft.Azure.Cosmos;
using MovieRecommender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRecommender.Services
{
    public class RatingsService
    {
        private Container _container;

        public RatingsService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task UpdateRatingAsync(Rating rating)
        {
            try
            {
                await _container.UpsertItemAsync(rating, new PartitionKey(rating.UserId));
            }
            catch (Exception e)
            {
                throw (e);   
            }
        }

        public async Task DeleteRatingAsync(string id, int userId)
        {
            await _container.DeleteItemAsync<Rating>(id, new PartitionKey(userId));
        }

        public async Task<Rating> GetRatingAsync(string id, int userId)
        {
            try
            {
                ItemResponse<Rating> response = await _container.ReadItemAsync<Rating>(id, new PartitionKey(userId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Rating>> GetCurrentUserRatingsAsync(int userId)
        {
            string queryString = "SELECT * FROM c WHERE c.userId = " + userId;
            var query = _container.GetItemQueryIterator<Rating>(new QueryDefinition(queryString));
            List<Rating> results = new List<Rating>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<int> GetNumRatingsForMovieAsync(int movieId)
        {
            string queryString = "SELECT * FROM c WHERE c.movieId = " + movieId;
            var query = _container.GetItemQueryIterator<Rating>(new QueryDefinition(queryString));
            List<Rating> results = new List<Rating>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results.Count();
        }
    }
}