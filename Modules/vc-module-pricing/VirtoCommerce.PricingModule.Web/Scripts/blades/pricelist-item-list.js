angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.pricelistItemListController', ['$scope', 'virtoCommerce.pricingModule.prices', '$filter', 'platformWebApp.bladeNavigationService', 'uiGridConstants', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'platformWebApp.dialogService', function ($scope, prices, $filter, bladeNavigationService, uiGridConstants, uiGridHelper, bladeUtils, dialogService) {
    $scope.uiGridConstants = uiGridConstants;
    var blade = $scope.blade;

    blade.refresh = function () {
        blade.isLoading = true;

        prices.search({
            priceListId: blade.currentEntityId,
            keyword: filter.keyword,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        }, function (data) {
            blade.currentEntities = data.results;
            $scope.pageSettings.totalItems = data.totalCount;

            blade.isLoading = false;
        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    };

    $scope.selectNode = function (node) {
        $scope.selectedNodeId = node.productId;

        var newBlade = {
            id: 'itemPrices',
            itemId: node.productId,
            priceListId: blade.currentEntityId,
            data: node,
            currency: blade.currency,
            title: 'pricing.blades.prices-list.title',
            titleValues: { name: node.product.name },
            subtitle: 'pricing.blades.prices-list.subtitle',
            controller: 'virtoCommerce.pricingModule.pricesListController',
            template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/prices-list.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, blade);
    };

    function openAddEntityWizard() {
        $scope.selectedNodeId = null;
        var selectedProducts = [];
        var newBlade = {
            id: "CatalogItemsSelect",
            title: "Select items for pricing", //catalogItemSelectController: hard-coded title
            controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
            breadcrumbs: [],
            toolbarCommands: [
        {
            name: "pricing.commands.add-selected", icon: 'fa fa-plus',
            executeMethod: function (blade) {
                addProductsToPricelist(selectedProducts, blade);
            },
            canExecuteMethod: function () {
                return selectedProducts.length > 0;
            }
        }]
        };

        newBlade.options = {
            checkItemFn: function (listItem, isSelected) {
                if (listItem.type == 'category') {
                    newBlade.error = 'Categories are not supported';
                    listItem.selected = undefined;
                } else {
                    if (isSelected) {
                        if (_.all(selectedProducts, function (x) { return x.id != listItem.id; })) {
                            selectedProducts.push(listItem);
                        }
                    }
                    else {
                        selectedProducts = _.reject(selectedProducts, function (x) { return x.id == listItem.id; });
                    }
                    newBlade.error = undefined;
                }
            }
        };

        bladeNavigationService.showBlade(newBlade, blade);
    }

    function addProductsToPricelist(products, theBlade) {
        theBlade.isLoading = true;

        // search for possible duplicating prices
        prices.search({
            priceListId: blade.currentEntityId,
            productIds: _.pluck(products, 'id')
        }, function (data) {
            var newItems = _.filter(products, function (product) {
                return _.all(data.results, function (x) {
                    return x.productId != product.id;
                })
            });

            var newProductPrices = _.map(newItems, function (x) {
                return {
                    // productId: x.id,
                    prices: [{ productId: x.id, list: 0, minQuantity: 1, currency: blade.currency, priceListId: blade.currentEntityId }]
                };
            });

            prices.update(newProductPrices, function () {
                bladeNavigationService.closeBlade(theBlade);
                blade.refresh();
                blade.parentWidgetRefresh();
            }, function (error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
            });
        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    }

    $scope.deleteList = function (list) {
        var dialog = {
            id: "confirmDeleteItem",
            title: "pricing.dialogs.pricelist-item-list-delete.title",
            message: "pricing.dialogs.pricelist-item-list-delete.message",
            callback: function (remove) {
                if (remove) {
                    bladeNavigationService.closeChildrenBlades(blade, function () {
                        prices.remove({ priceListId: blade.currentEntityId, productIds: _.pluck(list, 'productId') }, function () {
                            blade.refresh();
                            blade.parentWidgetRefresh();
                        },
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    blade.toolbarCommands = [
        {
            name: "platform.commands.refresh", icon: 'fa fa-refresh',
            executeMethod: blade.refresh,
            canExecuteMethod: function () { return true; }
        },
        {
            name: "platform.commands.add", icon: 'fa fa-plus',
            executeMethod: openAddEntityWizard,
            canExecuteMethod: function () { return true; },
            permission: blade.updatePermission
        },
        {
            name: "platform.commands.delete", icon: 'fa fa-trash-o',
            executeMethod: function () {
                $scope.deleteList($scope.gridApi.selection.getSelectedRows());
            },
            canExecuteMethod: function () {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            },
            permission: blade.updatePermission
        }
    ];

    $scope.getPriceRange = function (priceGroup) {
        var retVal;
        var allPrices = _.union(_.pluck(priceGroup.prices, 'list'), _.pluck(priceGroup.prices, 'sale'));
        var minprice = $filter('currency')(_.min(allPrices), '', 2);
        var maxprice = $filter('currency')(_.max(allPrices), '', 2);
        retVal = (minprice == maxprice ? minprice : minprice + '-' + maxprice);

        //else {
        //    retVal = 'NO PRICE';
        //}

        return retVal;
    }

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

    //No need to call this because page 'pageSettings.currentPage' is watched!!! It would trigger subsequent duplicated req...
    //blade.refresh();
}]);