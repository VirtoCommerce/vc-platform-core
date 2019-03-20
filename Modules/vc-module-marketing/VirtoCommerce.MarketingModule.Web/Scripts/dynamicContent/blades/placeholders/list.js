angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.placeholdersDynamicContentListController', ['$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService', 'virtoCommerce.marketingModule.dynamicContent.folders', 'virtoCommerce.marketingModule.dynamicContent.contentPlaces',
function ($scope, bladeUtils, uiGridHelper, dialogService, dynamicContentFoldersApi, contentPlacesApi) {
    var bladeNavigationService = bladeUtils.bladeNavigationService;
    var blade = $scope.blade;
    blade.headIcon = 'fa-location-arrow';
    blade.chosenFolderId = 'ContentPlace';
    blade.currentEntity = {};

    blade.initialize = function () {
        blade.refresh();
    };

    blade.addNew = function () {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            $scope.selectedNodeId = null;

            var newBlade = {
                id: 'listItemChild',
                title: 'marketing.blades.placeholders.add.title',
                subtitle: 'marketing.blades.placeholders.add.subtitle',
                chosenFolder: blade.chosenFolderId,
                addNewFolder: addNewFolder,
                addNewPlaceholder: addNewPlaceholder,
                controller: 'virtoCommerce.marketingModule.addPlaceholderElementController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/add.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    };

    function addNewFolder(data) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var newBlade = {
                id: 'listItemChild',
                title: 'marketing.blades.placeholders.folder-details.title-new',
                subtitle: 'marketing.blades.placeholders.folder-details.subtitle-new',
                entity: data,
                isNew: true,
                controller: 'virtoCommerce.marketingModule.addFolderPlaceholderController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/folder-details.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    }

    function addNewPlaceholder(data) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var newBlade = {
                id: 'listItemChild',
                title: 'marketing.blades.placeholders.placeholder-details.title-new',
                subtitle: 'marketing.blades.placeholders.placeholder-details.subtitle-new',
                entity: data,
                isNew: true,
                controller: 'virtoCommerce.marketingModule.placeholderDetailController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/placeholder-details.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    }

    blade.refresh = function () {
        blade.isLoading = true;
        contentPlacesApi.search({
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount,
            keyword: blade.searchKeyword,
            folderId: blade.chosenFolderId,
            sort: uiGridHelper.getSortExpression($scope),
            responseGroup: '20'
        }, function (data) {
            _.each(data.results, function (entry) {
                entry.isFolder = entry.objectType === 'DynamicContentFolder';
            });

            $scope.listEntries = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            setBreadcrumbs();
            blade.isLoading = false;
        });
    };

    blade.toolbarCommands = [{
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
            blade.breadcrumbs = [generateBreadcrumb({ id: 'ContentPlace', name: 'all', isFolder: true })];
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
            newBlade.title = 'marketing.blades.placeholders.folder-details.title-new';
            newBlade.subtitle = 'marketing.blades.placeholders.folder-details.subtitle-new';
            newBlade.controller = 'virtoCommerce.marketingModule.addFolderPlaceholderController';
            newBlade.template = 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/folder-details.tpl.html';
        } else {
            newBlade.title = 'marketing.blades.placeholders.placeholder-details.title';
            newBlade.subtitle = 'marketing.blades.placeholders.placeholder-details.subtitle';
            newBlade.controller = 'virtoCommerce.marketingModule.placeholderDetailController';
            newBlade.template = 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/placeholders/placeholder-details.tpl.html';
        }
        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.deleteItems = function (items) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var dialog = {
                id: "confirmDeleteContentPlaceholdersFolder",
                title: "marketing.dialogs.placeholders-folder-delete.title",
                message: "marketing.dialogs.placeholders-folder-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        var folderItems = _.filter(items, function (i) { return i.isFolder; });
                        if (folderItems.length) {
                            dynamicContentFoldersApi.delete({
                                ids: _.pluck(folderItems, 'id')
                            }, blade.refresh);
                        }
                        var placeholderItems = _.filter(items, function (i) { return !i.isFolder; });
                        if (placeholderItems.length) {
                            contentPlacesApi.delete({
                                ids: _.pluck(placeholderItems, 'id')
                            }, blade.refresh);
                        }
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        });
    };

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