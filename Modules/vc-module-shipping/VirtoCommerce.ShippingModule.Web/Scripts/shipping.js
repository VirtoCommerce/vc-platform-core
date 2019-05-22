//Call this to register our module to main application
var moduleName = "virtoCommerce.shippingModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['ngSanitize'])
    .run(['platformWebApp.widgetService', function (widgetService) {

        widgetService.registerWidget({
            controller: 'virtoCommerce.shippingModule.storeShippingWidgetController',
            template: 'Modules/$(VirtoCommerce.Shipping)/Scripts/widgets/storeShippingWidget.tpl.html'
        }, 'storeDetail');

        widgetService.registerWidget({
            controller: 'platformWebApp.entitySettingsWidgetController',
            template: '$(Platform)/Scripts/app/settings/widgets/entitySettingsWidget.tpl.html'
        }, 'shippingMethodDetail');

    }]);
