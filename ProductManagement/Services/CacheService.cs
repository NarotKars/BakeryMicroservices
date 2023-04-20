using System.Text.Json;
using Models.Products;
using ProductManagement.Services;
using StackExchange.Redis;

public class ProductsCacheWrapper
{
    private readonly IDatabase redisDb;
    private readonly ProductsService productsService;

    public ProductsCacheWrapper(ProductsService productsService)
    {
        this.productsService = productsService;
        this.redisDb = ConnectionMultiplexer.Connect("localhost").GetDatabase();
    }

    public void CacheProducts(IEnumerable<Product> products)
    {
        var productsJson = JsonSerializer.Serialize(products);
        redisDb.StringSet("products", productsJson);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        var productsJson = await redisDb.StringGetAsync("products");
        if (productsJson.HasValue)
        {
            return JsonSerializer.Deserialize<IEnumerable<Product>>(productsJson);
        }
        else
        {
            return await this.productsService.GetProductsFromDb();
        }
        return null;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryId(string id)
    {
        var productsJson = await redisDb.StringGetAsync("products");
        IEnumerable<Product> products = null;
        if (productsJson.HasValue)
        {
            products = JsonSerializer.Deserialize<IEnumerable<Product>>(productsJson);
        }
        else
        {
            products = await this.productsService.GetProductsByCategoryIdFromDb(id);
        }
        return products.Where(p => p.CategoryId == id);
    }

    public async Task<Product> GetProductById(string id)
    {
        var productsJson = await redisDb.StringGetAsync("products");
        if (productsJson.HasValue)
        {
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(productsJson);
            var product = products.Where(p => p.Id == id);
            if(product.Any())
            {
                return product.First();
            }
        }

        return (await this.productsService.GetProductsByIdsFromDb(new List<string>() { id })).First();
    }

    public async Task UpdateCacheAsync()
    {
        var products = await this.productsService.GetProductsFromDb();
        CacheProducts(products);
    }

    public async Task PreCacheAsync()
    {
        var products = await this.productsService.GetProductsFromDb();
        CacheProducts(products);
    }
}
