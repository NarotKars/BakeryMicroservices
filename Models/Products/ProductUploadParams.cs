namespace Models.Products
{
    public class ProductUploadParams
    {
        public string LocalFilePath { get; set; }
        public string Container { get; set; }
        public string BlobName { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
    }
}
