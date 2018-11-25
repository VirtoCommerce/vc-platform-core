namespace VirtoCommerce.Platform.Core.FileVersionProvider
{
    public interface IFileVersionProvider
    {
        string GetFileVersion(string fullPath);
    }
}
