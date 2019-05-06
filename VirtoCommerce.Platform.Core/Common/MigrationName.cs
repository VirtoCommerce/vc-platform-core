namespace VirtoCommerce.Platform.Core.Common
{
    public class MigrationName
    {
        public static string GetUpdateV2MigrationName(string moduleName)
        {
            return $"20000000000000_Update{moduleName.Replace("VirtoCommerce.", "")}V2";
        }
    }
}
