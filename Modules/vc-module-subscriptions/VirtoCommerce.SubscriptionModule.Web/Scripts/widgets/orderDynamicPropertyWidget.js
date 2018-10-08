angular.module('virtoCommerce.subscriptionModule')
.controller('virtoCommerce.subscriptionModule.orderDynamicPropertyWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;

    $scope.openBlade = function () {
        var newBlade = {
            id: "dynamicPropertiesList",
            currentEntity: blade.customerOrder,
            controller: 'platformWebApp.propertyValueListController',
            template: '$(Platform)/Scripts/app/dynamicProperties/blades/propertyValue-list.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, blade);
    };
    
    $scope.$watch('widget.blade.customerOrder', function (entity) {
        if (angular.isDefined(entity)) {
            var groupedByProperty = _.groupBy(entity.dynamicProperties, function (x) { return x.id; });
            $scope.dynamicPropertyCount = _.keys(groupedByProperty).length;
        }
    });
}]);
