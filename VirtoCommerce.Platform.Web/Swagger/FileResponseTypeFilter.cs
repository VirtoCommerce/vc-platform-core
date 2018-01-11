using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Web.Swagger
{
    public class FileResponseTypeFilter : IOperationFilter
    {
        [CLSCompliant(false)]
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (IsFileResponse(context.ApiDescription))
            {
                Schema responseSchema = new Schema { Format = "byte", Type = "file" };

                operation.Responses.Add(((int)HttpStatusCode.OK).ToString(), new Response
                {
                    Description = "OK",
                    Schema = responseSchema
                });
            }
        }

        private static bool IsFileResponse(ApiDescription apiDescription)
        {
            var result = apiDescription.ActionAttributes().OfType<SwaggerFileResponseAttribute>().Any();
            if (!result)
            {
                result = apiDescription.SupportedResponseTypes.Any(r => r.Type == typeof(Stream));
            }
            return result;
        }
    }
}
