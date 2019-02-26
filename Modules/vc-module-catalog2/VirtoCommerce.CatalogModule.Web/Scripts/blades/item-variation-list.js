angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemVariationListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.catalogModule.items', 'filterFilter', 'uiGridConstants', 'platformWebApp.uiGridHelper', function ($scope, bladeNavigationService, dialogService, items, filterFilter, uiGridConstants, uiGridHelper) {
    $scope.uiGridConstants = uiGridConstants;
    var blade = $scope.blade;

    //pagination settings
    $scope.pageSettings = {};
    $scope.pageSettings.totalItems = 0;
    $scope.pageSettings.currentPage = 1;
    $scope.pageSettings.numPages = 5;
    $scope.pageSettings.itemsPerPageCount = 20;

    blade.isLoading = false;

    blade.refresh = function (item) {
    	if (item) {
    		initialize(item);
    	}
    	else {
    		blade.parentBlade.refresh();
    	}

    };

    function initialize(item) {
    	blade.title = item.name;
    	blade.subtitle = 'catalog.widgets.itemVariation.blade-subtitle';
    	blade.item = item;
    	$scope.pageSettings.totalItems = blade.item.variations.length;
    }

    blade.setSelectedItem = function (listItem) {
        $scope.selectedNodeId = listItem.id;
    };

    $scope.selectVariation = function (listItem) {
        blade.setSelectedItem(listItem);
        var newBlade = {
            id: 'variationDetail',
            itemId: listItem.id,
            productType: listItem.productType,
            title: listItem.code,
            catalog: blade.catalog,
            subtitle: 'catalog.blades.item-detail.subtitle-variation',
            controller: 'virtoCommerce.catalogModule.itemDetailController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.deleteList = function (list) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var dialog = {
                id: "confirmDeleteItem",
                title: "catalog.dialogs.variation-delete.title",
                message: "catalog.dialogs.variation-delete.message",
                callback: function (remove) {
                    if (remove) {
                        var ids = _.pluck(list, 'id');
                        items.remove({ ids: ids }, blade.parentBlade.refresh, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    }
                }
            }
            dialogService.showConfirmationDialog(dialog);
        });
    };


    blade.headIcon = 'fa-dropbox';

    if (blade.toolbarCommandsAndEvents && blade.toolbarCommandsAndEvents.toolbarCommands) {
        blade.toolbarCommands = blade.toolbarCommandsAndEvents.toolbarCommands;
    } else
        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: function () {
                	blade.parentBlade.refresh()
                },
                canExecuteMethod: function () { return true; }
            },
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () {
                    items.newVariation({ itemId: blade.item.id }, function (data) {
                        // take variation properties only
                        data.properties = _.where(data.properties, { type: 'Variation' });
                        data.productType = blade.item.productType;

                        var newBlade = {
                            id: 'variationDetail',
                            item: data,
                            catalog: blade.catalog,
                            title: "catalog.wizards.new-variation.title",
                            subtitle: 'catalog.wizards.new-variation.subtitle',
                            controller: 'virtoCommerce.catalogModule.newProductWizardController',
                            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/wizards/newProduct/new-variation-wizard.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                },
                canExecuteMethod: function () { return true; },
                permission: 'catalog:create'
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () {
                    $scope.deleteList($scope.gridApi.selection.getSelectedRows());
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
                permission: 'catalog:delete'
            }
        ];

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions,
        function (gridApi) {
            gridApi.grid.registerRowsProcessor($scope.singleFilter, 90);
            $scope.$watch('pageSettings.currentPage', gridApi.pagination.seek);

            if (blade.toolbarCommandsAndEvents && blade.toolbarCommandsAndEvents.externalRegisterApiCallback) {
                blade.toolbarCommandsAndEvents.externalRegisterApiCallback(gridApi);
            }
        });
    };

    $scope.singleFilter = function (renderableRows) {
        var visibleCount = 0;
        renderableRows.forEach(function (row) {
            row.visible = _.any(filterFilter([row.entity], blade.searchText));
            if (row.visible) visibleCount++;
        });

        $scope.filteredEntitiesCount = visibleCount;
        return renderableRows;
    };

    //// actions on load
    //$scope.$watch('blade.parentBlade.item.variations', initializeBlade);

    initialize(blade.item);
}]);
