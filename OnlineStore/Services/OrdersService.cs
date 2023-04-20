using Models.Orders;
using Models.Products;
using System.Data;
using MongoDB.Driver;
using Client;
using System.Net;
using OrderManagement.Exceptions;

namespace OrderManagement.Services
{
    public class OrdersService
    {
        private readonly IConfiguration configuration;
        private readonly ProductClient client;

        public OrdersService(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new ProductClient();
        }

        public async Task<string> CreateOrder(OrderStoreParams orderParams)
        {
            var products = await this.client.GetProductsByIds(orderParams.Details.Select(detail => detail.ProductId));
            this.client.CheckProductsAvailability(orderParams.Details, products);

            var client = ConnectionManager.GetMongoClient(configuration);
            var mongoDb = client.GetDatabase("bakery");
            using var session = client.StartSession();

            var collection = mongoDb.GetCollection<Order>("Orders");

            var order = new Order()
            {
                Details = orderParams.Details,
                AddressId = orderParams.AddressId,
                OrderDate = orderParams.Date,
                Status = orderParams.Status,
                UserId = orderParams.UserId
            };
            await collection.InsertOneAsync(order);

            var productsColleection = mongoDb.GetCollection<Product>("Products");
            foreach (var orderDetails in orderParams.Details)
            {
                var update = Builders<Product>.Update.Inc(nameof(Product.Quantity), -orderDetails.Quantity);
                var filter = Builders<Product>.Filter.Eq(nameof(Product.Id), orderDetails.ProductId);
                await productsColleection.UpdateOneAsync(filter, update);
            }

            session.CommitTransaction();
            return order.Id;
        }

        public async Task<Order> GetOrder(string id)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Order>("Orders");
            var filter = Builders<Order>.Filter.Eq(nameof(Order.Id), id);
            return (await collection.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<IEnumerable<Order>> GetOrderByUserId(string userId)
        {
            var mongoDb = ConnectionManager.GetMongoDb(configuration);
            var collection = mongoDb.GetCollection<Order>("Orders");
            var filter = Builders<Order>.Filter.Eq(nameof(Order.UserId), userId);
            return (await collection.FindAsync(filter)).ToEnumerable();
        }
    }
}
