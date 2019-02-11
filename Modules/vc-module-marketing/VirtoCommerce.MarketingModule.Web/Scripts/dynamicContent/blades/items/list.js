angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.itemsDynamicContentListController', ['$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService', 'virtoCommerce.marketingModule.dynamicContent.folders', 'virtoCommerce.marketingModule.dynamicContent.contentItems',
function ($scope, bladeUtils, uiGridHelper, dialogService, dynamicContentFoldersApi, dynamicContentItemsApi) {
    var bladeNavigationService = bladeUtils.bladeNavigationService;
    var blade = $scope.blade;
    blade.headIcon = 'fa-inbox';
    blade.chosenFolderId = 'ContentItem';
    blade.currentEntity = {};

    blade.initializeBlade = function () {
        blade.refresh();
    };

    blade.addNew = function () {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            $scope.selectedNodeId = null;

            var newBlade = {
                id: 'listItemChild',
                title: 'marketing.blades.items.add.title',
                subtitle: 'marketing.blades.items.add.subtitle',
                chosenFolder: blade.chosenFolderId,
                addNewContentItem: addNewContentItem,
                addNewFolder: addNewFolder,
                controller: 'virtoCommerce.marketingModule.addContentItemsElementController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/items/add.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    };

    function addNewContentItem(data) {
        var newBlade = {
            id: 'listItemChild',
            title: 'marketing.blades.items.content-item-details.title-new',
            subtitle: 'marketing.blades.items.content-item-details.subtitle-new',
            entity: data,
            isNew: true,
            controller: 'virtoCommerce.marketingModule.itemDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/items/item-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    function addNewFolder(data) {
        var newBlade = {
            id: 'listItemChild',
            title: 'marketing.blades.items.folder-details.title-new',
            subtitle: 'marketing.blades.items.folder-details.subtitle-new',
            entity: data,
            isNew: true,
            controller: 'virtoCommerce.marketingModule.itemFolderDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/items/itemFolder-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    blade.refresh = function () {
        blade.isLoading = true;
        dynamicContentItemsApi.search({
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount,
            keyword: blade.searchKeyword,
            folderId: blade.chosenFolderId,
            sort: uiGridHelper.getSortExpression($scope),
            responseGroup: '18'
        }, function (data) {
            _.each(data.results, function (entry) {
                entry.isFolder = entry.objectType === 'DynamicContentFolder';
            });

            $scope.listEntries = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            setBreadcrumbs();
            blade.isLoading = false;
        });
    }

    blade.toolbarCommands = [
        {
            name: 'platform.commands.refresh', icon: 'fa fa-refresh',
            executeMethod: blade.refresh,
            canExecuteMethod: function () { return true; }
        }, {
            name: 'platform.commands.add', icon: 'fa fa-plus',
            executeMethod: blade.addNew,
            canExecuteMethod: function () { return true; }
        }, {
            name: 'platform.commands.delete', icon: 'fa fa-trash',
            executeMethod: function () {
                $scope.deleteItems($scope.gridApi.selection.getSelectedRows());
            },
            canExecuteMethod: function () {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            }
        }];

    function setBreadcrumbs() {
        if (blade.breadcrumbs) {
            var breadcrumb = _.find(blade.breadcrumbs, function (b) { return b.id === blade.currentEntity.id; });
            if (!breadcrumb) {
                breadCrumb = generateBreadcrumb(blade.currentEntity);
                blade.breadcrumbs.push(breadCrumb);
            } else {
                var position = blade.breadcrumbs.indexOf(breadcrumb);
                blade.breadcrumbs = blade.breadcrumbs.slice(0, position + 1);
            }
        } else {
            blade.breadcrumbs = [generateBreadcrumb({ id: 'ContentItem', name: 'all', isFolder: true })];
        }
    }

    function generateBreadcrumb(node) {
        return {
            id: node.id,
            name: node.name,
            navigate: function () {
                $scope.selectNode(node);
            }
        };
    }

    var filter = $scope.filter = {};
    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    $scope.clearKeyword = function () {
        blade.searchKeyword = null;
        blade.refresh();
    };

    $scope.selectNode = function (node) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            if (node.id && node.isFolder) {
                blade.currentEntity = node;
                blade.chosenFolderId = node.id;
                blade.refresh();
            } else {
                $scope.openDetailsBlade(node);
            }
        });
    };

    $scope.openDetailsBlade = function (node) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            id: 'listItemChild',
            entity: node,
            isNew: false
        };
        if (node.isFolder) {
            newBlade.title = 'marketing.blades.items.folder-details.title';
            newBlade.subtitle = 'marketing.blades.items.folder-details.subtitle';
            newBlade.controller = 'virtoCommerce.marketingModule.itemFolderDetailController';
            newBlade.template = 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/items/itemFolder-detail.tpl.html';
        } else {
            newBlade.title = 'marketing.blades.items.content-item-details.title';
            newBlade.subtitle = 'marketing.blades.items.content-item-details.subtitle';
            newBlade.controller = 'virtoCommerce.marketingModule.itemDetailController';
            newBlade.template = 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/items/item-detail.tpl.html';
        }
        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.deleteItems = function (items) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var dialog = {
                id: "confirmDeleteContentItemsFolder",
                title: "marketing.dialogs.content-item-folder-delete.title",
                message: "marketing.dialogs.content-item-folder-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        var folderItems = _.filter(items, function (i) { return i.isFolder; });
                        if (folderItems.length) {
                            dynamicContentFoldersApi.delete({
                                ids: _.pluck(folderItems, 'id')
                            }, blade.refresh);
                        }
                        var contentItems = _.filter(items, function (i) { return !i.isFolder; });
                        if (contentItems.length) {
                            dynamicContentItemsApi.delete({
                                ids: _.pluck(contentItems, 'id')
                            }, blade.refresh);
                        }
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        });
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
}]);