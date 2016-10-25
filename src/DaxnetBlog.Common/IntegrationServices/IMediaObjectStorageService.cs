using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.IntegrationServices
{
    /// <summary>
    /// Represents that the implemented classes are storages
    /// for storing media objects like images.
    /// </summary>
    public interface IMediaObjectStorageService
    {
        Task SaveAsync(string key, string base64);
    }
}
