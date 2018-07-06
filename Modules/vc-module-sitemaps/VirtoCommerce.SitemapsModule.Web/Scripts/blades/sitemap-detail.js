angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.sitemapDetailController', ['$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeNavigationService', 'virtoCommerce.sitemapsModule.sitemapApi', 'platformWebApp.dialogService', function ($scope, bladeUtils, uiGridHelper, bladeNavigationService, sitemapApi, dialogService) {
    var blade = $scope.blade;
    blade.updatePermission = 'sitemaps:update';

    blade.refresh = function () {
        blade.isLoading = true;
        sitemapApi.get({ id: blade.currentEntity.id }, function (data) {
            initializeBlade(data);
            blade.title = blade.currentEntity.location;
        });
    };

    function initializeBlade(data) {
        blade.currentEntity = angular.copy(data);
        blade.origEntity = data;
        blade.isLoading = false;
    }

    blade.refreshSitemapItems = function () {
        sitemapApi.searchSitemapItems({
            sitemapId: blade.currentEntity.id,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        },
        function (response) {
            $scope.pageSettings.totalItems = response.totalCount;
            blade.currentEntities = response.results;
            blade.isLoading = false;
        });
    };

    function refreshSitemapItemsAndParent() {
        blade.refreshSitemapItems();
        refreshParent();
    }

    function refreshParent() {
        if (blade.parentRefresh) {
            blade.parentRefresh();
        }
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "sitemapsModule.dialogs.discardChanges.title", "sitemapsModule.dialogs.discardChanges.message");
    };

    $scope.setForm = function (form) { $scope.formScope = form; };

    function isDirty() {
        return !angular.equals(blade.origEntity, blade.currentEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope && $scope.formScope.$valid;
    }

    $scope.hasStaticContentItems = function () {
        var staticContentItem = _.find(blade.currentEntities, function (e) { return e.objectType.toLowerCase() === 'contentitem' || e.objectType.toLowerCase() === 'folder' });
        return staticContentItem != null && staticContentItem != undefined;
    }

    $scope.saveChanges = function () {
        $scope.errorMessage = null;

        if (blade.currentEntity.location.toLowerCase() === 'sitemap.xml') {
            $scope.errorMessage = 'sitemapsModule.blades.sitemap.formSitemap.sitemapIndexErrorMessage';
        } else {
            blade.isLoading = true;

            sitemapApi.search({
                storeId: blade.currentEntity.storeId,
                location: blade.currentEntity.location,
            }, function (response) {
                if (_.any(response.results, function (s) { return blade.inNew || s.id !== blade.currentEntity.id })) {
                    $scope.errorMessage = 'sitemapsModule.blades.sitemap.formSitemap.sitemapSitemapLocationExistErrorMessage';
                    blade.isLoading = false;
                } else {
                    if (blade.isNew) {
                        sitemapApi.addSitemap(blade.currentEntity, function (result) {
                            angular.copy(blade.currentEntity, blade.origEntity);
                            refreshParent();
                            blade.parentSelectNode(result);
                        });
                    } else {
                        sitemapApi.updateSitemap(blade.currentEntity, function () {
                            blade.refresh();
                            refreshParent();
                        });
                    }
                }
            });
        }
    }

    if (blade.isNew) {
        blade.title = 'sitemapsModule.blades.sitemap.newSitemapTitle';
        initializeBlade(blade.currentEntity);
    } else {
        blade.title = blade.currentEntity.location;
        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: $scope.saveChanges,
                canExecuteMethod: canSave
            },
            {
                name: "platform.commands.reset", icon: 'fa fa-undo',
                executeMethod: function () {
                    angular.copy(blade.origEntity, blade.currentEntity);
                },
                canExecuteMethod: isDirty,
                permission: blade.updatePermission
            },
            {
                name: 'sitemapsModule.blades.sitemap.toolbar.addItems', icon: 'fa fa-plus',
                executeMethod: showAddItemsBlade,
                canExecuteMethod: function () { return true; },
                permission: blade.updatePermission
            },
            {
                name: 'sitemapsModule.blades.sitemap.toolbar.removeItems', icon: 'fa fa-trash-o',
                executeMethod: function () {
                    blade.removeItems($scope.gridApi.selection.getSelectedRows());
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
                permission: blade.updatePermission
            }
        ];

        function showAddItemsBlade() {
            var newBlade = {
                id: 'addSitemapItemsBlade',
                sitemap: blade.currentEntity,
                storeId: blade.currentEntity.storeId,
                parentRefresh: refreshSitemapItemsAndParent,
                title: 'sitemapsModule.blades.addItems.title',
                controller: 'virtoCommerce.sitemapsModule.sitemapItemsAddController',
                template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/sitemap-items-add.tpl.html'
            }
            bladeNavigationService.showBlade(newBlade, blade);
        }

        blade.removeItems = function (list) {
            var dialog = {
                id: "confirmDelete",
                title: "sitemapsModule.dialogs.sitemapItems-delete.title",
                message: "sitemapsModule.dialogs.sitemapItems-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        var ids = _.pluck(list, 'id');
                        sitemapApi.removeSitemapItems({ itemIds: ids }, refreshSitemapItemsAndParent);
                    }
                }
            }
            dialogService.showConfirmationDialog(dialog);
        };

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            $scope.gridOptions = gridOptions;

            gridOptions.onRegisterApi = function (gridApi) {
                gridApi.core.on.sortChanged($scope, function () {
                    if (!blade.isLoading) blade.refreshSitemapItems();
                });
            };

            bladeUtils.initializePagination($scope, true);
            $scope.$watch('pageSettings.currentPage', function () { blade.refreshSitemapItems(); });
            blade.refresh();
        };
    }

}]);