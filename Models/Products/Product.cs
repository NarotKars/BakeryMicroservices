using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Models.Products
{
    public class Product
    {

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }
        public string Container { get; set; }
        public string BlobName { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
    }
}
