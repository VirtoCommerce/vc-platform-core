angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.sitemapListController', ['$window', '$scope', '$modal', 'platformWebApp.bladeUtils', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'virtoCommerce.sitemapsModule.sitemapApi', function ($window, $scope, $modal, bladeUtils, bladeNavigationService, dialogService, uiGridHelper, sitemapApi) {
    var blade = $scope.blade;

    blade.refresh = function () {
        blade.isLoading = true;
        sitemapApi.search({
            storeId: blade.store.id,
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        }, function (response) {
            $scope.pageSettings.totalItems = response.totalCount;
            $scope.listEntries = response.results;
            blade.isLoading = false;
        });
    };

    blade.selectNode = function (node, isNew) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            id: 'sitemap-detail',
            isNew: isNew,
            currentEntity: node,
            parentRefresh: blade.refresh,
            parentSelectNode: blade.selectNode,
            controller: 'virtoCommerce.sitemapsModule.sitemapDetailController',
            template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/sitemap-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    blade.headIcon = 'fa fa-sitemap';
    blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () { return true; }
            },
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () {
                    blade.selectNode({
                        location: 'sitemap/',
                        urlTemplate: '{slug}',
                        storeId: blade.store.id,
                        items: []
                    }, true);
                },
                canExecuteMethod: function () { return true; },
                permission: 'sitemaps:create'
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () {
                    deleteList($scope.gridApi.selection.getSelectedRows());
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
                permission: 'sitemaps:delete'
            },
            {
                name: 'sitemapsModule.blades.sitemapList.toolbar.download', icon: 'fa fa-download',
                executeMethod: showDownloadDialog,
                canExecuteMethod: function () {
                    return !blade.isLoading && $scope.pageSettings.totalItems > 0;
                }
            }];

    function deleteList(selection) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var dialog = {
                id: 'confirmDeleteSitemaps',
                title: 'sitemapsModule.dialogs.sitemap-delete.title',
                message: 'sitemapsModule.dialogs.sitemap-delete.message',
                callback: function (confirm) {
                    if (confirm) {
                        blade.isLoading = true;
                        var sitemapIds = _.pluck(selection, 'id');
                        sitemapApi.remove({ ids: sitemapIds }, blade.refresh);
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        });
    }

    function showDownloadDialog() {
        var baseUrl = blade.store.url || blade.store.secureUrl
        var confirmDialog = {
            id: 'confirmBaseUrl',
            originalBaseUrl: angular.copy(baseUrl),
            baseUrl: baseUrl,
            templateUrl: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/dialogs/confirm-base-url-dialog.tpl.html',
            controller: 'virtoCommerce.sitemapsModule.baseUrlDialogController',
            resolve: {
                dialog: function () {
                    return confirmDialog;
                }
            }
        };
        var dialogInstance = $modal.open(confirmDialog);
        dialogInstance.result.then(function (baseUrl) {
            if (baseUrl) {
                var newBlade = {
                    id: 'sitemap-download',
                    title: 'sitemapsModule.blades.download.title',
                    headIcon: 'fa fa-download',
                    controller: 'virtoCommerce.sitemapsModule.sitemapDownloadController',
                    template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/sitemap-download.tpl.html',
                    storeId: blade.store.id,
                    baseUrl: baseUrl
                };
                bladeNavigationService.showBlade(newBlade, blade);

                //$window.open('api/sitemaps/download?storeId=' + blade.store.id + '&baseUrl=' + baseUrl, '_blank');
            }
        });
    }

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
            uiGridHelper.bindRefreshOnSortChanged($scope);
        });
        bladeUtils.initializePagination($scope);
    };

    //No need to call this because page 'pageSettings.currentPage' is watched!!! It would trigger subsequent duplicated req...
    //blade.refresh();
}]);