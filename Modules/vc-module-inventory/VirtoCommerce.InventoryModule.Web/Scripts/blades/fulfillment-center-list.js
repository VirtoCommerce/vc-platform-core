angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.fulfillmentListController', ['$scope', 'virtoCommerce.inventoryModule.fulfillments', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService',
        function ($scope, fulfillments, bladeNavigationService, bladeUtils, uiGridConstants, uiGridHelper, dialogService) {
            $scope.uiGridConstants = uiGridConstants;
            var blade = $scope.blade;


            // actions on load
            blade.title = 'core.blades.fulfillment-center-list.title';
            blade.subtitle = 'core.blades.fulfillment-center-list.subtitle';

            blade.refresh = function (parentRefresh) {
                blade.isLoading = true;

                fulfillments.search({
                        searchPhrase: filter.keyword ? filter.keyword : undefined,
                        sort: uiGridHelper.getSortExpression($scope),
                        skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                        take: $scope.pageSettings.itemsPerPageCount
                    }, function (response) {
                        blade.isLoading = false;
                        blade.currentEntities = response.results;
                        if (parentRefresh === true) {
                            blade.parentBlade.refresh();
                        }
                        return response.results;
                    });
            };

            function showDetailBlade(node, title) {
                $scope.selectedNodeId = node.id;

                var newBlade = {
                    id: 'fulfillmentDetail',
                    currentEntityId: node.id,
                    currentEntity: node,
                    title: title,
                    subtitle: 'inventory.blades.fulfillment-center-detail.subtitle',
                    controller: 'virtoCommerce.inventoryModule.fulfillmentCenterDetailController',
                    template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/blades/fulfillment-center-detail.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            blade.selectNode = function (node) {
                showDetailBlade(node, node.name);
            };

            blade.headIcon = 'fa-wrench';
            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () {
                        showDetailBlade({ maxReleasesPerPickBatch: 20, pickDelay: 30 }, 'New Fulfillment center');
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'inventory:fulfillment:edit'
                }
            ];

        

            $scope.delete = function (item) {
                var dialog = {
                    id: "confirmDelete",
                    title: "inventory.dialogs.fulfillment-delete.title",
                    message: "inventory.dialogs.fulfillment-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;
                            fulfillments.remove({ ids: item.id }, function () {
                                blade.refresh(true);
                                blade.isLoading = false;
                            }, function (error) {
                                bladeNavigationService.setError('Error ' + error.status, blade);
                            });
                        }
                    }
                }
                dialogService.showConfirmationDialog(dialog);
            }

            // simple and advanced filtering
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
                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                });
                bladeUtils.initializePagination($scope);
            };

        }]);
