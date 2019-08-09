using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Web.Authorization
{
    public class ExportAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public ExportAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;

        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var requirements = new List<IAuthorizationRequirement>()
            {
                new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Export), new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Read)
            };

            var exportPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(requirements.ToArray())
                .Build();

            _options.AddPolicy(typeof(Price).FullName + "ExportDataPolicy", exportPolicy);
            _options.AddPolicy(typeof(PricelistAssignment).FullName + "ExportDataPolicy", exportPolicy);
            _options.AddPolicy(typeof(Pricelist).FullName + "ExportDataPolicy", exportPolicy);

            return exportPolicy;
        }
    }
}
