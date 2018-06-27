namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchProviderSettings
    {

        public LuceneSearchProviderSettings(string dataDirectoryPath, string scope)
        {
            DataDirectoryPath = dataDirectoryPath;
            Scope = scope;
        }

        public string DataDirectoryPath { get; }
        public string Scope { get; }
    }
}
