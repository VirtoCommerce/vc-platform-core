namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IBlobContentStorageProviderFactory
    {
        IBlobContentStorageProvider CreateProvider(string basePath);
    }
}
