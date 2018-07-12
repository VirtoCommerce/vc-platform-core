namespace VirtoCommerce.SearchModule.Core.Model
{
    public class NumericRange
    {
        public decimal? Lower { get; set; }
        public decimal? Upper { get; set; }
        public bool IncludeLower { get; set; }
        public bool IncludeUpper { get; set; }
    }
}
