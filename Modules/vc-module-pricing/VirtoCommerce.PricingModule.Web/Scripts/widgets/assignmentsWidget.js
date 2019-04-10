angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.assignmentsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.pricingModule.pricelistAssignments', function ($scope, bladeNavigationService, assignments) {
	var blade = $scope.widget.blade;
	function refresh(pricelist) {
		if (pricelist) {
			$scope.widget.assignmentsCount = pricelist.assignments ? pricelist.assignments.length : 0;
			if (pricelist.id) {
				return assignments.search({
					priceListId: pricelist.id,
					take: 0
				}, function (data) {
					$scope.widget.assignmentsCount = data.totalCount;
				});
			}
		}
	}

    $scope.openBlade = function () {
        var newBlade = {
            id: "pricelistChild",
            pricelistId: blade.currentEntity.id,
            currentEntity: blade.currentEntity,
            title: blade.title,
            subtitle: 'pricing.blades.pricelist-assignment-list.subtitle',
            controller: 'virtoCommerce.pricingModule.assignmentListController',
            template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/assignment-list.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.$watch("widget.blade.currentEntity", function (pricelist) {
    	refresh(pricelist);
    });

}]);
