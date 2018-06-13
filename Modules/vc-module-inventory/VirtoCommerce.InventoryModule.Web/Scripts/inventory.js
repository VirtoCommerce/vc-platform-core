//Call this to register our module to main application
var moduleName = "virtoCommerce.inventoryModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .run(
    ['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', 'platformWebApp.authService', 'platformWebApp.metaFormsService', function ($rootScope, mainMenuService, widgetService, authService, metaFormsService) {

        //Register widgets in catalog item details
        widgetService.registerWidget({
            isVisible: function (blade) { return blade.productType !== 'Digital' && authService.checkPermission('inventory:update'); },
            controller: 'virtoCommerce.inventoryModule.inventoryWidgetController',
            template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/widgets/inventoryWidget.tpl.html'
        }, 'itemDetail');

        widgetService.registerWidget({    
            size: [2, 1],
            controller: 'virtoCommerce.inventoryModule.fulfillmentAddressesWidgetController',
            template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/widgets/fulfillmentAddressesWidget.tpl.html'
        }, 'fulfillmentCenterDetail');

        metaFormsService.registerMetaFields('inventoryDetails', []);
    }]);
