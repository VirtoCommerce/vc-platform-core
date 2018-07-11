using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Model;

namespace VirtoCommerce.CoreModule.Core.Services
{
    /// <summary>
    /// Used to detect seo duplicates within any object based on it inner structure and relationships (store, catalogs, categories etc)
    /// </summary>
    public interface ISeoDuplicatesDetector
    {
        IEnumerable<SeoInfo> DetectSeoDuplicates(string objectType, string objectId, IEnumerable<SeoInfo> allSeoDuplicates);
    }
}
