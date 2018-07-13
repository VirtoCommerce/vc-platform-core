angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentTotalsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
	$scope.blade = $scope.widget.blade;

	$scope.$watch('widget.blade.currentEntity', function (payment) {
	    if (payment) {
		    $scope.payment = payment;
		}
	}, true);
}]);
