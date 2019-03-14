var moduleName = 'virtoCommerce.sitemapsModule';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.run(['platformWebApp.widgetService', function (widgetService) {
    widgetService.registerWidget({
        controller: 'virtoCommerce.sitemapsModule.storeSitemapsWidgetController',
        template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/widgets/store-sitemaps-widget.tpl.html'
    }, 'storeDetail');
}]);