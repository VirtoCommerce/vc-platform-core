angular.module('virtoCommerce.taxModule').controller('virtoCommerce.taxModule.taxProviderListController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.taxModule.taxProviders', function ($scope, bladeNavigationService, taxProviders) {
    var blade = $scope.blade;

    blade.refresh = function() {
        blade.isLoading = true;
        taxProviders.search({ storeId: blade.storeId },
            function(data) {
                blade.isLoading = false;
                blade.currentEntities = data.results;
                blade.selectedTaxProvider = _.findWhere(blade.currentEntities, { isActive: true });
            },
            function(error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
            });
    };

    $scope.selectNode = function (node) {
        $scope.selectedNodeId = node.typeName;

        var newBlade = {
            id: 'taxProviderDetail',
            taxProvider: node,
            storeId: blade.storeId,
            title: blade.title,
            subtitle: 'tax.blades.tax-provider-detail.subtitle',
            controller: 'virtoCommerce.taxModule.taxProviderDetailController',
            template: 'Modules/$(VirtoCommerce.Tax)/Scripts/blades/taxProvider-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    };

    blade.toolbarCommands = [
        {
            name: "platform.commands.refresh", icon: 'fa fa-refresh',
            executeMethod: blade.refresh,
            canExecuteMethod: function () {
                return true;
            }
        }
    ];

    blade.headIcon = 'fa-archive';
    blade.refresh();

}]);
