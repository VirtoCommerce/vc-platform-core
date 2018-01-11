using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Web.Swagger
{
    public class TagsFilter : IDocumentFilter, IOperationFilter
    {
        private readonly IModuleCatalog _moduleCatalog;
        private readonly ISettingsManager _settingManager;

        public TagsFilter(IModuleCatalog moduleCatalog, ISettingsManager settingManager)
        {
            _moduleCatalog = moduleCatalog;
            _settingManager = settingManager;
        }


        [CLSCompliant(false)]
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var defaultApiKey = _settingManager.GetValue("Swashbuckle.DefaultApiKey", string.Empty);
            swaggerDoc.Info.Description = string.Format(CultureInfo.InvariantCulture, "For this sample, you can use the `{0}` key to satisfy the authorization filters.", defaultApiKey);
            swaggerDoc.Info.TermsOfService = "";

            swaggerDoc.Info.Contact = new Contact
            {
                Email = "support@virtocommerce.com",
                Name = "Virto Commerce",
                Url = "http://virtocommerce.com"
            };

            swaggerDoc.Info.License = new License
            {
                Name = "Virto Commerce Open Software License 3.0",
                Url = "http://virtocommerce.com/opensourcelicense"
            };

            var tags = _moduleCatalog.Modules
                .OfType<ManifestModuleInfo>()
                .Select(x => new Tag
                {
                    Name = x.Title,
                    Description = x.Description
                })
                .ToList();

            tags.Add(new Tag
            {
                Name = "VirtoCommerce platform",
                Description = "Platform functionality represent common resources and operations"
            });

            swaggerDoc.Tags = tags;
        }

        [CLSCompliant(false)]
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerTypeInfo = ((ControllerActionDescriptor)context.ApiDescription.ActionDescriptor).ControllerTypeInfo;
            var module = _moduleCatalog.Modules
                .OfType<ManifestModuleInfo>()
                .Where(x => x.ModuleInstance != null)
                .FirstOrDefault(x => (controllerTypeInfo.Assembly == x.ModuleInstance.GetType().Assembly));

            if (module != null)
            {
                operation.Tags = new[] { module.Title };
            }
            else if (controllerTypeInfo.Assembly.GetName().Name == "VirtoCommerce.Platform.Web")
            {
                operation.Tags = new[] { "VirtoCommerce platform" };
            }
        }
    }
}
