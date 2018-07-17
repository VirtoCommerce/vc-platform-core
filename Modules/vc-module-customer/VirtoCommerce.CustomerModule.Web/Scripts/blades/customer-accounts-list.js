﻿angular.module('virtoCommerce.customerModule')
.controller('virtoCommerce.customerModule.customerAccountsListController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeNavigationService', 'filterFilter', 'platformWebApp.accounts',
function ($scope, dialogService, uiGridHelper, bladeNavigationService, filterFilter, accounts) {
    $scope.uiGridConstants = uiGridHelper.uiGridConstants;
    var blade = $scope.blade;

    blade.refresh = function () {
        blade.isLoading = true;
        blade.parentBlade.refresh();
    };

    function initializeBlade(data) {
        blade.memberId = data.id;
        blade.currentEntities = angular.copy(data.securityAccounts);
        blade.origEntity = data.securityAccounts;
        blade.isLoading = false;
    }

    blade.selectNode = function (node) {
        if (bladeNavigationService.checkPermission('platform:security:read')) {
            $scope.selectedNodeId = node.userName;

            var newBlade = {
                id: 'listItemChild',
                data: node,
                title: node.userName,
                subtitle: blade.subtitle,
                controller: 'platformWebApp.accountDetailController',
                template: '$(Platform)/Scripts/app/security/blades/account-detail.tpl.html'
            };

            bladeNavigationService.showBlade(newBlade, blade);
        } else {
            bladeNavigationService.setError('Insufficient permission', blade);
        }
    };

    function openNewAccountWizard(store) {
        var newBlade = {
            id: 'newAccountWizard',
            currentEntity: { roles: [], userType: 'Customer', storeId: store.id, memberId: blade.memberId },
            title: 'platform.blades.account-detail.title-new',
            subtitle: blade.subtitle,
            controller: 'platformWebApp.newAccountWizardController',
            template: '$(Platform)/Scripts/app/security/wizards/newAccount/new-account-wizard.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    $scope.deleteList = function (selection) {
        var dialog = {
            id: "confirmDeleteItem",
            title: "platform.dialogs.account-delete.title",
            message: "platform.dialogs.account-delete.message",
            callback: function (remove) {
                if (remove) {
                    bladeNavigationService.closeChildrenBlades(blade, function () {
                        var itemIds = _.pluck(selection, 'userName');
                        accounts.remove({ names: itemIds }, blade.refresh);
                    });
                }
            }
        };
        dialogService.showConfirmationDialog(dialog);
    };

    blade.headIcon = 'fa-key';

    blade.toolbarCommands = [
        {
            name: "platform.commands.add", icon: 'fa fa-plus',
            executeMethod: function () {
                bladeNavigationService.closeChildrenBlades(blade, function () {
                    var newBlade = {
                        id: 'pickStoreList',
                        title: 'customer.blades.pick-store-list.title',
                        subtitle: 'customer.blades.pick-store-list.subtitle',
                        onAfterNodeSelected: openNewAccountWizard,
                        controller: 'virtoCommerce.customerModule.pickStoreListController',
                        template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/pick-store-list.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                });
            },
            canExecuteMethod: function () { return true; },
            permission: 'platform:security:create'
        },
        {
            name: "platform.commands.delete", icon: 'fa fa-trash-o',
            executeMethod: function () { $scope.deleteList($scope.gridApi.selection.getSelectedRows()); },
            canExecuteMethod: function () {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            },
            permission: 'platform:security:delete'
        //},
        //{
        //    name: "customer.blades.customer-accounts-list.commands.link", icon: 'fa fa-link',
        //    executeMethod: function () { },
        //    canExecuteMethod: function () { return true; },
        //    permission: 'platform:security:update'
        //},
        //{
        //    name: "customer.blades.customer-accounts-list.commands.unlink", icon: 'fa fa-chain-broken',
        //    executeMethod: function () { $scope.unlinkList($scope.gridApi.selection.getSelectedRows()); },
        //    canExecuteMethod: function () {
        //        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
        //    },
        //    permission: 'platform:security:update'
        }
    ];

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
            gridApi.grid.registerRowsProcessor($scope.singleFilter, 90);
        });
    };

    $scope.singleFilter = function (renderableRows) {
        var visibleCount = 0;
        renderableRows.forEach(function (row) {
            row.visible = _.any(filterFilter([row.entity], blade.searchText));
            if (row.visible) visibleCount++;
        });

        $scope.filteredEntitiesCount = visibleCount;
        return renderableRows;
    };

    $scope.$watch('blade.parentBlade.currentEntity', initializeBlade);

    // on load:
    // $scope.$watch('blade.parentBlade.currentEntity.securityAccounts' gets fired    
}]);