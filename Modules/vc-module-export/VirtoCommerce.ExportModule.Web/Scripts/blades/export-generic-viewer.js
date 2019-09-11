angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportGenericViewerController',
    ['$localStorage', '$timeout', '$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'virtoCommerce.exportModule.exportModuleApi', 'virtoCommerce.exportModule.genericViewerItemService',
    function ($localStorage, $timeout, $scope, bladeUtils, uiGridHelper, exportModuleApi, genericViewerItemService) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        $scope.hasMore = true;
        $scope.items = [];
        $scope.blade.headIcon = 'fa-upload';
        $scope.exportSearchFilters = [];
        $scope.exportSearchFilterIds = [];

        var blade = $scope.blade;
        var bladeNavigationService = bladeUtils.bladeNavigationService;
        blade.isLoading = true;
        blade.isExpanded = true;

        var filter = blade.filter = $scope.filter = {};
        blade.exportDataRequest = blade.exportDataRequest ? angular.copy(blade.exportDataRequest) : { exportTypeName: "NotSpecified" };

        if (blade.exportDataRequest.dataQuery && blade.exportDataRequest.dataQuery.keyword) {
            filter.keyword = blade.exportDataRequest.dataQuery.keyword;
        }

        $scope.$localStorage = $localStorage;

        blade.refresh = function () {
            $scope.items = [];

            if ($scope.pageSettings.currentPage !== 1) {
                $scope.pageSettings.currentPage = 1;
            }

            loadData();

            resetStateGrid();
        };

        function loadData(callback) {
            blade.isLoading = true;

            angular.extend(blade.exportDataRequest.dataQuery, buildDataQuery());
            var dataQuery = blade.exportDataRequest.dataQuery;
            var pagedDataQuery = {
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            }
            var exportDataRequest = {
                dataQuery: pagedDataQuery,
                exportTypeName: blade.exportDataRequest.exportTypeName
            }
            delete dataQuery.skip;
            delete dataQuery.take;
            angular.extend(pagedDataQuery, dataQuery);

            exportModuleApi.getData(
                exportDataRequest,
                function (data) {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    $scope.items = $scope.items.concat(data.results);
                    $scope.hasMore = data.results.length === $scope.pageSettings.itemsPerPageCount;

                    $timeout(function () {
                        if ($scope.gridApi) {
                            if (pagedDataQuery.objectIds && pagedDataQuery.objectIds.length) {
                                _.each(pagedDataQuery.objectIds, function (objectId) {
                                    var dataItem = _.findWhere($scope.items, { id: objectId });
                                    $scope.gridApi.selection.selectRow(dataItem);
                                });
                            }
                            else if (pagedDataQuery.isAllSelected) {
                                $scope.gridApi.selection.selectAllRows();
                            }
                        }
                    });

                    if (callback) {
                        callback();
                    }
                });
        }

        function showMore() {
            if ($scope.hasMore) {
                ++$scope.pageSettings.currentPage;
                $scope.gridApi.infiniteScroll.saveScrollPercentage();
                loadData(function () {
                    $scope.gridApi.infiniteScroll.dataLoaded();

                    $timeout(function () {
                        // wait for grid to ingest data changes
                        if ($scope.gridApi.selection.getSelectAllState()) {
                            $scope.gridApi.selection.selectAllRows();
                        }
                    });
                });
            }
        }

        blade.resetFiltering = function () {
            filter.resetKeyword();
            blade.resetRequestCustomFilter();
        };

        blade.resetRequestCustomFilter = function () {
            blade.exportDataRequest.dataQuery = getEmptyDataQuery();
        }

        function buildDataQuery() {
            var dataQuery = getEmptyDataQuery();

            angular.extend(dataQuery, getFilterConditions());

            return dataQuery;
        }

        function getEmptyDataQuery() {

            var dataQuery = {
                exportTypeName: blade.exportDataRequest.dataQuery.exportTypeName,
                includedProperties: blade.exportDataRequest.dataQuery.includedProperties,
                sort: uiGridHelper.getSortExpression($scope)
            };
            return dataQuery;
        }

        function getFilterConditions() {
            var result = {};
            var isAnyFilterApplied = !!filter.current || filter.keyword;

            result.isAnyFilterApplied = isAnyFilterApplied;
            angular.extend(result, filter.current);

            var dataQuery = blade.exportDataRequest.dataQuery;
            if (dataQuery.objectIds && dataQuery.length) {
                angular.extend(result, { objectIds: dataQuery.objectIds });
            }

            if (filter.keyword) {
                angular.extend(result, { keyword: filter.keyword });
            }

            return result;
        }

        function resetStateGrid() {
            if ($scope.gridApi) {
                $scope.items = [];
                $scope.gridApi.selection.clearSelectedRows();
                $scope.gridApi.infiniteScroll.resetScroll(true, true);
                $scope.gridApi.infiniteScroll.dataLoaded();
            }
        }

        blade.setSelectedItem = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectItem = function (e, listItem) {
            blade.setSelectedItem(listItem);

            var viewerFunc = genericViewerItemService.getViewer(listItem.type);

            if (viewerFunc) {
                bladeNavigationService.showBlade(viewerFunc(listItem), blade);
            }
        };


        if (!$localStorage.exportSearchFilters) {
            $localStorage.exportSearchFilters = {};
        }

        if (!$localStorage.exportSearchFilters[blade.exportDataRequest.exportTypeName]) {
            $localStorage.exportSearchFilters[blade.exportDataRequest.exportTypeName] = [{ name: 'export.blades.export-generic-viewer.labels.new-filter' }];
        }

        $scope.exportSearchFilters = $localStorage.exportSearchFilters[blade.exportDataRequest.exportTypeName];

        if (!$localStorage.exportSearchFilterIds) {
            $localStorage.exportSearchFilterIds = {};
        }

        $scope.exportSearchFilterId = $localStorage.exportSearchFilterIds[blade.exportDataRequest.exportTypeName];

        if ($scope.exportSearchFilterId) {
            filter.current = _.findWhere($scope.exportSearchFilters, { id: $scope.exportSearchFilterId });
        }

        if (blade.exportDataRequest.dataQuery) {
            if (!filter.current) {
                filter.current = {};
            }
            angular.extend(filter.current, blade.exportDataRequest.dataQuery);
        }

        filter.change = function () {
            var exportSearchFilterId = filter.current ? filter.current.id : null;
            var metafieldsId = blade.exportDataRequest.exportTypeName + 'ExportFilter';
            $localStorage.exportSearchFilterIds[blade.exportDataRequest.exportTypeName] = exportSearchFilterId;
            if (filter.current && !filter.current.id) {
                filter.current = null;
                showFilterDetailBlade({ isNew: true, metafieldsId: metafieldsId, exportTypeName: blade.exportDataRequest.exportTypeName });
            } else {
                bladeNavigationService.closeBlade({ id: 'exportGenericViewerFilter' });

                if (!filter.current) {
                    blade.resetRequestCustomFilter();
                }

                filter.criteriaChanged();
            }
        };

        filter.edit = function () {
            var metafieldsId = blade.exportDataRequest.exportTypeName + 'ExportFilter';
            var filterDetailsParams = {
                data: filter.current,
                metafieldsId: metafieldsId,
                exportTypeName: blade.exportDataRequest.exportTypeName
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

            if (blade.exportDataRequest.dataQuery) {
                blade.exportDataRequest.dataQuery.keyword = undefined;
            }
        }

        blade.toolbarCommands = [{
            name: "platform.commands.refresh",
            icon: 'fa fa-refresh',
            executeMethod: function () {
                blade.resetFiltering();
                blade.refresh();
            },
            canExecuteMethod: function () {
                return true;
            }
        }];

        $scope.setGridOptions = function (gridOptions) {
            bladeUtils.initializePagination($scope, true);
            $scope.pageSettings.itemsPerPageCount = 20;

            uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                //update gridApi for current grid
                $scope.gridApi = gridApi;

                uiGridHelper.bindRefreshOnSortChanged($scope);
                $scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, showMore);

            });

            // need to call refresh after digest cycle as we do not "$watch" for $scope.pageSettings.currentPage
            $timeout(function () {
                blade.refresh();
            });
        };

        $scope.cancelChanges = function () {
            bladeNavigationService.closeBlade(blade);
        };

        $scope.isValid = function () {
            return ($scope.items && $scope.items.length);
        };

        $scope.saveChanges = function () {
            var dataQuery = buildDataQuery();
            var isAllSelected = $scope.gridApi.selection.getSelectAllState();
            var selectedIds = _.map($scope.gridApi.selection.getSelectedRows(), function (item) { return item.id; });

            dataQuery.isAllSelected = isAllSelected;

            if ((blade.exportDataRequest.dataQuery.objectIds && blade.exportDataRequest.dataQuery.objectIds.length)
                || (!isAllSelected)) {
                if (selectedIds.length) {
                    dataQuery.objectIds = selectedIds;
                }
                else {
                    dataQuery.objectIds = [];
                }
            }

            if (blade.onCompleted) {
                blade.onCompleted(dataQuery);
            }

            bladeNavigationService.closeBlade(blade);
        };
    }]);
