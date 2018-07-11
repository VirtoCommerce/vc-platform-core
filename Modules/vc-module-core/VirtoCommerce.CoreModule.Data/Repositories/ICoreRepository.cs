using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Data.Currency;
using VirtoCommerce.CoreModule.Data.Model;
using VirtoCommerce.CoreModule.Data.Package;
using VirtoCommerce.CoreModule.Data.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public interface ICoreRepository : IRepository
    {
        IQueryable<SeoUrlKeywordEntity> SeoUrlKeywords { get; }
        IQueryable<SequenceEntity> Sequences { get; }
        IQueryable<CurrencyEntity> Currencies { get; }
        IQueryable<PackageTypeEntity> PackageTypes { get; }

        Task<SeoUrlKeywordEntity[]> GetSeoByIdsAsync(string[] ids);
        Task<SeoUrlKeywordEntity[]> GetObjectSeoUrlKeywordsAsync(string objectType, string objectId);
    }
}
