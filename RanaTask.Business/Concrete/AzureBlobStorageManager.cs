using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Concrete
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly CloudBlobContainer _container;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlob:ConnectionString"];
            var containerName = configuration["AzureBlob:ContainerName"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(containerName);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var blob = _container.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(fileStream);
            return blob.Uri.ToString();
        }

        public async Task DeleteAsync(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            var blob = _container.GetBlockBlobReference(uri.Segments.Last());
            await blob.DeleteIfExistsAsync();
        }
    }
}
