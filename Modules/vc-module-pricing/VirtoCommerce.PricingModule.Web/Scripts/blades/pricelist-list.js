angular.module('virtoCommerce.pricingModule')
    .controller('virtoCommerce.pricingModule.pricelistListController', ['$scope', 'virtoCommerce.pricingModule.pricelists', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', '$localStorage', 
function ($scope, pricelists, dialogService, uiGridHelper, bladeUtils, $localStorage) {
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;

    var defaultDataRequest = {
        exportTypeName: 'VirtoCommerce.PricingModule.Data.ExportImport.ExportablePricelist',
        isTabularExportSupported: true,
        dataQuery: {
            exportTypeName: 'PricelistExportDataQuery'
        }
    };
    var exportDataRequest = angular.copy(defaultDataRequest);
    var filter = blade.filter = $scope.filter = {};

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
                exportDataRequest.dataQuery.isAllSelected = true;
                var selectedRows = $scope.gridApi.selection.getSelectedRows();
                exportDataRequest.dataQuery.objectIds = [];
                if (selectedRows && selectedRows.length) {
                    exportDataRequest.dataQuery.isAllSelected = false;
                    exportDataRequest.dataQuery.objectIds = _.map(selectedRows, function (pricelist) {
                        return pricelist.id;
                    });
                }

                var searchCriteria = getSearchCriteria();
                if (searchCriteria.keyword !== '') {
                    exportDataRequest.dataQuery.isAnyFilterApplied = true;
                }

                exportDataRequest.dataQuery = angular.extend(exportDataRequest.dataQuery, searchCriteria);

                var newBlade = {
                    id: 'pricelistExport',
                    title: 'pricing.blades.exporter.priceListTitle',
                    subtitle: 'pricing.blades.exporter.pricelistSubtitle',
                    controller: 'virtoCommerce.exportModule.exportSettingsController',
                    template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-settings.tpl.html',
                    exportDataRequest: exportDataRequest,
                    totalItemsCount: exportDataRequest.dataQuery.objectIds.length || $scope.pageSettings.totalItems

                };
                bladeNavigationService.showBlade(newBlade, blade);
            }
        }
    ];

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



    if (!$localStorage.exportSearchFilters) {
        $localStorage.exportSearchFilters = {};
    }

    if (!$localStorage.exportSearchFilters[exportDataRequest.exportTypeName]) {
        $localStorage.exportSearchFilters[exportDataRequest.exportTypeName] = [{ name: 'export.blades.export-generic-viewer.labels.new-filter' }];
    }

    $scope.exportSearchFilters = $localStorage.exportSearchFilters[exportDataRequest.exportTypeName];

    if (!$localStorage.exportSearchFilterIds) {
        $localStorage.exportSearchFilterIds = {};
    }

    $scope.exportSearchFilterId = $localStorage.exportSearchFilterIds[exportDataRequest.exportTypeName];

    if ($scope.exportSearchFilterId) {
        filter.current = _.findWhere($scope.exportSearchFilters, { id: $scope.exportSearchFilterId });
    }

    filter.change = function () {
        $localStorage.exportSearchFilterId = filter.current ? filter.current.id : null;
        var metafieldsId = exportDataRequest.exportTypeName + 'ExportFilter';
        if (filter.current && !filter.current.id) {
            filter.current = null;
            showFilterDetailBlade({ isNew: true, metafieldsId: metafieldsId, exportTypeName: exportDataRequest.exportTypeName });
        } else {
            bladeNavigationService.closeBlade({ id: 'exportGenericViewerFilter' });

            if (!filter.current) {
                blade.resetRequestCustomFilter();
            }

            filter.criteriaChanged();
        }
    };

    filter.edit = function () {
        var metafieldsId = exportDataRequest.exportTypeName + 'ExportFilter';
        var filterDetailsParams = {
            data: filter.current,
            metafieldsId: metafieldsId,
            exportTypeName: exportDataRequest.exportTypeName
        };

        if (filter.current) {
            angular.extend(filterDetailsParams, { data: filter.current });
        }
        else {
            angular.extend(filterDetailsParams, { isNew: true });
        }

        showFilterDetailBlade(filterDetailsParams);
    };

    function showFilterDetailBlade(bladeData) {
        var newBlade = {
            id: 'exportGenericViewerFilter',
            controller: 'virtoCommerce.exportModule.exportGenericViewerFilterController',
            template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-generic-viewer-filter.tpl.html',
            onBeforeApply: blade.resetRequestCustomFilter
        };
        angular.extend(newBlade, bladeData);
        bladeNavigationService.showBlade(newBlade, blade);
    }

    filter.criteriaChanged = function () {
        blade.refresh();
    };

    filter.resetKeyword = function () {
        filter.keyword = undefined;

        if (exportDataRequest.dataQuery) {
            exportDataRequest.dataQuery.keyword = undefined;
        }
    }

    blade.resetRequestCustomFilter = function () {
        angular.copy(exportDataRequest, defaultDataRequest);
    }

    function getSearchCriteria() {
        var result = {
            keyword: filter.keyword,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        };

        if (filter.current) {
            result.currencies = filter.current.currencies;
        } 
        return result;
    }

    // actions on load
    //blade.refresh();
}]);
