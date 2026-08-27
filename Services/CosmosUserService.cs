using Microsoft.Azure.Cosmos;

namespace CosmosCrudApi.Services
{
    public class CosmosUserService : IUserService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;
        private readonly IEmailService _emailService;

        public CosmosUserService(IConfiguration configuration, IEmailService emailService)
        {
            _emailService = emailService;
            string endpoint = configuration["CosmosDb:Endpoint"]!;
            string key = configuration["CosmosDb:Key"]!;
            string databaseName = configuration["CosmosDb:DatabaseName"]!;
            string containerName = configuration["CosmosDb:ContainerName"]!;

            CosmosClient client = new CosmosClient(endpoint, key);

            Database database = client
                .CreateDatabaseIfNotExistsAsync(databaseName)
                .GetAwaiter()
                .GetResult();

            _container = database
                .CreateContainerIfNotExistsAsync(
                    containerName,
                    "/id")
                .GetAwaiter()
                .GetResult();
        }

        // CREATE
        public async Task<CosmosCrudApi.Models.User> CreateUserAsync(
            CosmosCrudApi.Models.User user)
        {
            if (string.IsNullOrEmpty(user.id))
            {
                user.id = Guid.NewGuid().ToString();
            }

            ItemResponse<CosmosCrudApi.Models.User> response =
                await _container.CreateItemAsync(
                    user,
                    new PartitionKey(user.id));

            await _emailService.SendRegistrationEmailAsync(
                user.email,
                user.name);

            return response.Resource;

        }

        // READ ALL
        public async Task<List<CosmosCrudApi.Models.User>> GetUsersAsync()
        {
            List<CosmosCrudApi.Models.User> users =
                new List<CosmosCrudApi.Models.User>();

            QueryDefinition query =
                new QueryDefinition("SELECT * FROM c");

            using FeedIterator<CosmosCrudApi.Models.User> resultSet =
                _container.GetItemQueryIterator<CosmosCrudApi.Models.User>(
                    query);

            while (resultSet.HasMoreResults)
            {
                FeedResponse<CosmosCrudApi.Models.User> response =
                    await resultSet.ReadNextAsync();

                users.AddRange(response);
            }

            return users;
        }

        // READ BY ID
        public async Task<CosmosCrudApi.Models.User?> GetUserByIdAsync(
            string id)
        {
            try
            {
                ItemResponse<CosmosCrudApi.Models.User> response =
                    await _container.ReadItemAsync<CosmosCrudApi.Models.User>(
                        id,
                        new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        // UPDATE
        public async Task<CosmosCrudApi.Models.User?> UpdateUserAsync(
            string id,
            CosmosCrudApi.Models.User user)
        {
            try
            {
                user.id = id;

                ItemResponse<CosmosCrudApi.Models.User> response =
                    await _container.ReplaceItemAsync(
                        user,
                        id,
                        new PartitionKey(id));

                return response.Resource;
            }
            catch (CosmosException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        // DELETE
        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<CosmosCrudApi.Models.User>(
                    id,
                    new PartitionKey(id));

                return true;
            }
            catch (CosmosException ex)
                when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}