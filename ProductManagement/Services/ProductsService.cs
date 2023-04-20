using Models.Products;
using Models.Orders;
using ProductManagement.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace ProductManagement.Services
{
    public class ProductsService
    {
        private readonly IConfiguration configuration;
        private readonly BlobService blobService;

        public ProductsService(IConfiguration configuration, BlobService blobService)
        {
            this.configuration = configuration;
            this.blobService = blobService;
        }

        public async Task<IEnumerable<Product>> GetProductsFromDb()
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            return (await collection.FindAsync(new BsonDocument())).ToEnumerable();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdFromDb(string categoryId)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            var filter = Builders<Product>.Filter.Eq(nameof(Product.CategoryId), categoryId);
            return (await collection.FindAsync(filter)).ToEnumerable();
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsFromDb(IEnumerable<string> ids)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            var filter = Builders<Product>.Filter.In(nameof(Product.Id), ids);
            return (await collection.FindAsync(filter)).ToEnumerable();
        }

        public async Task<Product> AddProduct(ProductUploadParams productUploadParams)
        {
            await this.blobService.UploadToBlob(productUploadParams.LocalFilePath, productUploadParams.Container, productUploadParams.BlobName);
            var product = new Product()
            {
                Container = productUploadParams.Container,
                BlobName = productUploadParams.BlobName,
                CategoryId = productUploadParams.CategoryId,
                Description = productUploadParams.Description,
                Price = productUploadParams.Price,
                Quantity = productUploadParams.Quantity
            };
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            await collection.InsertOneAsync(product);
            return product;
        }

        public async Task<Product> DeleteProduct(string id)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            var filter = Builders<Product>.Filter.Eq(nameof(Product.Id), id);
            var product = (await collection.FindAsync(filter)).FirstOrDefault();
            if (product == default)
            {
                throw new RESTException("There is no product with the specified id", System.Net.HttpStatusCode.NotFound);
            }
            await this.blobService.DeleteFromBlob(product.Container, product.BlobName);
            await collection.DeleteOneAsync(filter);
            return product;
        }

        public async Task UpdateProduct(string id, ProductUploadParams productUploadParams)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Product>("Products");
            var filter = Builders<Product>.Filter.Eq(nameof(Product.Id), id);
            var product = (await collection.FindAsync(filter)).FirstOrDefault();
            if (product == default)
            {
                throw new RESTException("There is no product with the specified id", System.Net.HttpStatusCode.NotFound);
            }

            await this.blobService.DeleteFromBlob(product.Container, product.BlobName);
            await this.blobService.UploadToBlob(productUploadParams.LocalFilePath, productUploadParams.Container, productUploadParams.BlobName);

            var update = Builders<Product>.Update.Set(nameof(Product.Container), productUploadParams.Container)
                                                 .Set(nameof(Product.BlobName), productUploadParams.BlobName);

            if (product.CategoryId != productUploadParams.CategoryId)
            {
                update = update.Set(nameof(Product.CategoryId), productUploadParams.CategoryId);
            }

            if(product.Description != productUploadParams.Description)
            {
                update = update.Set(nameof(Product.Description), productUploadParams.Description);
            }

            if (product.Price != productUploadParams.Price)
            {
                update = update.Set(nameof(Product.Price), productUploadParams.Price);
            }

            if (product.Quantity != productUploadParams.Quantity)
            {
                update = update.Set(nameof(Product.Quantity), productUploadParams.Quantity);
            }

            await collection.UpdateOneAsync(filter, update);
        }
    }
}