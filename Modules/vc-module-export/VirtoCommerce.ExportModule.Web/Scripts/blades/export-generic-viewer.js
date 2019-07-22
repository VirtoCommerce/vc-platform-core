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
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            };
            return dataQuery;
        }

        function getFilterConditions() {
            return {};
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
            // var newBlade;
            // if (listItem.type === 'category') {
            //     var openNewBlade = e.ctrlKey || filter.keyword;
            //     newBlade = {
            //         id: 'itemsList' + (blade.level + (openNewBlade ? 1 : 0)),
            //         level: blade.level + (openNewBlade ? 1 : 0),
            //         mode: blade.mode,
            //         isBrowsingLinkedCategory: blade.isBrowsingLinkedCategory || $scope.hasLinks(listItem),
            //         breadcrumbs: blade.breadcrumbs,
            //         title: 'catalog.blades.categories-items-list.title',
            //         subtitle: 'catalog.blades.categories-items-list.subtitle',
            //         subtitleValues: listItem.name !== null ? { name: listItem.name } : '',
            //         catalogId: blade.catalogId || listItem.catalogId,
            //         categoryId: listItem.id,
            //         category: listItem,
            //         catalog: blade.catalog,
            //         disableOpenAnimation: true,
            //         controller: 'virtoCommerce.catalogModule.categoriesItemsListController',
            //         template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/categories-items-list.tpl.html'
            //     };

            //     if (openNewBlade) {
            //         bladeNavigationService.showBlade(newBlade, blade);
            //     } else {
            //         bladeNavigationService.closeBlade(blade, function () {
            //             bladeNavigationService.showBlade(newBlade, blade.parentBlade);
            //         });
            //     }
            // } else {
            //     newBlade = {
            //         id: "listItemDetail" + blade.mode,
            //         itemId: listItem.id,
            //         productType: listItem.productType,
            //         title: listItem.name,
            //         catalog: blade.catalog,
            //         controller: 'virtoCommerce.catalogModule.itemDetailController',
            //         template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
            //     };
            //     bladeNavigationService.showBlade(newBlade, blade);

            //     // setting current categoryId to be globally available
            //     bladeNavigationService.catalogsSelectedCategoryId = blade.categoryId;
            // }
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
            name: 'platform.commands.pick-selected',
            icon: 'fa fa-save',
            canExecuteMethod: function () {
                return $scope.gridApi && $scope.gridApi.selection.getSelectedRows() && $scope.gridApi.selection.getSelectedRows().length;
            },
            executeMethod: function () {
                var dataQuery = buildDataQuery();
                dataQuery.objectIds = _.map($scope.gridApi.selection.getSelectedRows(), function(item) { return item.id; });
                dataQuery.isAllSelected = false;

                if (blade.onCompleted) {
                    blade.onCompleted(dataQuery);
                }

                bladeNavigationService.closeBlade(blade);
            }
        }, {
            name: 'export.commands.pick-all',
            icon: 'fa fa-save',
            canExecuteMethod: function () {
                return true;
            },
            executeMethod: function () {
                var dataQuery = buildDataQuery();
                dataQuery.isAllSelected = true;

                if (blade.onCompleted) {
                    blade.onCompleted(dataQuery);
                }
                
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
        }, {
            name: 'platform.commands.cancel',
            icon: 'fa fa-times',
            canExecuteMethod: function () {
                return true;
            },
            executeMethod: function () {
                bladeNavigationService.closeBlade(blade);
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
