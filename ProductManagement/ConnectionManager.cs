using Azure.Storage.Blobs;
using Microsoft.Data.SqlClient;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using MongoDB.Driver;
using System.Security.Authentication;
using System.Text;

namespace ProductManagement
{
    public class ConnectionManager
    {
        private static string ConnectionString { get; set; }

        public static void CheckConnection()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException)
                {
                    throw new Exception("Incorrect username or password. Please try again");
                }
            }
        }

        public static SqlConnection CreateConnection(IConfiguration configuration)
        {
            ConnectionString = CreateConnectionStringSQL(configuration);
            return new SqlConnection(ConnectionString);
        }

        public static string CreateConnectionStringSQL(IConfiguration configuration)
        {
            SqlConnectionStringBuilder builder = new();
            builder.DataSource = configuration["DbConnectionString:DataSource"];
            builder.InitialCatalog = configuration["DbConnectionString:InitialCatalog"];
            builder.UserID = configuration["DbConnectionString:UserName"];
            builder.Password = Encoding.UTF8.GetString(Convert.FromBase64String(configuration["DbConnectionString:PasswordHash"]));
            return builder.ConnectionString;
        }
        public static BlobContainerClient GetBlobContainerClient(IConfiguration configuration, string container)
        {
            var storageCredentials = new StorageCredentials(configuration["BlobConnectionString:AccountName"]
                                                          , Encoding.UTF8.GetString(Convert.FromBase64String(configuration["BlobConnectionString:AccountKeyHash"])));

            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            var blobServiceClient = new BlobServiceClient(storageAccount.ToString(true));
            if (!blobServiceClient.GetBlobContainerClient(container).Exists())
            {
                blobServiceClient.CreateBlobContainer(container, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            return new BlobContainerClient(storageAccount.ToString(true), container);
        }

        public static IMongoDatabase GetMongoDb(IConfiguration configuration)
        {
            return GetMongoClient(configuration).GetDatabase("products");
        }

        public static MongoClient GetMongoClient(IConfiguration configuration)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(configuration["MongoDbConnectionString"])
            );
            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            return new MongoClient(settings);
        }
    }
}