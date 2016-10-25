using DaxnetBlog.Common.IntegrationServices;
using System;
using System.Threading.Tasks;

namespace DaxnetBlog.AzureServices
{
    public class AzureBlobStorageService : IMediaObjectStorageService
    {
        private readonly string storageAccount;
        private readonly string accessKey;

        public AzureBlobStorageService(string storageAccount, string accessKey)
        {
            this.storageAccount = storageAccount;
            this.accessKey = accessKey;
        }

        public Task SaveAsync(string key, string base64)
        {
            throw new NotImplementedException();
        }
    }
}
