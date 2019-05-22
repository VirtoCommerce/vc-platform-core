//Call this to register our module to main application
var moduleName = "virtoCommerce.taxModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['ngSanitize'])
    .run(
        ['platformWebApp.toolbarService', 'platformWebApp.bladeNavigationService', 'platformWebApp.widgetService',
            function (toolbarService, bladeNavigationService, widgetService) {
                //Register tax widget for store
                widgetService.registerWidget({
                    controller: 'virtoCommerce.taxModule.storeTaxingWidgetController',
                    template: 'Modules/$(VirtoCommerce.Tax)/Scripts/widgets/storeTaxingWidget.tpl.html'
                }, 'storeDetail');

                widgetService.registerWidget({
                    controller: 'platformWebApp.entitySettingsWidgetController',
                    template: '$(Platform)/Scripts/app/settings/widgets/entitySettingsWidget.tpl.html'
                }, 'taxProviderDetail');
            }])
