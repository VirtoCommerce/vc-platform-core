using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    // These permissions required to fetch data
    [Authorize(ModuleConstants.Security.Permissions.Export)]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public class PricelistAssignmentExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private ViewableEntityConverter<PricelistAssignment> _viewableEntityConverter;

        public PricelistAssignmentExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            PricelistAssignment[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricelistAssignmentsByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).Result;
                totalCount = result.Length;
            }
            else
            {
                var pricelistAssignmentSearchResult = _searchService.SearchPricelistAssignmentsAsync((PricelistAssignmentsSearchCriteria)searchCriteria).Result;
                result = pricelistAssignmentSearchResult.Results.ToArray();
                totalCount = pricelistAssignmentSearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is PricelistAssignment pricelistAssignment))
            {
                throw new System.InvalidCastException(nameof(PricelistAssignment));
            }

            EnsureViewableConverterCreated();

            return _viewableEntityConverter.ToViewableEntity(pricelistAssignment);
        }

        protected virtual void EnsureViewableConverterCreated()
        {
            if (_viewableEntityConverter == null)
            {
                _viewableEntityConverter = new ViewableEntityConverter<PricelistAssignment>(x => $"{x.Name}", x => x.Id, x => null);
            }
        }
    }
}
