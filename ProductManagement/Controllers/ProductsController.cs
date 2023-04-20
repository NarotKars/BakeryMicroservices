using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Models.Products;
using ProductManagement.Services;
using System.Collections;
using Models.Orders;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService productsService;
        private readonly ProductsCacheWrapper cacheService;

        public ProductsController(ProductsService productsService, ProductsCacheWrapper cacheService)
        {
            this.productsService = productsService;
            this.cacheService = cacheService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await this.cacheService.GetProducts();
        }

        [HttpGet("ByCategory/{id}")]
        public async Task<IEnumerable<Product>> GetProductsByCategoryId(string id)
        {
            return await this.cacheService.GetProductsByCategoryId(id);
        }

        [HttpGet("{id}")]
        public async Task<Product> GetProductById(string id)
        {
            return await this.cacheService.GetProductById(id);
        }

        [HttpPost("GetByIds")]
        public async Task<IEnumerable<Product>> GetProductsByIds(IEnumerable<string> ids)
        {
            return await this.productsService.GetProductsByIdsFromDb(ids);
        }

        [HttpPost("Add")]
        public async Task<Product> AddProduct(ProductUploadParams productUploadParams)
        {
            return await this.productsService.AddProduct(productUploadParams);
        }

        [HttpDelete("{id}")]
        public async Task<Product> DeleteProduct(string id)
        {
            return await this.productsService.DeleteProduct(id);
        }

        [HttpPut("{id}")]
        public async Task UpdateProduct(string id, ProductUploadParams productUploadParams)
        {
            await this.productsService.UpdateProduct(id, productUploadParams);
        }
    }
}
