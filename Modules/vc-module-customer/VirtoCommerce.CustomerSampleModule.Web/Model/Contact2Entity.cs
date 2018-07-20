using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerSampleModule.Web.Model
{
    public class Contact2Entity : ContactEntity
    {
        [StringLength(128)]
        public string JobTitle { get; set; }
    }
}
