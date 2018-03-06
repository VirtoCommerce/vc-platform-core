namespace VirtoCommerce.Platform.Core.Common
{

    public class Tenant : ValueObject
    {
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
