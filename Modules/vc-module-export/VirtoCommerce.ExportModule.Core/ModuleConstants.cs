namespace VirtoCommerce.ExportModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "export:access";
                public const string Download = "export:download";

                public static readonly string[] AllPermissions = { Access, Download };
            }
        }
    }
}
