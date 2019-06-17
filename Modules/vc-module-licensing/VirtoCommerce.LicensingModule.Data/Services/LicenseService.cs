using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.LicensingModule.Data.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly Func<ILicenseRepository> _licenseRepositoryFactory;
        private readonly IChangeLogSearchService _changeLogSearchService;
        private readonly IEventPublisher _eventPublisher;
        private readonly LicenseOptions _licenseOptions;

        public LicenseService(Func<ILicenseRepository> licenseRepositoryFactory, IChangeLogSearchService changeLogSearchService, IEventPublisher eventPublisher
            , IOptions<LicenseOptions> licenseOptions)
        {
            _licenseRepositoryFactory = licenseRepositoryFactory;
            _changeLogSearchService = changeLogSearchService;
            _eventPublisher = eventPublisher;
            _licenseOptions = licenseOptions.Value;
        }

        public async Task<LicenseSearchResult> SearchAsync(LicenseSearchCriteria criteria)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                var result = AbstractTypeFactory<LicenseSearchResult>.TryCreateInstance();
                var query = repository.Licenses;

                if (criteria.Keyword != null)
                {
                    query = query.Where(x => x.CustomerName.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<License>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);
                var arrayLicense = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();

                result.TotalCount = await query.CountAsync();
                result.Results = arrayLicense.Select(x => x.ToModel(AbstractTypeFactory<License>.TryCreateInstance())).ToArray();

                return result;
            }
        }

        public async Task<License[]> GetByIdsAsync(string[] ids)
        {
            var result = Array.Empty<License>();

            using (var repository = _licenseRepositoryFactory())
            {
                var arrayLicense = await repository.GetByIdsAsync(ids);
                result = arrayLicense
                    .Select(x =>
                    {
                        var retVal = x.ToModel(AbstractTypeFactory<License>.TryCreateInstance());
                        return retVal;
                    }).ToArray();
            }
            var searchResult = await _changeLogSearchService.SearchAsync(new ChangeLogSearchCriteria { ObjectIds = ids, ObjectType = typeof(License).Name, Take = int.MaxValue });
            foreach (var license in result)
            {
                license.OperationsLog = searchResult.Results.Where(x => x.ObjectId == license.Id).ToList();
            }
            return result;
        }

        public async Task SaveChangesAsync(License[] licenses)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<License>>();
            using (var repository = _licenseRepositoryFactory())
            {

                var existingEntities = await repository.GetByIdsAsync(licenses.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var entity in licenses)
                {
                    //ensure that ActivationCode is filled
                    if (string.IsNullOrEmpty(entity.ActivationCode))
                    {
                        entity.ActivationCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                    }

                    var originalEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                    var originalLicense = originalEntity != null ? originalEntity.ToModel(AbstractTypeFactory<License>.TryCreateInstance()) : entity;
                    var sourceEntity = AbstractTypeFactory<LicenseEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(entity, pkMap);
                        var targetEntity = existingEntities.FirstOrDefault(x => x.Id == entity.Id);
                        if (targetEntity != null)
                        {
                            changedEntries.Add(new GenericChangedEntry<License>(entity, originalLicense, EntryState.Modified));
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                            changedEntries.Add(new GenericChangedEntry<License>(entity, EntryState.Added));
                        }
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new LicenseChangedEvent(changedEntries));
                pkMap.ResolvePrimaryKeys();
            }
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                await repository.RemoveByIdsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
            }
        }

        public async Task<string> GetSignedLicenseAsync(string code, string clientIpAddress, bool isActivated)
        {
            string result = null;

            var licenseEntity = await GetByCodeAsync(code);
            if (licenseEntity != null)
            {
                var license = new
                {
                    licenseEntity.Type,
                    licenseEntity.CustomerName,
                    licenseEntity.CustomerEmail,
                    licenseEntity.ExpirationDate,
                };

                var licenseString = JsonConvert.SerializeObject(license);
                var signature = await CreateSignatureAsync(licenseString);

                result = string.Join("\r\n", licenseString, signature);

                //Raise event
                var activateEvent = new LicenseSignedEvent(licenseEntity.ToModel(AbstractTypeFactory<License>.TryCreateInstance()), clientIpAddress, isActivated);
                await _eventPublisher.Publish(activateEvent);
            }

            return result;
        }


        private async Task<LicenseEntity> GetByCodeAsync(string code)
        {
            using (var repository = _licenseRepositoryFactory())
            {
                return await repository.Licenses.FirstOrDefaultAsync(x => x.ActivationCode == code);
            }
        }

        private async Task<string> CreateSignatureAsync(string data)
        {
            var hashAlgorithmName = HashAlgorithmName.SHA256.Name;

            using (var hashAlgorithm = SHA256.Create())
            using (var rsa = new RSACryptoServiceProvider())
            {
                // TODO: Store private key in a more secure storage, for example in Azure Key Vault
                var privateKeyFilePath = Path.GetFullPath(_licenseOptions.LicensePrivateKeyPath);
                var privateKey = await ReadFileWithKey(privateKeyFilePath);
                if (!string.IsNullOrEmpty(privateKey))
                {
                    rsa.FromXmlStringCustom(privateKey);
                }

                var signatureFormatter = new RSAPKCS1SignatureFormatter(rsa);
                signatureFormatter.SetHashAlgorithm(hashAlgorithmName);

                var dataBytes = Encoding.UTF8.GetBytes(data);
                var dataHash = hashAlgorithm?.ComputeHash(dataBytes) ?? new byte[0];
                var signatureBytes = signatureFormatter.CreateSignature(dataHash);
                var signature = Convert.ToBase64String(signatureBytes);

                return signature;
            }
        }

        private static async Task<string> ReadFileWithKey(string path)
        {
            if (!File.Exists(path))
            {
                throw new LicenseOrKeyNotFoundException(path);
            }

            return await File.ReadAllTextAsync(path);
        }
    }
}
