angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.linkListsController', ['$rootScope', '$scope', 'virtoCommerce.contentModule.menus', 'virtoCommerce.storeModule.stores', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'uiGridConstants', 'platformWebApp.uiGridHelper',
    function ($rootScope, $scope, menus, stores, bladeNavigationService, dialogService, uiGridConstants, uiGridHelper) {
        $scope.uiGridConstants = uiGridConstants;
        var blade = $scope.blade;

        blade.refresh = function () {
            blade.isLoading = true;
            menus.get({ storeId: blade.storeId }, function (data) {
                blade.currentEntities = data;
                blade.isLoading = false;
                blade.parentBlade.refresh(blade.storeId, 'menus');
            },
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        };

        blade.selectNode = function (listItem, isNew) {
            $scope.selectedNodeId = listItem.id;

            var newBlade = {
                id: 'menuLinkListBlade',
                storeId: blade.storeId,
                parentRefresh: blade.refresh,
                controller: 'virtoCommerce.contentModule.menuLinkListController',
                template: 'Modules/$(VirtoCommerce.Content)/Scripts/blades/menu/menu-link-list.tpl.html'
            };

            if (isNew) {
                angular.extend(newBlade, {
                    isNew: true,
                    title: 'content.blades.menu-link-list.title-new',
                    subtitle: 'content.blades.menu-link-list.subtitle-new'
                });
            } else {
                angular.extend(newBlade, {
                    listId: listItem.id,
                    title: 'content.blades.menu-link-list.title',
                    titleValues: { name: listItem.name },
                    subtitle: 'content.blades.menu-link-list.subtitle'
                });
            }
            bladeNavigationService.showBlade(newBlade, blade);
        }

        blade.deleteList = function (selection) {
            bladeNavigationService.closeChildrenBlades(blade, function () {
                var dialog = {
                    id: "confirmDelete",
                    title: "content.dialogs.link-list-delete.title",
                    message: "content.dialogs.link-list-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;

                            var listEntryIds = _.pluck(selection, 'id');
                            menus.delete({ storeId: blade.storeId, listIds: listEntryIds }, function () {
                                blade.refresh();
                                $rootScope.$broadcast("cms-menus-changed", blade.storeId);
                            },
                            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            });
        };

        blade.headIcon = 'fa-list-ol';

        blade.toolbarCommands = [
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () { blade.selectNode({}, true); },
                canExecuteMethod: function () { return true; },
                permission: 'content:create'
            },
			{
			    name: "platform.commands.delete", icon: 'fa fa-trash-o',
			    executeMethod: function () { blade.deleteList($scope.gridApi.selection.getSelectedRows()); },
			    canExecuteMethod: function () {
			        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
			    },
			    permission: 'content:delete'
			}
        ];

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions);
        };

        blade.refresh();
    }]);
