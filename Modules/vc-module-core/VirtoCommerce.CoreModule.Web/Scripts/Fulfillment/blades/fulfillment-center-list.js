angular.module('virtoCommerce.coreModule.fulfillment')
    .controller('virtoCommerce.coreModule.fulfillment.fulfillmentListController', ['$scope', 'virtoCommerce.coreModule.fulfillment.fulfillments', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'uiGridConstants', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService',
        function ($scope, fulfillments, bladeNavigationService, bladeUtils, uiGridConstants, uiGridHelper, dialogService) {
            $scope.uiGridConstants = uiGridConstants;
            var blade = $scope.blade;

            blade.refresh = function (parentRefresh) {
                blade.isLoading = true;

                fulfillments.query({}, function (results) {
                    blade.isLoading = false;
                    blade.currentEntities = results;
                    if (parentRefresh === true) {
                        blade.parentBlade.refresh();
                    }
                    return results;
                }, function (error) {
                    bladeNavigationService.setError('Error ' + error.status, blade);
                });
            };

            function showDetailBlade(node, title) {
                $scope.selectedNodeId = node.id;

                var newBlade = {
                    id: 'fulfillmentDetail',
                    currentEntityId: node.id,
                    currentEntity: node,
                    title: title,
                    subtitle: 'core.blades.fulfillment-center-detail.subtitle',
                    controller: 'virtoCommerce.coreModule.fulfillment.fulfillmentCenterDetailController',
                    template: 'Modules/$(VirtoCommerce.Core)/Scripts/fulfillment/blades/fulfillment-center-detail.tpl.html'
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
                    permission: 'core:fulfillment:create'
                }
            ];

            // actions on load
            blade.title = 'core.blades.fulfillment-center-list.title',
                blade.subtitle = 'core.blades.fulfillment-center-list.subtitle',
                blade.refresh();

            $scope.delete = function (item) {
                var dialog = {
                    id: "confirmDelete",
                    title: "core.dialogs.fulfillment-delete.title",
                    message: "core.dialogs.fulfillment-delete.message",
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

            $scope.setGridOptions = function (gridOptions) {
                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                });
                bladeUtils.initializePagination($scope);
            };
        }]);