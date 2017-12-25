using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Web.Infrastructure;

namespace VirtoCommerce.Platform.Web.Middelware
{
    public class VirtualFoldersUrlRewriteRule: IRule
    {     
        private readonly VirtualFolderOptions _options;

        public VirtualFoldersUrlRewriteRule(VirtualFolderOptions options)
        {
            _options = options;
        }

        public void ApplyRule(RewriteContext context)
        {
            var requestPath = context.HttpContext.Request.Path;

            foreach (var pair in _options.Items)
            {
                if (requestPath.StartsWithSegments(pair.Key, out var remainingPath))
                {
                    context.HttpContext.Request.Path = new PathString(pair.Value + remainingPath.Value);
                    break;
                }
            }
            context.Result = RuleResult.ContinueRules;
        }        
    }    
}
