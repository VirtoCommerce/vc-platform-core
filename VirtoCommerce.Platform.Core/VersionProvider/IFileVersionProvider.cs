namespace VirtoCommerce.Platform.Core.VersionProvider
{
    public interface IFileVersionProvider
    {
        string GetFileVersion(string fullPath);
    }
}
