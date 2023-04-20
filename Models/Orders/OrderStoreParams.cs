using Models.Enums;

namespace Models.Orders
{
    public class OrderStoreParams
    {
        public List<OrderDetail> Details { get; set; }
        public string AddressId { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Date { get; set; }
    }
}
