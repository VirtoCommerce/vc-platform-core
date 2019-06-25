using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchOptions
    {
        [Required]
        public string Path { get; set; }
    }
}
