angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.editorialReviewsListController', ['$timeout', '$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService', function ($timeout, $scope, bladeNavigationService, uiGridHelper, dialogService) {
    var blade = $scope.blade;

    $scope.selectedNodeId = null; // need to initialize to null
    blade.isLoading = false;
    blade.refresh = function (item) {
    	initialize(item);    	
    };

    function initialize(item) {
    	blade.headIcon = 'fa-comments';
    	blade.item = item;
    	blade.title = blade.item.name;
    	blade.subtitle = 'catalog.blades.editorialReviews-list.subtitle';
    	blade.selectNode = $scope.openBlade;
    };

    $scope.openBlade = function (node) {
    	if (node) {
    		$scope.selectedNodeId = node.id;
    	}
        var newBlade = {
            id: 'editorialReview',
            currentEntity: node,
            item: blade.item,
            catalog: blade.catalog,
            languages: blade.catalog.languages,
            controller: 'virtoCommerce.catalogModule.editorialReviewDetailController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/editorialReview-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    }        
   

    $scope.delete = function (data) {
    	deleteList([data]);
    };

    function deleteList(selection) {
    	var dialog = {
    		id: "confirmDelete",
    		title: "catalog.dialogs.review-delete.title",
    		message: "catalog.dialogs.review-delete.message",
    		callback: function (remove) {
    			if (remove) {
    				bladeNavigationService.closeChildrenBlades(blade, function () {
    					_.each(selection, function (x) {
    						blade.item.reviews.splice(blade.item.reviews.indexOf(x), 1);
    					});
    				});
    			}
    		}
    	};
    	dialogService.showConfirmationDialog(dialog);
    }

    blade.toolbarCommands = [
        {
        	name: "platform.commands.add", icon: 'fa fa-plus',
        	executeMethod: function () {
        		$scope.openBlade({});
        	},
        	canExecuteMethod: function () {
        		return true;
        	}
        },
		{
			name: "platform.commands.delete", icon: 'fa fa-trash-o',
			executeMethod: function () { deleteList($scope.gridApi.selection.getSelectedRows()); },
			canExecuteMethod: function () {
				return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
			}
		}

    ];

	// ui-grid
    $scope.setGridOptions = function (gridOptions) {
    	uiGridHelper.initialize($scope, gridOptions);
    };

    // open blade for new review 
    if (!_.some(blade.item.reviews)) {
    	$timeout($scope.openBlade, 60, false);
    }

    initialize(blade.item);
}]);
