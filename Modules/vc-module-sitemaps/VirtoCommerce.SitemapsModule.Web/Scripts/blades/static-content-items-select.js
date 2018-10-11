angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.staticContentItemSelectController', ['$scope', '$timeout', 'virtoCommerce.contentModule.contentApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper',
function ($scope, $timeout, staticContent, bladeNavigationService, bladeUtils, uiGridHelper) {
    var blade = $scope.blade;

    blade.selectedItems = [];

    blade.toolbarCommands = [{
        name: 'sitemapsModule.blades.addStaticContentItems.toolbar.addSelected',
        icon: 'fa fa-plus',
        canExecuteMethod: function (staticContentBlade) {
            return _.any(staticContentBlade.selectedItems);
        },
        executeMethod: function (staticContentBlade) {
            var sitemapItems = _.map(staticContentBlade.selectedItems, itemToSitemapItem);
            staticContentBlade.confirmChangesFn(sitemapItems, staticContentBlade);
        }
    }];

    blade.refresh = function () {
        blade.isLoading = true;
        staticContent.query({
            contentType: 'pages',
            storeId: blade.storeId,
            keyword: blade.searchKeyword,
            folderUrl: blade.currentEntity.url
        }, function (data) {
            $scope.pageSettings.totalItems = data.length;
            _.each(data, function (x) {
                x.id = x.url;
                x.isImage = x.mimeType && x.mimeType.startsWith('image/');
                x.isOpenable = x.mimeType && (x.mimeType.startsWith('application/j') || x.mimeType.startsWith('text/'));
            });
            $scope.listEntries = data;
            blade.isLoading = false;
            setBreadcrumbs();
        }, function (error) {
            bladeNavigationService.setError('Error ' + error.status, blade);
            blade.isLoading = false;
        });
    };

    blade.setSelectedNode = function (listItem) {
        $scope.selectedNodeId = listItem.id;
    };

    $scope.uiGridConstants = uiGridHelper.uiGridConstants;

    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions, externalRegisterApiCallback);
    };

    $scope.selectNode = function (listItem) {
        if ($scope.selectedNodeId === listItem.id) {
            return;
        }
        blade.setSelectedNode(listItem);
        if ($scope.options.selectItemFn) {
            $scope.options.selectItemFn(listItem);
        }
        if (listItem.type.toLowerCase() === 'folder') {
            var newBlade = {
                confirmChangesFn: blade.confirmChangesFn,
                id: blade.id,
                contentType: blade.contentType,
                storeId: blade.storeId,
                languages: blade.languages,
                currentEntity: listItem,
                breadcrumbs: blade.breadcrumbs,
                title: blade.title,
                subtitle: blade.subtitle,
                controller: blade.controller,
                template: blade.template,
                disableOpenAnimation: true,
                isClosingDisabled: blade.isClosingDisabled
            };
            bladeNavigationService.showBlade(newBlade, blade.parentBlade);
        }
    };

    $scope.options = angular.extend({
        showCheckingMultiple: true,
        allowCheckingItem: true,
        selectedItemIds: []
    }, blade.options);

    bladeUtils.initializePagination($scope, true);

    blade.refresh();

    function setBreadcrumbs() {
        if (blade.breadcrumbs) {
            var breadcrumbs = blade.breadcrumbs.slice(0);
            if (_.all(breadcrumbs, function (x) { return x.id !== blade.currentEntity.id; })) {
                var breadCrumb = generateBreadcrumb(blade.currentEntity.id, blade.currentEntity.name);
                breadcrumbs.push(breadCrumb);
            }
            blade.breadcrumbs = breadcrumbs;
        } else {
            blade.breadcrumbs = [generateBreadcrumb(null, 'all')];
        }
    }

    function generateBreadcrumb(id, name) {
        return {
            id: id,
            name: name,
            blade: blade,
            navigate: function (breadcrumb) {
                breadcrumb.blade.disableOpenAnimation = true;
                bladeNavigationService.showBlade(breadcrumb.blade);
                breadcrumb.blade.refresh();
            }
        };
    }

    function externalRegisterApiCallback(gridApi) {
        $scope.$watch('pageSettings.currentPage', gridApi.pagination.seek);
        gridApi.grid.registerDataChangeCallback(function (grid) {
            $timeout(function () {
                _.each($scope.items, function (x) {
                    if (_.some($scope.options.selectedItemIds, function (y) { return y === x.id; })) {
                        gridApi.selection.selectRow(x);
                    }
                });
            });
        }, [$scope.uiGridConstants.dataChange.ROW]);

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            if ($scope.options.checkItemFn) {
                $scope.options.checkItemFn(row.entity, row.isSelected);
            }
            if (row.isSelected) {
                if (!_.contains($scope.options.selectedItemIds, row.entity.id)) {
                    $scope.options.selectedItemIds.push(row.entity.id);
                    blade.selectedItems.push(row.entity);
                }
            }
            else {
                $scope.options.selectedItemIds = _.without($scope.options.selectedItemIds, row.entity.id);
                blade.selectedItems = _.filter(blade.selectedItems, function (x) { return x.id !== row.entity.id; });
            }
        });
    }

    function itemToSitemapItem(item) {
        return {
            title: item.relativeUrl,
            urlTemplate: item.relativeUrl,
            objectType: item.type.toLowerCase() !== 'folder' ? 'ContentItem' : item.type
        };
    }




    //$scope.uiGridConstants = uiGridHelper.uiGridConstants;
    //var bladeNavigationService = bladeUtils.bladeNavigationService;
    //var blade = $scope.blade;
    //blade.selectedItems = [];
    //blade.toolbarCommands = [{
    //    name: 'sitemapsModule.blades.addStaticContentItems.toolbar.addSelected',
    //    icon: 'fa fa-plus',
    //    canExecuteMethod: function (staticContentBlade) {
    //        return _.any(staticContentBlade.selectedItems);
    //    },
    //    executeMethod: function (staticContentBlade) {
    //        var sitemapItems = _.map(staticContentBlade.selectedItems, itemToSitemapItem);
    //        staticContentBlade.confirmChangesFn(sitemapItems, staticContentBlade);
    //    }
    //}];

    //blade.setSelectedNode = function (listItem) {
    //    $scope.selectedNodeId = listItem.id;
    //};

    //blade.refresh = function () {
    //    blade.isLoading = true;
    //    staticContent.query({
    //        contentType: 'pages',
    //        storeId: blade.storeId,
    //        keyword: blade.searchKeyword,
    //        folderUrl: blade.currentEntity.url
    //    }, function (data) {
    //        $scope.pageSettings.totalItems = data.length;
    //        _.each(data, function (x) {
    //            x.id = x.url;
    //            x.isImage = x.mimeType && x.mimeType.startsWith('image/');
    //            x.isOpenable = x.mimeType && (x.mimeType.startsWith('application/j') || x.mimeType.startsWith('text/'));
    //        });
    //        $scope.listEntries = data;
    //        blade.isLoading = false;
    //        setBreadcrumbs();
    //    }, function (error) {
    //        bladeNavigationService.setError('Error ' + error.status, blade);
    //        blade.isLoading = false;
    //    });
    //};

    //$scope.options = angular.extend({
    //    showCheckingMultiple: true,
    //    allowCheckingItem: true,
    //    selectedItemIds: []
    //}, blade.options);

    //$scope.selectNode = function (listItem) {
    //    if ($scope.selectedNodeId === listItem.id) {
    //        return;
    //    }
    //    blade.setSelectedNode(listItem);
    //    if ($scope.options.selectItemFn) {
    //        $scope.options.selectItemFn(listItem);
    //    }
    //    if (listItem.type.toLowerCase() === 'folder') {
    //        var newBlade = {
    //            confirmChangesFn: blade.confirmChangesFn,
    //            id: blade.id,
    //            contentType: blade.contentType,
    //            storeId: blade.storeId,
    //            languages: blade.languages,
    //            currentEntity: listItem,
    //            breadcrumbs: blade.breadcrumbs,
    //            title: blade.title,
    //            subtitle: blade.subtitle,
    //            controller: blade.controller,
    //            template: blade.template,
    //            disableOpenAnimation: true,
    //            isClosingDisabled: blade.isClosingDisabled
    //        };
    //        bladeNavigationService.showBlade(newBlade, blade.parentBlade);
    //    }
    //};

    //var filter = $scope.filter = {};
    //filter.criteriaChanged = function () {
    //    if (filter.keyword === null) {
    //        blade.memberType = undefined;
    //    }
    //    if ($scope.pageSettings.currentPage > 1) {
    //        $scope.pageSettings.currentPage = 1;
    //    } else {
    //        blade.refresh();
    //    }
    //};

    //function setBreadcrumbs() {
    //    if (blade.breadcrumbs) {
    //        var breadcrumbs = blade.breadcrumbs.slice(0);
    //        if (_.all(breadcrumbs, function (x) { return x.id !== blade.currentEntity.id; })) {
    //            var breadCrumb = generateBreadcrumb(blade.currentEntity.id, blade.currentEntity.name);
    //            breadcrumbs.push(breadCrumb);
    //        }
    //        blade.breadcrumbs = breadcrumbs;
    //    } else {
    //        blade.breadcrumbs = [generateBreadcrumb(null, 'all')];
    //    }
    //}

    //function generateBreadcrumb(id, name) {
    //    return {
    //        id: id,
    //        name: name,
    //        blade: blade,
    //        navigate: function (breadcrumb) {
    //            breadcrumb.blade.disableOpenAnimation = true;
    //            bladeNavigationService.showBlade(breadcrumb.blade);
    //            breadcrumb.blade.refresh();
    //        }
    //    };
    //}

    //// ui-grid
    //$scope.setGridOptions = function (gridOptions) {
    //    uiGridHelper.initialize($scope, gridOptions, externalRegisterApiCallback);
    //    bladeUtils.initializePagination($scope, true);
    //};

    //function externalRegisterApiCallback(gridApi) {
    //    $scope.$watch('pageSettings.currentPage', gridApi.pagination.seek);
    //    gridApi.grid.registerDataChangeCallback(function (grid) {
    //        $timeout(function () {
    //            _.each($scope.items, function (x) {
    //                if (_.some($scope.options.selectedItemIds, function (y) { return y === x.id; })) {
    //                    gridApi.selection.selectRow(x);
    //                }
    //            });
    //        });
    //    }, [$scope.uiGridConstants.dataChange.ROW]);

    //    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
    //        if ($scope.options.checkItemFn) {
    //            $scope.options.checkItemFn(row.entity, row.isSelected);
    //        }
    //        if (row.isSelected) {
    //            if (!_.contains($scope.options.selectedItemIds, row.entity.id)) {
    //                $scope.options.selectedItemIds.push(row.entity.id);
    //                blade.selectedItems.push(row.entity);
    //            }
    //        }
    //        else {
    //            $scope.options.selectedItemIds = _.without($scope.options.selectedItemIds, row.entity.id);
    //            blade.selectedItems = _.filter(blade.selectedItems, function (x) { return x.id !== row.entity.id; });
    //        }
    //    });

    //    uiGridHelper.bindRefreshOnSortChanged($scope);
    //}

    //function itemToSitemapItem(item) {
    //    return {
    //        title: item.relativeUrl,
    //        urlTemplate: item.relativeUrl,
    //        objectType: item.type.toLowerCase() !== 'folder' ? 'ContentItem' : item.type
    //    };
    //}

    //blade.refresh();
}]);