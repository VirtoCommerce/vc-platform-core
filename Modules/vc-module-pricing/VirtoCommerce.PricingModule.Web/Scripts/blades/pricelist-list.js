angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.pricelistListController', ['$scope', 'virtoCommerce.pricingModule.pricelists', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
function ($scope, pricelists, dialogService, uiGridHelper, bladeUtils) {
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;

    var exportDataRequest = {
        exportTypeName: 'Pricelist',
        dataQuery: {
            exportTypeName: 'PricelistExportDataQuery'
        }
    };

    blade.refresh = function (parentRefresh) {
        blade.isLoading = true;
        return pricelists.search(getSearchCriteria(), function (data) {
            blade.isLoading = false;
            blade.currentEntities = data.results;
            $scope.pageSettings.totalItems = data.totalCount;

            if (parentRefresh === true && blade.parentRefresh) {
                blade.parentRefresh();
            }

            return data;
        }).$promise;
    };

    $scope.selectNode = function (node, isNew) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            id: 'listItemChild',
            controller: 'virtoCommerce.pricingModule.pricelistDetailController',
            template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/pricelist-detail.tpl.html'
        };

        if (isNew) {
            angular.extend(newBlade, {
                title: 'pricing.blades.pricelist-detail.title-new',
                isNew: true,
                saveCallback: function (newPricelist) {
                    newBlade.isNew = false;
                    blade.refresh(true).then(function () {
                        newBlade.currentEntityId = newPricelist.id;
                        bladeNavigationService.showBlade(newBlade, blade);
                    });
                }
                // onChangesConfirmedFn: callback,
            });
        } else {
            angular.extend(newBlade, {
                currentEntityId: node.id,
                title: node.name,
                subtitle: blade.subtitle
            });
        }

        bladeNavigationService.showBlade(newBlade, blade);
    };

    function isItemsChecked() {
        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
    }

    $scope.deleteList = function (list) {
        var dialog = {
            id: "confirmDeleteItem",
            title: "pricing.dialogs.pricelists-delete.title",
            message: "pricing.dialogs.pricelists-delete.message",
            callback: function (remove) {
                if (remove) {
                    bladeNavigationService.closeChildrenBlades(blade, function () {
                        pricelists.remove({ ids: _.pluck(list, 'id') },
                            function() {
                                return blade.refresh(true);
                            });
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    blade.headIcon = 'fa-usd';
    blade.subtitle = 'pricing.blades.pricelist-list.subtitle';

    blade.toolbarCommands = [
    {
        name: "platform.commands.refresh", icon: 'fa fa-refresh',
        executeMethod: blade.refresh,
        canExecuteMethod: function () { return true; }
    },
    {
        name: "platform.commands.add", icon: 'fa fa-plus',
        executeMethod: function () {
            $scope.selectNode({}, true);
        },
        canExecuteMethod: function () {
            return true;
        },
        permission: 'pricing:create'
    },
    {
        name: "platform.commands.delete", icon: 'fa fa-trash-o',
        executeMethod: function () {
            $scope.deleteList($scope.gridApi.selection.getSelectedRows());
        },
        canExecuteMethod: isItemsChecked,
        permission: 'pricing:delete'
        },
        {
            name: "platform.commands.export",
            icon: 'fa fa-upload',
            canExecuteMethod: function () {
                return true;
            },
            executeMethod: function () {

                var selectedRows = $scope.gridApi.selection.getSelectedRows();
                exportDataRequest.dataQuery.objectIds = [];
                if (selectedRows && selectedRows.length) {
                    exportDataRequest.dataQuery.objectIds = _.map(selectedRows, function (pricelist) {
                        return pricelist.id;
                    });
                }

                exportDataRequest.dataQuery = angular.extend(exportDataRequest.dataQuery, getSearchCriteria());

                var newBlade = {
                    id: 'pricelistExport',
                    title: 'pricing.blades.exporter.priceListTitle',
                    subtitle: 'pricing.blades.exporter.pricelistSubtitle',
                    controller: 'virtoCommerce.exportModule.exportSettingsController',
                    template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/exportSettings.tpl.html',
                    exportDataRequest: exportDataRequest
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }
        }
    ];

    var filter = $scope.filter = {};
    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;

        gridOptions.onRegisterApi = function (gridApi) {
            gridApi.core.on.sortChanged($scope, function () {
                if (!blade.isLoading) blade.refresh();
            });
        };

        bladeUtils.initializePagination($scope);
    };

    function getSearchCriteria() {
        var result = {
            keyword: filter.keyword,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        }
        return result;
    }

    // actions on load
    //blade.refresh();
}]);
