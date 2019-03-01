angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.catalogItemSelectController', ['$scope', 'virtoCommerce.catalogModule.catalogs', 'virtoCommerce.catalogModule.listEntries', 'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension', '$timeout',
    function ($scope, catalogs, listEntries, bladeUtils, uiGridConstants, uiGridHelper, gridOptionExtension, $timeout) {
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;

    if (!blade.title) {
        blade.title = "Select Catalog items...";
    }

    $scope.options = angular.extend({
        showCheckingMultiple: true,
        allowCheckingItem: true,
        allowCheckingCategory: false,
        selectedItemIds: [],
        gridColumns: []
    }, blade.options);

    blade.refresh = function () {
        blade.isLoading = true;
        filter.searchedKeyword = filter.keyword;

        if (!$scope.isCatalogSelectMode()) {
            listEntries.listitemssearch(
                angular.extend({
                    catalogId: blade.catalogId,
                    categoryId: blade.categoryId,
                    keyword: filter.keyword,
                    searchInVariations : filter.searchInVariations ? filter.searchInVariations : false,
                    responseGroup: 'withCategories, withProducts',                    
                    sort: uiGridHelper.getSortExpression($scope),
                    skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                    take: $scope.pageSettings.itemsPerPageCount
                }, blade.searchCriteria),
            function (data, headers) {
                blade.isLoading = false;
                $scope.pageSettings.totalItems = data.totalCount;
                $scope.items = data.listEntries;
                if ($scope.options.onItemsLoaded) {
                    $scope.options.onItemsLoaded(data.listEntries);
                }

                //Set navigation breadcrumbs
                setBreadcrumbs();

            });
        }
        else {
            catalogs.getCatalogs({}, function (results) {
                blade.isLoading = false;

                $scope.items = results;
                //Set navigation breadcrumbs
                setBreadcrumbs();

            });
        }
    }

    //Breadcrumbs
    function setBreadcrumbs() {
        //Clone array (angular.copy leave a same reference)
        blade.breadcrumbs = blade.breadcrumbs.slice(0);

        //catalog breadcrumb by default
        var breadCrumb = {
            id: blade.catalogId ? blade.catalogId : "All",
            name: blade.catalog ? blade.catalog.name : "All",
            blade: $scope.blade
        };

        //if category need change to category breadcrumb
        if (blade.category) {

            breadCrumb.id = blade.categoryId;
            breadCrumb.name = blade.category.name;
        }

        //prevent duplicate items
        if (!_.some(blade.breadcrumbs, function (x) { return x.id == breadCrumb.id })) {
            blade.breadcrumbs.push(breadCrumb);
        }

        breadCrumb.navigate = function (breadcrumb) {
            bladeNavigationService.closeBlade($scope.blade,
                        function () {
                            if (breadcrumb.id == "All") {
                                blade.catalogId = null;
                                blade.filter.keyword = null;
                            }
                            bladeNavigationService.showBlade($scope.blade, blade.parentBlade);
                         });
        };
    }

    $scope.isCatalogSelectMode = function () {
        return !blade.catalogId && !filter.searchedKeyword;
    };

    $scope.selectItem = function (e, listItem) {
        //if ($scope.selectedNodeId == listItem.id)
        //    return;

        $scope.selectedNodeId = listItem.id;
        //call callback function
        if ($scope.options.selectItemFn) {
            $scope.options.selectItemFn(listItem);
        };
        
        var newBlade = {
            id: blade.id,
            breadcrumbs: blade.breadcrumbs,
            catalogId: blade.catalogId,
            catalog: blade.catalog,
            controller: blade.controller,
            template: blade.template,
            options: $scope.options,
            searchCriteria: blade.searchCriteria,
            toolbarCommands: blade.toolbarCommands
        };

        if ($scope.isCatalogSelectMode()) {
            newBlade.catalogId = listItem.id;
            newBlade.catalog = listItem;
            bladeNavigationService.closeBlade(blade, function () {
                bladeNavigationService.showBlade(newBlade, blade.parentBlade);
            });

            // setting current catalog to be globally available 
            bladeNavigationService.catalogsSelectedCatalog = listItem;
            bladeNavigationService.catalogsSelectedCategoryId = undefined;
        }
        else if (listItem.type === 'category') {
            newBlade.categoryId = listItem.id;
            newBlade.category = listItem;

            bladeNavigationService.closeBlade(blade, function () {
                bladeNavigationService.showBlade(newBlade, blade.parentBlade);
            });
        }
        else {
            //default blade for product details
            newBlade = {
                id: "listItemDetail",
                itemId: listItem.id,
                productType: listItem.productType,
                title: listItem.name,
                catalog: blade.catalog,
                variationsToolbarCommandsAndEvents: { toolbarCommands: blade.toolbarCommands, externalRegisterApiCallback: externalRegisterApiCallback },
                controller: 'virtoCommerce.catalogModule.itemDetailController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
            };

            //extension point allows to use custom views for product details
            if ($scope.options.fnGetBladeForItem) {
                var customBlade = $scope.options.fnGetBladeForItem(listItem);
                if (customBlade) {
                    newBlade = customBlade;
                }
            }
           
            bladeNavigationService.showBlade(newBlade, blade);

            // setting current categoryId to be globally available
            bladeNavigationService.catalogsSelectedCategoryId = blade.categoryId;
        }
    };

    // simple and advanced filtering

    var filter = blade.filter = blade.filter || { keyword: '' };

    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    $scope.setGridOptions = function (gridId, gridOptions) {
        gridOptions.isRowSelectable = angular.isFunction($scope.options.isItemSelectable)
            ? function(row) {
                return $scope.options.isItemSelectable(row.entity);
            }
            : function(row) {
                return $scope.options.allowCheckingItem && row.entity.type !== 'category' ||
                    $scope.options.allowCheckingCategory && row.entity.type === 'category';
            };

        gridOptions.columnDefs = gridOptions.columnDefs.concat($scope.options.gridColumns);
        gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);
        uiGridHelper.initialize($scope, gridOptions, externalRegisterApiCallback);
        bladeUtils.initializePagination($scope);
    };

    function externalRegisterApiCallback(gridApi) {
        //update gridApi for current grid
        $scope.gridApi = gridApi;

        gridApi.grid.registerDataChangeCallback(function (grid) {
            //check already selected rows
            $timeout(function () {
                _.each($scope.items, function (x) {
                    if (_.some($scope.options.selectedItemIds, function (y) { return y == x.id; })) {
                        gridApi.selection.selectRow(x);
                    }
                });
            });
        }, [uiGridConstants.dataChange.ROW]);

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            if ($scope.options.checkItemFn) {
                $scope.options.checkItemFn(row.entity, row.isSelected);
            };
            if (row.isSelected) {
                if (!_.contains($scope.options.selectedItemIds, row.entity.id)) {
                    $scope.options.selectedItemIds.push(row.entity.id);
                }
            }
            else {
                $scope.options.selectedItemIds = _.without($scope.options.selectedItemIds, row.entity.id);
            }
        });

        uiGridHelper.bindRefreshOnSortChanged($scope);
    }
}]);
