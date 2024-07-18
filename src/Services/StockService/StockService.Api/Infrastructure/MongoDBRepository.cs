using MongoDB.Driver;

namespace StockService.Api.Infrastructure
{
    public class MongoDBRepository
    {
        readonly IMongoDatabase _database;
        public MongoDBRepository(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("StockDB");
        }
        public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
    }
}
