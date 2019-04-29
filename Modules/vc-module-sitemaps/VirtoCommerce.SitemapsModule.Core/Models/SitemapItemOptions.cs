namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItemOptions
    {
        public string UpdateFrequency { get; set; } = Models.UpdateFrequency.Weekly;
        public decimal Priority { get; set; } = .5M;
    }
}
