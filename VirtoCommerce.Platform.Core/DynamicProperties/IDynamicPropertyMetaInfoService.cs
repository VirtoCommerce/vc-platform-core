using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IDynamicPropertyMetaInfoService
    {
        /// <summary>
        /// Deep loads and populate dynamic properties values for objects
        /// </summary>
        /// <param name="owner"></param>
        Task ResolveDynamicPropertyMetaInfoAsync(params IHasDynamicProperties[] owner);
    }
}
