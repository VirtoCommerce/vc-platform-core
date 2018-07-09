using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Core.Events;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Services
{
    public class SeoServiceImpl : ISeoService
    {
        private readonly Func<ICoreRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public SeoServiceImpl(Func<ICoreRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        public async Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            using (var repository = _repositoryFactory())
            {
                var objectIds = seoSupportObjects.Where(x => x.Id != null).Select(x => x.Id).Distinct().ToArray();

                var seoInfosEntities = await repository.SeoUrlKeywords.Where(x => objectIds.Contains(x.ObjectId)).ToArrayAsync();
                var seoInfos = seoInfosEntities.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();

                foreach (var seoSupportObject in seoSupportObjects)
                {
                    seoSupportObject.SeoInfos = seoInfos.Where(x => x.ObjectId == seoSupportObject.Id && x.ObjectType == seoSupportObject.SeoObjectType).ToList();
                }
            }
        }

        public async Task SaveSeoInfosAsync(SeoInfo[] seoinfos)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<SeoInfo>>();
            using (var repository = _repositoryFactory())
            {
                var alreadyExistSeoInfos = await repository.GetSeoByIdsAsync(seoinfos.Select(x => x.Id).ToArray());
                var target = new { SeoInfos = new ObservableCollection<Model.SeoUrlKeywordEntity>(alreadyExistSeoInfos) };
                var source = new
                {
                    SeoInfos = new ObservableCollection<Model.SeoUrlKeywordEntity>(seoinfos.Select(x =>
                        AbstractTypeFactory<Model.SeoUrlKeywordEntity>.TryCreateInstance().FromModel(x, pkMap)))
                };

                source.SeoInfos.Patch(target.SeoInfos, (sourceSeoUrlKeyword, targetSeoUrlKeyword) =>
                {
                    changedEntries.Add(new GenericChangedEntry<SeoInfo>(
                        sourceSeoUrlKeyword.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()),
                        targetSeoUrlKeyword.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()),
                        EntryState.Modified));
                    sourceSeoUrlKeyword.Patch(targetSeoUrlKeyword);
                });

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new SeoInfoChangedEvent(changedEntries));
            }
        }

        public async Task SaveSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            if (seoSupportObjects == null)
            {
                throw new ArgumentNullException(nameof(seoSupportObjects));
            }

            var changedEntries = new List<GenericChangedEntry<SeoInfo>>();
            var pkMap = new PrimaryKeyResolvingMap();
            foreach (var seoObject in seoSupportObjects.Where(x => x.Id != null))
            {
                var objectType = seoObject.SeoObjectType;

                using (var repository = _repositoryFactory())
                {
                    if (seoObject.SeoInfos != null)
                    {
                        // Normalize seoInfo
                        foreach (var seoInfo in seoObject.SeoInfos)
                        {
                            if (seoInfo.ObjectId == null)
                                seoInfo.ObjectId = seoObject.Id;

                            if (seoInfo.ObjectType == null)
                                seoInfo.ObjectType = objectType;
                        }
                    }

                    if (seoObject.SeoInfos != null)
                    {
                        var target = new
                        {
                            SeoInfos = new ObservableCollection<Model.SeoUrlKeywordEntity>(
                                await repository.GetObjectSeoUrlKeywordsAsync(objectType, seoObject.Id))
                        };
                        var source = new
                        {
                            SeoInfos = new ObservableCollection<Model.SeoUrlKeywordEntity>(
                                seoObject.SeoInfos.Select(x =>
                                    AbstractTypeFactory<Model.SeoUrlKeywordEntity>.TryCreateInstance()
                                        .FromModel(x, pkMap)))
                        };

                        var seoComparer = AnonymousComparer.Create((Model.SeoUrlKeywordEntity x) => x.Id ?? string.Join(":", x.StoreId, x.ObjectId, x.ObjectType, x.Language));
                        source.SeoInfos.Patch(target.SeoInfos, seoComparer, (sourceSeoUrlKeyword, targetSeoUrlKeyword) =>
                        {
                            changedEntries.Add(new GenericChangedEntry<SeoInfo>(
                                sourceSeoUrlKeyword.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()),
                                targetSeoUrlKeyword.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()),
                                EntryState.Modified));
                            sourceSeoUrlKeyword.Patch(targetSeoUrlKeyword);
                        });
                    }

                    await repository.UnitOfWork.CommitAsync();
                    pkMap.ResolvePrimaryKeys();
                    await _eventPublisher.Publish(new SeoInfoChangedEvent(changedEntries));
                }
            }
        }

        public async Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject)
        {
            if (seoSupportObject == null)
            {
                throw new ArgumentNullException(nameof(seoSupportObject));
            }

            if (seoSupportObject.Id != null)
            {
                var changedEntries = new List<GenericChangedEntry<SeoInfo>>();

                using (var repository = _repositoryFactory())
                {

                    var objectType = seoSupportObject.SeoObjectType;
                    var objectId = seoSupportObject.Id;
                    var seoUrlKeywords = await repository.GetObjectSeoUrlKeywordsAsync(objectType, objectId);

                    foreach (var seoUrlKeyword in seoUrlKeywords)
                    {
                        changedEntries.Add(new GenericChangedEntry<SeoInfo>(
                            seoUrlKeyword.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance()),
                            EntryState.Deleted));
                        repository.Remove(seoUrlKeyword);
                    }

                    await repository.UnitOfWork.CommitAsync();
                    await _eventPublisher.Publish(new SeoInfoChangedEvent(changedEntries));
                }
            }
        }

        public async Task<IEnumerable<SeoInfo>> GetAllSeoDuplicatesAsync()
        {
            var retVal = new List<SeoInfo>();
            using (var repository = _repositoryFactory())
            {
                var dublicateSeoRecords = await repository.SeoUrlKeywords.GroupBy(x => x.Keyword + ":" + x.StoreId)
                                                    .Where(x => x.Count() > 1)
                                                    .SelectMany(x => x)
                                                    .ToArrayAsync();
                retVal.AddRange(dublicateSeoRecords.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())));
            }
            return retVal;
        }


        public async Task<IEnumerable<SeoInfo>> GetSeoByKeywordAsync(string keyword)
        {
            using (var repository = _repositoryFactory())
            {
                // Find seo entries for specified keyword. Also add other seo entries related to found object.
                var query = await repository.SeoUrlKeywords
                    .Where(x => x.Keyword == keyword)
                    .Join(repository.SeoUrlKeywords, x => new { x.ObjectId, x.ObjectType }, y => new { y.ObjectId, y.ObjectType }, (x, y) => y)
                    .ToArrayAsync();

                var result = query.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();
                return result;
            }
        }
    }
}
