angular.module('virtoCommerce.pricingModule')
    .controller('virtoCommerce.pricingModule.assignmentListController', ['$scope', 'virtoCommerce.pricingModule.pricelistAssignments', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'virtoCommerce.catalogModule.catalogs', '$localStorage',
        function ($scope, assignments, dialogService, uiGridHelper, bladeUtils, catalogs, $localStorage) {
            $scope.uiGridConstants = uiGridHelper.uiGridConstants;
            var blade = $scope.blade;
            var bladeNavigationService = bladeUtils.bladeNavigationService;
            var defaultDataRequest = {
                exportTypeName: 'VirtoCommerce.PricingModule.Data.ExportImport.ExportablePricelistAssignment',
                isTabularExportSupported: true,
                dataQuery: {
                    exportTypeName: 'PricelistAssignmentExportDataQuery'
                }
            };
            var exportDataRequest = angular.copy(defaultDataRequest);
            var filter = blade.filter = $scope.filter = {};

            blade.refresh = function () {
                
                blade.isLoading = true;
                assignments.search(getSearchCriteria(), function (data) {
                    //Loading catalogs for assignments because they do not contains them
                    //Need to display name of catalog in assignments grid
                    catalogs.getCatalogs(function (results) {
                        blade.isLoading = false;
                        $scope.pageSettings.totalItems = data.totalCount;

                        var priceAssignments = data.results;
                        _.each(priceAssignments, function (x) {
                            var catalog = _.findWhere(results, { id: x.catalogId });
                            if (catalog) {
                                x.catalog = catalog.name;
                            }
                        });

                        blade.currentEntities = priceAssignments;
                    });
                });
            };

            $scope.selectNode = function (node, isNew) {
                $scope.selectedNodeId = node.id;

                var newBlade = {
                    id: 'pricelistAssignmentDetail',
                    controller: 'virtoCommerce.pricingModule.assignmentDetailController',
                    template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/assignment-detail.tpl.html'
                };

                if (isNew) {
                    angular.extend(newBlade, {
                        isNew: true,
                        pricelistId: blade.pricelistId,
                        data: node,
                        title: 'pricing.blades.assignment-detail.new-title'
                    });
                } else {
                    angular.extend(newBlade, {
                        currentEntityId: node.id,
                        title: node.name,
                        subtitle: 'pricing.blades.assignment-detail.subtitle'
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
                    title: "pricing.dialogs.assignments-delete.title",
                    message: "pricing.dialogs.assignments-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            closeChildrenBlades();

                            var itemIds = _.pluck(list, 'id');
                            assignments.remove({ ids: itemIds }, function () {
                                blade.refresh();
                            }, function (error) {
                                bladeNavigationService.setError('Error ' + error.status, blade);
                            });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            };

            $scope.deleteAllFiltered = function () {
                var dialog = {
                    id: "confirmDeleteItems",
                    callback: function (confirm) {
                        if (!confirm)
                            return;
                        closeChildrenBlades();
                        blade.isLoading = true;
                        assignments.removeFiltered({
                            pricelistId: blade.pricelistId,
                            keyword: filter.keyword
                        }, function () {
                            blade.refresh();
                        });
                    }
                };
                dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.Pricing)/Scripts/dialogs/deleteAll-dialog.tpl.html', 'platformWebApp.confirmDialogController');
            };

            function closeChildrenBlades() {
                angular.forEach(blade.childrenBlades.slice(), function (child) {
                    bladeNavigationService.closeBlade(child);
                });
            }

            blade.headIcon = 'fa-anchor';
            blade.subtitle = 'pricing.blades.pricelist-assignment-list.subtitle';

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
                    canExecuteMethod: function () { return true; },
                    permission: 'pricing:create'
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: function () {
                        $scope.deleteList($scope.gridApi.selection.getSelectedRows());
                    },
                    canExecuteMethod: function () {
                        return isItemsChecked();
                    },
                    permission: 'pricing:delete'
                },
                {
                    name: "pricing.commands.delete-all-filtered",
                    icon: 'fa fa-trash-o',
                    executeMethod: function () {
                        $scope.deleteAllFiltered();
                    },
                    canExecuteMethod: function () {
                        return blade.currentEntities && blade.currentEntities.length > 0;
                    },
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
                            exportDataRequest.dataQuery.objectIds = _.map(selectedRows, function (priceAssignments) {
                                return priceAssignments.id;
                            });
                        }

                        var searchCriteria = getSearchCriteria();
                        if ((searchCriteria.pricelistIds && searchCriteria.pricelistIds.length > 0) || searchCriteria.keyword !== '') {
                            exportDataRequest.dataQuery.isAnyFilterApplied = true;
                        }

                        exportDataRequest.dataQuery = angular.extend(exportDataRequest.dataQuery, searchCriteria);

                        var newBlade = {
                            id: 'priceAssignmentExport',
                            title: 'pricing.blades.exporter.priceAssignmentTitle',
                            subtitle: 'pricing.blades.exporter.priceAssignmentSubtitle',
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
                        blade.refresh();
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
                };
                if (filter.current) {
                    result.pricelistIds = filter.current.priceListIds;
                    result.catalogIds = filter.current.catalogIds || [];
                } else {
                    result.pricelistIds = blade.pricelistId ? [blade.pricelistId] : [];
                }

                return result;
            }
            if (!$localStorage.exportSearchFilters) {
                $localStorage.exportSearchFilters = [];
            }

            if (!$localStorage.exportSearchFilters[exportDataRequest.exportTypeName]) {
                $localStorage.exportSearchFilters[exportDataRequest.exportTypeName] = [{ name: 'export.blades.export-generic-viewer.labels.new-filter' }];
            }

            $scope.exportSearchFilters = $localStorage.exportSearchFilters[exportDataRequest.exportTypeName];

            if (!$localStorage.exportSearchFilterIds) {
                $localStorage.exportSearchFilterIds = [];
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
            // actions on load
            //blade.refresh();
        }]);
