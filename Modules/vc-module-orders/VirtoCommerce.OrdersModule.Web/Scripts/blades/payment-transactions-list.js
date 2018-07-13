angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentTransactionsListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', function ($scope, bladeNavigationService, dialogService) {
    var blade = $scope.blade;

    blade.isLoading = false;

	blade.title = 'orders.blades.transactions-list.title';
    blade.subtitle = 'orders.blades.transactions-list.subtitle',
    // ui-grid
	$scope.setGridOptions = function (gridOptions) {
	    // add currency filter for properties that need it
	    Array.prototype.push.apply(gridOptions.columnDefs, _.map(["amount"], function (name) {
	        return { name: name, cellFilter: "currency", visible: false };
	    }));

	    $scope.gridOptions = gridOptions;
	};

	$scope.selectNode = function (transaction) {
	
	    var newBlade = {
	        id: 'transactionDetail',
	        currentEntity: transaction,	     
	        controller: 'virtoCommerce.orderModule.paymentTransactionsDetailController',
	        template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/payment-transactions-detail.tpl.html'
	    };
	    bladeNavigationService.showBlade(newBlade, blade);
	};	
}]);