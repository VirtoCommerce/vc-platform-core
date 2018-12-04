namespace VirtoCommerce.Platform.Core.Extensions
{
    public static class ModuleRelativePathExtension
    {
        public static string GetRelativeFilePath(this string targetString, string moduleRootFolderName, string moduleFolderName)
        {
            return targetString
                .Replace(moduleRootFolderName + "/", string.Empty)
                .Replace(moduleFolderName + "/", string.Empty);
        }
    }
}
