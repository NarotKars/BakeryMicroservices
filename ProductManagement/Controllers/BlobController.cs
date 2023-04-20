using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Security.Principal;
using System.Text;
using ProductManagement.Services;

namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlobController : ControllerBase
    {
        private const string baseUrl = "https://bakery9.blob.core.windows.net";
        private readonly IConfiguration configuration;
        private readonly BlobService blobService;
        public BlobController(IConfiguration configuration, BlobService blobService)
        {
            this.configuration = configuration;
            this.blobService = blobService;
        }

        [HttpGet("{container}/{blobName}")]
        public async Task<Stream> GetBlob(string container, string blobName)
        {
            var client = new Client();
            var url = baseUrl + $"/{container}/{blobName}";
            return await client.GetStreamAsync(HttpMethod.Get, string.Format(url, container, blobName));
        }
    }
}
