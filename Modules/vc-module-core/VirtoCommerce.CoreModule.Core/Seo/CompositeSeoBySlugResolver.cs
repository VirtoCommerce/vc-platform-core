using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.CoreModule.Core.Seo
{
    public class CompositeSeoBySlugResolver : ISeoBySlugResolver
    {
        private readonly IEnumerable<ISeoBySlugResolver> _resolvers;
        public CompositeSeoBySlugResolver(IEnumerable<ISeoBySlugResolver> resolvers)
        {
            _resolvers = resolvers;
        }
        #region ISeoBySlugResolver members
        public async Task<SeoInfo[]> FindSeoBySlugAsync(string slug)
        {
            var result = Array.Empty<SeoInfo>();
            if (!string.IsNullOrEmpty(slug))
            {
                var tasks = _resolvers.Select(x => x.FindSeoBySlugAsync(slug)).ToArray();
                result = (await Task.WhenAll(tasks)).SelectMany(x => x).Where(x => x.ObjectId != null && x.ObjectType != null).Distinct().ToArray();
            }
            return result;
        }
        #endregion
    }
}
