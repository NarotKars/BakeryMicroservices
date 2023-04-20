using MongoDB.Bson;

namespace Models.Orders
{
    public class OrderDetail
    {
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
