//Call this to register our module to main application
var moduleName = "virtoCommerce.paymentModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, []).run(['platformWebApp.widgetService',
    function (widgetService) {

        //Register payment widget for store
        widgetService.registerWidget({
            controller: 'virtoCommerce.paymentModule.storePaymentsWidgetController',
            template: 'Modules/$(VirtoCommerce.Payment)/Scripts/widgets/storePaymentsWidget.tpl.html'
        }, 'storeDetail');

        widgetService.registerWidget({
            controller: 'platformWebApp.entitySettingsWidgetController',
            template: '$(Platform)/Scripts/app/settings/widgets/entitySettingsWidget.tpl.html'
        }, 'paymentMethodDetail');

    }]);
