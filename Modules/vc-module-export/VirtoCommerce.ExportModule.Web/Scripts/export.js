//Call this to register our module to main application
var moduleName = "virtoCommerce.exportModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['ngSanitize'])
    .run(
        [
            'platformWebApp.toolbarService', 'platformWebApp.bladeNavigationService', 'platformWebApp.widgetService',
            function(toolbarService, bladeNavigationService, widgetService) {
            }
        ]);
