using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VirtoCommerce.Platform.Web.Swagger
{
    public class AssignOAuth2SecurityOperationFilter : IOperationFilter
    {
        [CLSCompliant(false)]
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.TryGetMethodInfo(out var methodInfo))
            {
                var allowAnonymousAttributes = methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute));
                if (allowAnonymousAttributes.Any())
                {
                    operation.Security = null;
                    return;
                }

                var permissionAttributes = methodInfo.GetCustomAttributes<AuthorizeAttribute>();

                var permissions = permissionAttributes.Select(x => x.Policy).Distinct().ToList();
                if (!permissions.Any())
                {
                    return;
                }

                var securityRequirements = new Dictionary<string, IEnumerable<string>>
                {
                    {"OAuth2", permissions}
                };

                if (operation.Security == null)
                {
                    operation.Security = new List<IDictionary<string, IEnumerable<string>>>(1);
                }

                operation.Security.Add(securityRequirements);
            }

        }
    }
}
