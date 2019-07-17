using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExportModule.Core.Model
{

    public abstract class BaseExportPagedDataSource : IPagedDataSource
    {
        protected class FetchResult
        {
            public IEnumerable Results { get; set; }
            public int TotalCount { get; set; }

            public FetchResult(IEnumerable results, int totalCount)
            {
                Results = results;
                TotalCount = totalCount;
            }
        }

        public int PageSize { get; set; } = 50;
        public int CurrentPageNumber { get; private set; }
        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationPolicyProvider _authorizationPolicyProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        protected BaseExportPagedDataSource(IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
        {
            _authorizationService = authorizationService;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationPolicyProvider = authorizationPolicyProvider;
            _userManager = userManager;
        }

        protected abstract FetchResult FetchData(SearchCriteriaBase searchCriteria);

        /// <summary>
        /// Checks user passed in <see cref="DataQuery"/> for permissions before fetching data.
        /// Permissions should be attached to *ExportPagedDataSource class using <see cref="AuthorizeAttribute"/>
        /// </summary>
        /// <returns></returns>
        public virtual async Task Authorize()
        {
            var user = await _userManager.FindByNameAsync(DataQuery.UserName);
            var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);
            var policies = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true).Select(x => ((AuthorizeAttribute)x).Policy);
            foreach (var policyString in policies)
            {
                var policy = await _authorizationPolicyProvider.GetPolicyAsync(policyString);
                var allow = await _authorizationService.AuthorizeAsync(claimsPrincipal, null, policy.Requirements);
                if (!allow.Succeeded)
                {
                    throw new UnauthorizedAccessException(allow.Failure.FailedRequirements.FirstOrDefault().ToString());
                }
            }
        }

        public virtual IEnumerable FetchNextPage()
        {
            if (_searchCriteria == null)
            {
                _searchCriteria = DataQuery.ToSearchCriteria();
                Authorize().GetAwaiter().GetResult();
            }

            _searchCriteria.Skip = PageSize * CurrentPageNumber;
            _searchCriteria.Take = PageSize;

            var result = FetchData(_searchCriteria);
            _totalCount = result.TotalCount;
            CurrentPageNumber++;
            return result.Results;
        }

        public virtual int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                if (_searchCriteria == null)
                {
                    _searchCriteria = DataQuery.ToSearchCriteria();
                    Authorize().GetAwaiter().GetResult();
                }

                _searchCriteria.Skip = 0;
                _searchCriteria.Take = 0;

                var result = FetchData(_searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
        }
    }
}
