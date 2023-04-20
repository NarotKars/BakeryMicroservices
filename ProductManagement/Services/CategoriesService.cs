using Models.Products;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ProductManagement.Services
{
    public class CategoriesService
    {
        private readonly IConfiguration configuration;

        public CategoriesService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Category>("Categories");
            return (await collection.FindAsync(new BsonDocument())).ToEnumerable();
        }

        public async Task<Category> GetCategoryById(string id)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Category>("Products");
            var filter = Builders<Category>.Filter.Eq(nameof(Category.Id), id);
            return (await collection.FindAsync(filter)).First();
        }
    }
}
