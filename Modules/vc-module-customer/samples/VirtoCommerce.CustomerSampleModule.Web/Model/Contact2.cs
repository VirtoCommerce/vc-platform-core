using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.CustomerSampleModule.Web.Model
{
    public class Contact2 : Contact
    {
        public Contact2()
        {
            base.MemberType = typeof(Contact).Name;
        }
        public string JobTitle { get; set; }
    }
}
