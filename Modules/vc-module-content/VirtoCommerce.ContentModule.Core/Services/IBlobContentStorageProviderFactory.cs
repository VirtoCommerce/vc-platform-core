using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IBlobContentStorageProviderFactory
    {
        IBlobContentStorageProvider CreateProvider(string basePath);
    }
}
