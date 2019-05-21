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

    .factory('virtoCommerce.taxModule.taxUtils', ['virtoCommerce.taxModule.taxProviders', 'platformWebApp.bladeNavigationService', function (taxProviders, bladeNavigationService) {
        var taxProvidersRef;
        return {
            getTaxProviders: function (storeId) {
                taxProvidersRef = taxProviders.search({ storeId });
                return taxProvidersRef;
            },
            editTaxProviders: function (blade) {
                var newBlade = {
                    id: 'taxProviders',
                    parentRefresh: function (data) { angular.copy(data, taxProvidersRef); },
                    controller: 'virtoCommerce.taxModule.taxProviderListController',
                    template: 'Modules/$(VirtoCommerce.Tax)/Scripts/blades/taxProvider-list.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }
        };
    }])
