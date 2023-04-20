using Azure.Storage.Blobs;

namespace ProductManagement.Services
{
    public class BlobService
    {
        private readonly IConfiguration configuration;
        private readonly CategoriesService categoriesService;
        public BlobService(IConfiguration configuration, CategoriesService categoriesService)
        {
            this.configuration = configuration;
            this.categoriesService = categoriesService;
        }

        public async Task UploadToBlob(string localFilePath, string container, string blobName)
        {
            var containerClient = ConnectionManager.GetBlobContainerClient(configuration, container);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(localFilePath, true);
        }

        public async Task DeleteFromBlob(string container, string blobName)
        {
            var containerClient = ConnectionManager.GetBlobContainerClient(configuration, container);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
