angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.shipmentTotalsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
	$scope.blade = $scope.widget.blade;

	$scope.$watch('widget.blade.currentEntity', function (shipment) {
		if (shipment) {
			$scope.shipment = shipment;
		}
	}, true);
}]);
