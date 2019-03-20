angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.couponListController', ['$scope', '$localStorage', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'virtoCommerce.marketingModule.promotions',
function ($scope, $localStorage, dialogService, bladeUtils, uiGridHelper, promotionsApi) {
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;
    blade.headIcon = 'fa-ticket';
    blade.isLoading = false;

    blade.refresh = function () {
        blade.isLoading = true;
        var criteria = {
            promotionId: blade.promotionId,
            sort: uiGridHelper.getSortExpression($scope),
            code: filter.keyword,
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        };
        if (filter.current) {
            angular.extend(criteria, filter.current);
        }

        promotionsApi.searchCoupons(criteria, function (response) {
            blade.isLoading = false;
            blade.currentEntities = response.results;
            blade.parentBlade.couponCount();
            $scope.pageSettings.totalItems = response.totalCount;
        });

    }

    $scope.$on('coupon-import-finished', function (event) {
        blade.refresh();
    });

    blade.toolbarCommands = [{
        name: 'marketing.blades.coupons.toolbar.add',
        icon: 'fa fa-plus',
        canExecuteMethod: function () {
            return true;
        },
        executeMethod: showAddCouponBlade
    }, {
        name: 'marketing.blades.coupons.toolbar.import',
        icon: 'fa fa-download',
        canExecuteMethod: function () {
            return true;
        },
        executeMethod: showCouponImportBlade
    }, {
        name: 'marketing.blades.coupons.toolbar.refresh',
        icon: 'fa fa-refresh',
        canExecuteMethod: function () {
            return true;
        },
        executeMethod: blade.refresh
    }, {
        name: 'marketing.blades.coupons.toolbar.delete',
        icon: 'fa fa-trash-o',
        canExecuteMethod: function () {
            return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
        },
        executeMethod: function () {
            removeCoupons(false);
        }
    }];

    var filter = blade.filter = $scope.filter = {};
    $scope.$localStorage = $localStorage;
    if (!$localStorage.couponSearchFilters) {
        $localStorage.couponSearchFilters = [{ name: 'marketing.blades.promotion-list.new-filter' }];
    }
    if ($localStorage.couponSearchFilterId) {
        filter.current = _.findWhere($localStorage.couponSearchFilters, { id: $localStorage.couponSearchFilterId });
    }
    filter.change = function () {
        $localStorage.couponSearchFilterId = filter.current ? filter.current.id : null;
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
    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    $scope.selectNode = function (node) {
        var newBlade = {
            id: 'couponDetail',
            originalEntity: angular.copy(node),
            currentEntity: angular.copy(node),
            title: 'Coupon "' + node.code + '"',
            controller: 'virtoCommerce.marketingModule.couponDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/coupon-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
        selectedEntity = node;
    };

    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
            uiGridHelper.bindRefreshOnSortChanged($scope);
        });

        bladeUtils.initializePagination($scope);
    };

    function showAddCouponBlade() {
        var newBlade = {
            id: 'couponImport',
            title: 'marketing.blades.coupon-detail.new-title',
            promotionId: blade.promotionId,
            currentEntity: {
                promotionId: blade.promotionId,
                isNew: true
            },
            controller: 'virtoCommerce.marketingModule.couponDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/coupon-detail.tpl.html'
        }
        bladeNavigationService.showBlade(newBlade, blade);
    }

    function showCouponImportBlade() {
        var newBlade = {
            id: 'couponImport',
            title: 'marketing.blades.coupons-import.title',
            promotionId: blade.promotionId,
            controller: 'virtoCommerce.marketingModule.couponImportController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/coupon-import.tpl.html'
        }
        bladeNavigationService.showBlade(newBlade, blade);
    }

    function removeCoupons(removeAll, promotionId) {
        var dialog = {
            id: "confirmDeleteItem",
            title: "marketing.dialogs.coupon-delete.title",
            message: removeAll ? "marketing.dialogs.coupon-delete.message-all": "marketing.dialogs.coupon-delete.message",
            callback: function (remove) {
                if (remove) {
                    bladeNavigationService.closeChildrenBlades(blade, function () {
                        blade.isLoading = true;
                        if (removeAll && promotionId) {
                            promotionsApi.clearCoupons({ promotionId: promotionId }, function () {
                                blade.refresh();
                                blade.parentBlade.refresh();
                            });
                        } else if (!removeAll) {
                            var itemIds = _.pluck($scope.gridApi.selection.getSelectedRows(), 'id');
                            promotionsApi.deleteCoupons({ ids: itemIds, all: removeAll }, function () {
                                blade.refresh();
                                blade.parentBlade.refresh();
                            });
                        }
                    });
                }
            }
        };
        dialogService.showConfirmationDialog(dialog);
    }

    function showFilterDetailBlade(bladeData) {
        var newBlade = {
            id: 'filterDetail',
            controller: 'virtoCommerce.marketingModule.filterDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/filter-detail.tpl.html'
        };
        angular.extend(newBlade, bladeData);
        bladeNavigationService.showBlade(newBlade, blade);
    }
}]);
