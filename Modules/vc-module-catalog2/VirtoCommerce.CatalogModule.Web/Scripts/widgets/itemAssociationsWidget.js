angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemAssociationsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.blade;

    $scope.openBlade = function () {
    	if (blade.item.associations) {
    		var newBlade = {
    			id: "associationsList",
    			item : blade.item,              
    			controller: 'virtoCommerce.catalogModule.itemAssociationsListController',
    			template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-associations-list.tpl.html'
    		};
    	}
    	bladeNavigationService.showBlade(newBlade, blade);
    };    
}]);
