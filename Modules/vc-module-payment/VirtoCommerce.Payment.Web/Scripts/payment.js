//Call this to register our module to main application
var moduleName = "virtoCommerce.paymentModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [
    'ngSanitize'
])
.run(
  ['platformWebApp.toolbarService', 'platformWebApp.bladeNavigationService', 'platformWebApp.widgetService',
    function (toolbarService, bladeNavigationService, widgetService) {

        //Register payment widget for store
        widgetService.registerWidget({
            controller: 'virtoCommerce.paymentModule.storePaymentsWidgetController',
            template: 'Modules/$(VirtoCommerce.Payment)/Scripts/widgets/storePaymentsWidget.tpl.html'
        }, 'storeDetail');

    }]);
