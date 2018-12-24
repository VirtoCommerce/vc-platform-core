using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web.Swagger
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "VirtoCommerce Solution REST API documentation",
                    Version = "v1",
                    TermsOfService = "",
                    Description = "For this sample, you can use the",
                    Contact = new Contact
                    {
                        Email = "support@virtocommerce.com",
                        Name = "Virto Commerce",
                        Url = "http://virtocommerce.com"
                    },
                    License = new License
                    {
                        Name = "Virto Commerce Open Software License 3.0",
                        Url = "http://virtocommerce.com/opensourcelicense"
                    }
                });

                c.TagActionsBy(api => api.GroupByModuleName(services));
                c.DocInclusionPredicate((docName, api) => true);
                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteProperties();
                c.IgnoreObsoleteActions();
                c.OperationFilter<FileResponseTypeFilter>();
                c.OperationFilter<OptionalParametersFilter>();
                c.OperationFilter<TagsFilter>();
                c.DocumentFilter<TagsFilter>();
                c.MapType<object>(() => new Schema
                {
                    Type = "object"
                });
                c.AddModulesXmlComments(services);
                c.CustomSchemaIds(x => x.FullName);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseSwagger(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSwagger(c => c.RouteTemplate = "docs/{documentName}/docs.json");

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            applicationBuilder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/docs/v1/docs.json", "Explore");
                c.RoutePrefix = "docs";
                c.EnableValidator();
                c.IndexStream = () =>
                {
                    var type = typeof(Startup).GetTypeInfo().Assembly
                        .GetManifestResourceStream("VirtoCommerce.Platform.Web.wwwroot.swagger.index.html");
                    return type;
                };
                c.DocumentTitle = "VirtoCommerce Solution REST API documentation";
                c.InjectStylesheet("/swagger/vc.css");
                c.ShowExtensions();
                c.DocExpansion(DocExpansion.None);
                c.DefaultModelsExpandDepth(-1);
            });
        }


        /// <summary>
        /// grouping by Module Names in the ApiDescription
        /// with comparing Assemlies
        /// </summary>
        /// <param name="api"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IList<string> GroupByModuleName(this ApiDescription api, IServiceCollection services)
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
            var groupName = moduleCatalog.Modules.FirstOrDefault(m => m.ModuleInstance != null && m.Assembly == moduleAssembly);

            return new List<string> { groupName != null ? groupName.ModuleName : "Platform" };
        }

        /// <summary>
        /// Add Comments/Descriptions from XML-files in the ApiDescription
        /// </summary>
        /// <param name="options"></param>
        /// <param name="services"></param>
        private static void AddModulesXmlComments(this SwaggerGenOptions options, IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var localStorageModuleCatalogOptions = provider.GetService<IOptions<LocalStorageModuleCatalogOptions>>().Value;

            var xmlCommentsDirectoryPaths = new[]
            {
                localStorageModuleCatalogOptions.ProbingPath,
                AppContext.BaseDirectory
            };

            foreach (var path in xmlCommentsDirectoryPaths)
            {
                var xmlComments = Directory.GetFiles(path, "*.XML");
                foreach (var xmlComment in xmlComments)
                {
                    options.IncludeXmlComments(xmlComment);
                }
            }
        }
    }
}
