using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapService :  ISitemapService
    {
        protected Func<ISitemapRepository> _repositoryFactory { get; private set; }
        protected ISitemapItemService _sitemapItemService { get; private set; }

        public SitemapService(Func<ISitemapRepository> repositoryFactory, ISitemapItemService sitemapItemService)
        {
            _repositoryFactory = repositoryFactory;
            _sitemapItemService = sitemapItemService;
        }

        public virtual async Task<Sitemap> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id");
            }

            Sitemap sitemap = null;

            using (var repository = _repositoryFactory())
            {
                var sitemapEntity = repository.Sitemaps.FirstOrDefault(s => s.Id == id);
                if (sitemapEntity != null)
                {
                    sitemap = AbstractTypeFactory<Sitemap>.TryCreateInstance();
                    if (sitemap != null)
                    {
                        var sitemapItemsSearchResponse = await _sitemapItemService.SearchAsync(new SitemapItemSearchCriteria
                        {
                            SitemapId = sitemap.Id
                        });

                        sitemap = sitemapEntity.ToModel(sitemap);
                        // sitemap.Items = sitemapItemsSearchResponse.Results;
                        sitemap.TotalItemsCount = sitemapItemsSearchResponse.TotalCount;
                    }
                }

                return await Task.FromResult(sitemap);
            }
        }

        public virtual async Task<GenericSearchResult<Sitemap>> SearchAsync(SitemapSearchCriteria request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            using (var repository = _repositoryFactory())
            {
                var searchResponse = new GenericSearchResult<Sitemap>();

                var sitemapEntities = repository.Sitemaps;

                if (!string.IsNullOrEmpty(request.StoreId))
                {
                    sitemapEntities = sitemapEntities.Where(s => s.StoreId == request.StoreId);
                }
                if (!string.IsNullOrEmpty(request.Location))
                {
                    sitemapEntities = sitemapEntities.Where(s => s.Filename == request.Location);
                }

                searchResponse.TotalCount = sitemapEntities.Count();

                foreach (var sitemapEntity in sitemapEntities.OrderByDescending(s => s.CreatedDate).Skip(request.Skip).Take(request.Take))
                {
                    var sitemap = AbstractTypeFactory<Sitemap>.TryCreateInstance();
                    if (sitemap != null)
                    {
                        var sitemapItemsSearchResponse = await _sitemapItemService.SearchAsync(new SitemapItemSearchCriteria
                        {
                            SitemapId = sitemapEntity.Id
                        });

                        sitemap = sitemapEntity.ToModel(sitemap);
                        sitemap.TotalItemsCount = sitemapItemsSearchResponse.TotalCount;
                        searchResponse.Results.Add(sitemap);
                    }
                }

                return searchResponse;
            }
        }

        public virtual async Task SaveChangesAsync(Sitemap[] sitemaps)
        {
            if (sitemaps == null)
                throw new ArgumentNullException("sitemaps");

            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            {
                var sitemapIds = sitemaps.Where(s => !s.IsTransient()).Select(s => s.Id);
                var sitemapExistEntities = repository.Sitemaps.Where(s => sitemapIds.Contains(s.Id));
                foreach (var sitemap in sitemaps)
                {
                    var sitemapSourceEntity = AbstractTypeFactory<SitemapEntity>.TryCreateInstance();
                    if (sitemapSourceEntity != null)
                    {
                        sitemapSourceEntity.FromModel(sitemap, pkMap);
                        var sitemapTargetEntity = sitemapExistEntities.FirstOrDefault(s => s.Id == sitemap.Id);
                        if (sitemapTargetEntity != null)
                        {
                            sitemapSourceEntity.Patch(sitemapTargetEntity);
                        }
                        else
                        {
                            repository.Add(sitemapSourceEntity);
                        }
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual async Task RemoveAsync(string[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");
            
            using (var repository = _repositoryFactory())
            {
                var sitemapEntities = repository.Sitemaps.Where(s => ids.Contains(s.Id));
                foreach (var sitemapEntity in sitemapEntities)
                {
                    repository.Remove(sitemapEntity);
                }

                await repository.UnitOfWork.CommitAsync();
            }
        }
    }
}
