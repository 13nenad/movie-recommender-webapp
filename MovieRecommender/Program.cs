using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using MovieRecommender.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();

var cosmosDbSection = builder.Configuration.GetSection("CosmosDb");
CosmosClient cosmosClient = getCosmosClient(cosmosDbSection);
builder.Services.AddSingleton(InitializeMoviesClientInstanceAsync(cosmosDbSection, cosmosClient).GetAwaiter().GetResult());
builder.Services.AddSingleton(InitializeRatingsClientInstanceAsync(cosmosDbSection, cosmosClient).GetAwaiter().GetResult());
builder.Services.AddSingleton(InitializePredictionsClientInstanceAsync(cosmosDbSection, cosmosClient).GetAwaiter().GetResult());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();

static CosmosClient getCosmosClient(IConfigurationSection configurationSection)
{
    string account = configurationSection.GetSection("Account").Value;
    string key = configurationSection.GetSection("Key").Value;
    var clientBuilder = new CosmosClientBuilder(account, key);
    return clientBuilder.WithConnectionModeDirect().Build();
}

static async Task<MoviesService> InitializeMoviesClientInstanceAsync(
    IConfigurationSection configurationSection, CosmosClient cosmosClient)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("MoviesContainerName").Value;

    var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return new MoviesService(cosmosClient, databaseName, containerName);
}

static async Task<RatingsService> InitializeRatingsClientInstanceAsync(
    IConfigurationSection configurationSection, CosmosClient cosmosClient)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("RatingsContainerName").Value;

    var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/userId");

    return new RatingsService(cosmosClient, databaseName, containerName);
}

static async Task<UserPredictedRatingsService> InitializePredictionsClientInstanceAsync(
    IConfigurationSection configurationSection, CosmosClient cosmosClient)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("PredictionsContainerName").Value;

    var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return new UserPredictedRatingsService(cosmosClient, databaseName, containerName);
}
