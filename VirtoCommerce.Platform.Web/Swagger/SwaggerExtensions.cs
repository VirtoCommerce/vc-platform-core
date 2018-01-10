using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web.Swagger
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// grouping by Module Names in the ApiDescription
        /// with comparing Assemlies
        /// </summary>
        /// <param name="api"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static string GroupByModuleName(this ApiDescription api, IServiceCollection services)
        {
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

            var moduleAssembly = actionDescriptor?.ControllerTypeInfo.Assembly ?? Assembly.GetExecutingAssembly();
            var groupName = moduleCatalog.Modules.FirstOrDefault(m => m.Assembly == moduleAssembly);

            return groupName != null ? groupName.ModuleName : "Platform";
        }

        /// <summary>
        /// Add Comments/Descriptions from XML-files in the ApiDescription
        /// </summary>
        /// <param name="options"></param>
        /// <param name="xmlCommentsDirectoryPaths"></param>
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
