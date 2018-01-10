using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web.Extensions
{
    public static class SwaggerExtensions
    {
        public static string GroupByModuleName(this ApiDescription api, IServiceCollection services)
        {
            //var groupNameAttribute = (SwaggerGroupAttribute)api.ControllerAttributes().SingleOrDefault(attribute => attribute is SwaggerGroupAttribute);
            var groups = api.Properties;
            var providerSnapshot = services.BuildServiceProvider();

            var moduleCatalog = providerSnapshot.GetRequiredService<ILocalModuleCatalog>();
            
            // ------
            // Lifted from ApiDescriptionExtensions
            var actionDescriptor = api.GetProperty<ControllerActionDescriptor>();

            if (actionDescriptor == null)
            {
                actionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                api.SetProperty(actionDescriptor);
            }
            // ------

            var moduleName = actionDescriptor.ControllerTypeInfo.Assembly.GetName().Name.Replace(".Web","");
            var groupName = moduleCatalog.Modules.FirstOrDefault(m => m.ModuleName == moduleName);

            return groupName != null ? groupName.ModuleName : actionDescriptor?.ControllerName;
        }

        public static void AddModulesXmlComments(this SwaggerGenOptions options, string[] xmlCommentsDirectoryPaths)
        {
            foreach (var path in xmlCommentsDirectoryPaths)
            {
                var xmlComments = Directory.GetFiles(path, "*.Web.XML");
                foreach (var xmlComment in xmlComments)
                {
                    options.IncludeXmlComments(xmlComment);
                }
            }
        }
    }
}
