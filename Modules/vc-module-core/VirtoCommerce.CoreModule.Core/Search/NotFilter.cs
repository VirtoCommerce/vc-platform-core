namespace VirtoCommerce.Domain.Search
{
    public class NotFilter : IFilter
    {
        public IFilter ChildFilter { get; set; }

        public override string ToString()
        {
            return ChildFilter != null ? $"NOT({ChildFilter})" : string.Empty;
        }
    }
}
