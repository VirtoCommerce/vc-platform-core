using System.Threading.Tasks;

namespace VirtoCommerce.CoreModule.Core.Seo
{
    public interface ISeoBySlugResolver
    {
        Task<SeoInfo[]> FindSeoBySlugAsync(string slug);
    }
}
