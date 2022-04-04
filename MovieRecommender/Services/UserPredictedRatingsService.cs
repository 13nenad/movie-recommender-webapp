using Microsoft.Azure.Cosmos;
using MovieRecommender.Models;
using System.Threading.Tasks;

namespace MovieRecommender.Services
{
    public class UserPredictedRatingsService
    {
        private Container _container;

        public UserPredictedRatingsService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<Predictions> GetCurrentUserPredictionsAsync(int userId)
        {
            try
            {
                ItemResponse<Predictions> response = 
                    await _container.ReadItemAsync<Predictions>(userId.ToString(), new PartitionKey(userId.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        } 
    }
}