using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class SearchOptions
    {
        [Required]
        public string Provider { get; set; }
        [Required]
        public string Scope { get; set; }
    }
}
