angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportGenericViewerController', 
    ['$localStorage', '$timeout', '$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi',
    function ($localStorage, $timeout, $scope, bladeUtils, uiGridHelper, bladeNavigationService, exportModuleApi) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        $scope.hasMore = true;
        $scope.items = [];
        
        var blade = $scope.blade;
        blade.isLoading = true;
        blade.isExpanded = true;
        $scope.blade.headIcon = 'fa-upload';

        function initializeBlade() {
        }

        blade.refresh = function () {
            loadData();

            resetStateGrid();
        };

        function loadData() {
            blade.isLoading = true;
            angular.extend(blade.exportDataRequest.dataQuery, buildDataQuery());

            exportModuleApi.getData(
                blade.exportDataRequest,
                function (data) {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    $scope.items = data.results;
                });
        }

        blade.resetFiltering = function() {
            filter.keyword = undefined;
            resetFilterConditions();
            blade.exportDataRequest.dataQuery = getEmptyDataQuery();
        };

        function buildDataQuery()
        {
            var dataQuery = getEmptyDataQuery();
            
            if (filter.keyword) {
                angular.extend(dataQuery, {keyword: filter.keyword});
            }

            angular.extend(dataQuery, getFilterConditions());

            return dataQuery;
        }

        function getEmptyDataQuery() {

            var dataQuery = {
                exportTypeName: blade.exportDataRequest.dataQuery.exportTypeName,
                includedColumns: blade.exportDataRequest.dataQuery.includedColumns,
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount,
            };
            return dataQuery;
        }

        function getFilterConditions() {
            var result = {};
            var isAnyFilterApplied = false;

            result.isAnyFilterApplied = isAnyFilterApplied;

            return result;
        }

        function resetFilterConditions() {
        }

        function resetStateGrid() {
            if ($scope.gridApi) {
                $scope.items = [];
                $scope.gridApi.selection.clearSelectedRows();
            }
        }

        blade.setSelectedItem = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectItem = function (e, listItem) {
            blade.setSelectedItem(listItem);
            // TODO item view on select
        };

        var filter = blade.filter = $scope.filter = {};
        $scope.$localStorage = $localStorage;
        if (!$localStorage.exportSearchFilters) {
            $localStorage.exportSearchFilters = [{ name: 'export.blades.export-generic-viewer.labels.new-filter' }];
        }
        if ($localStorage.exportSearchFilterId) {
            filter.current = _.findWhere($localStorage.exportSearchFilters, { id: $localStorage.exportSearchFilterId });
        }

        filter.change = function () {
            $localStorage.exportSearchFilterId = filter.current ? filter.current.id : null;
            if (filter.current && !filter.current.id) {
                filter.current = null;
                showFilterDetailBlade({ isNew: true });
            } else {
                bladeNavigationService.closeBlade({ id: 'filterDetail' });
                filter.criteriaChanged();
            }
        };

        filter.edit = function () {
            if (filter.current) {
                showFilterDetailBlade({ data: filter.current });
            }
        };

        function showFilterDetailBlade(bladeData) {
            var newBlade = {
                id: 'filterDetail',
                controller: 'virtoCommerce.exportModule.filterDetailController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/filter-detail.tpl.html'
            };
            angular.extend(newBlade, bladeData);
            bladeNavigationService.showBlade(newBlade, blade);
        }

        filter.criteriaChanged = function () {
            if ($scope.pageSettings.currentPage > 1) {
                $scope.pageSettings.currentPage = 1;
            } else {
                blade.refresh();
            }
        };

        blade.toolbarCommands = [{
            name: 'export.blades.export-generic-viewer.commands.select',
            icon: 'fa fa-save',
            canExecuteMethod: function () {
                return ($scope.items && $scope.items.length);
            },
            executeMethod: function () {
                var dataQuery = buildDataQuery();
                var selectedIds = _.map($scope.gridApi.selection.getSelectedRows(), function(item) { return item.id; });

                if (selectedIds.length) {
                    dataQuery.objectIds = selectedIds;
                } else {
                    dataQuery.isAllSelected = true;
                }

                if (blade.onCompleted) {
                    blade.onCompleted(dataQuery);
                }

                bladeNavigationService.closeBlade(blade);
            }
        }, {
            name: 'platform.commands.cancel',
            icon: 'fa fa-times',
            canExecuteMethod: function () {
                return true;
            },
            executeMethod: function () {
                bladeNavigationService.closeBlade(blade);
            }
        }, {
            name: "platform.commands.reset",
            icon: 'fa fa-undo',
            executeMethod: function() {
                if ($scope.pageSettings.currentPage > 1) {
                    $scope.pageSettings.currentPage = 1;
                }

                blade.resetFiltering();
                blade.refresh();
            },
            canExecuteMethod: function () {
                return true;
            }
        }];

        $scope.setGridOptions = function (gridOptions) {

            //disable watched
            bladeUtils.initializePagination($scope, true);
            //сhoose the optimal amount that ensures the appearance of the scroll
            $scope.pageSettings.itemsPerPageCount = 20;

            uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                //update gridApi for current grid
                $scope.gridApi = gridApi;

                uiGridHelper.bindRefreshOnSortChanged($scope);
            });

            bladeUtils.initializePagination($scope);
        };

        initializeBlade();
    }]);
