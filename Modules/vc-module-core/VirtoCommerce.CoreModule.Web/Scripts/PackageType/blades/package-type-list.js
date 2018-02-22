angular.module('virtoCommerce.coreModule.packageType')
.controller('virtoCommerce.coreModule.packageType.packageTypeListController', ['$scope', 'virtoCommerce.coreModule.packageType.packageTypeApi', 'platformWebApp.bladeNavigationService',
function ($scope, packageTypeApi, bladeNavigationService) {
	var blade = $scope.blade;

	blade.refresh = function (parentRefresh) {
		blade.isLoading = true;

		packageTypeApi.query({}, function (results) {
			blade.isLoading = false;
			blade.currentEntities = results;

			if (parentRefresh && blade.parentRefresh) {
				blade.parentRefresh(results);
			}
		}, function (error) {
			bladeNavigationService.setError('Error ' + error.status, blade);
		});
	};

	blade.setSelectedId = function (selectedNodeId) {
		$scope.selectedNodeId = selectedNodeId;
	};

	function showDetailBlade(bladeData) {
		var newBlade = {
			id: 'packageTypeDetail',
			controller: 'virtoCommerce.coreModule.packageType.packageTypeDetailController',
			template: 'Modules/$(VirtoCommerce.Core)/Scripts/PackageType/blades/package-type-detail.tpl.html'
		};
		angular.extend(newBlade, bladeData);
		bladeNavigationService.showBlade(newBlade, blade);
	};

	$scope.selectNode = function (node) {
		blade.setSelectedId(node.code);
		showDetailBlade({ data: node });
	};

	blade.headIcon = 'fa-square';
	blade.toolbarCommands = [
      {
      	name: "platform.commands.refresh", icon: 'fa fa-refresh',
      	executeMethod: blade.refresh,
      	canExecuteMethod: function () {
      		return true;
      	}
      },
        {
        	name: "platform.commands.add", icon: 'fa fa-plus',
        	executeMethod: function () {
        		blade.setSelectedId(null);
        		showDetailBlade({ isNew: true });
        	},
        	canExecuteMethod: function () {
        		return true;
        	},
        	permission: 'core:packageType:create'
        }
	];

	// actions on load
	blade.title = 'core.blades.package-type-list.title';
	blade.subtitle = 'core.blades.package-type-list.subtitle';
    blade.refresh();
}]);