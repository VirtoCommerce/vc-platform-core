namespace VirtoCommerce.Platform.Core.Common
{

    public class TenantIdentity : ValueObject
    {
        public TenantIdentity(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public bool IsValid => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Type);

    }
}
