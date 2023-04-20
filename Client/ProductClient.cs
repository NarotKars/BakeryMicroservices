using Models.Exceptions;
using Models.Orders;
using Models.Products;
using System.Net;

namespace Client
{
    public class ProductClient
    {
        private const string baseUrl = "https://bakery/api/Products";
        //private const string baseUrl = "https://localhost:44393/";

        public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<string> ids)
        {
            var client = new Client();
            var url = baseUrl + $"/GetByIds";
            return await client.GetResultAsync<IEnumerable<Product>>(HttpMethod.Post, url, ids);
        }

        public void CheckProductsAvailability(List<OrderDetail> orderDetails, IEnumerable<Product> products)
        {
            foreach (var orderDetail in orderDetails)
            {
                var product = products.Where(p => p.Id == orderDetail.ProductId).FirstOrDefault();
                if (product == null)
                {
                    throw new RESTException($"There is no product with the specified {orderDetail.ProductId} id", HttpStatusCode.NotFound);
                }

                if (product.Quantity < orderDetail.Quantity)
                {
                    throw new RESTException($"{product.Id} product is not available. \r\n available quantity - {product.Quantity} \r\n ordered quantity - {orderDetail.Quantity}", HttpStatusCode.NotFound);
                }
            }
        }
    }
}
