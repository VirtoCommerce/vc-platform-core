namespace VirtoCommerce.CustomerModule.Core.Model
{
    public class Organization : Member
    {
        public string Description { get; set; }
        public string BusinessCategory { get; set; }
        public string OwnerId { get; set; }
        public string ParentId { get; set; }

        public override string ObjectType => typeof(Organization).FullName;
    }
}
