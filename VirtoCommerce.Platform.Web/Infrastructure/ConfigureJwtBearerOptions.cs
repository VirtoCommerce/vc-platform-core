using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
namespace VirtoCommerce.Platform.Web.Infrastructure
{
    public class ConfigureJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Authorization _authorization;

        public ConfigureJwtBearerOptions(IHttpContextAccessor httpContextAccessor, IOptions<Authorization> authorization)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorization = authorization.Value;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
        {
            if (string.IsNullOrEmpty(_authorization.Authority))
            {
                var host = _httpContextAccessor.HttpContext.Request.Host.ToString();
                var protocol = _httpContextAccessor.HttpContext.Request.Scheme;

                var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.HasValue ?
                        _httpContextAccessor.HttpContext.Request.PathBase.ToString()
                        :
                        null;

                options.Authority = $"{protocol}://{host}{pathBase}";
            }
            else
            {
                options.Authority = _authorization.Authority;
            }
        }
    }
}
