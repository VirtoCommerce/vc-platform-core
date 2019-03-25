namespace VirtoCommerce.SearchModule.Core.Model
{
    public class SearchSettings
    {
        public string Provider { get; set; }
        public SearchConnectionSettingsBase ConnectionSettings { get; set; }
    }
}
