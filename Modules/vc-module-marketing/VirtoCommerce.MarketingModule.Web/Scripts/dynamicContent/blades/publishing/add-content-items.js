angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.addPublishingContentItemsStepController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.contentItems', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', function ($scope, dynamicContentItemsApi, bladeNavigationService, bladeUtils, uiGridHelper) {
    var blade = $scope.blade;
    blade.chosenFolder = 'ContentItem';
    blade.currentEntity = {};

    blade.refresh = function () {
        blade.isLoading = true;
        dynamicContentItemsApi.search({
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount,
            folderId: blade.chosenFolder,
            sort: uiGridHelper.getSortExpression($scope),
            responseGroup: '18'
        }, function (data) {
            _.each(data.results, function (entry) {
                entry.isFolder = entry.objectType === 'DynamicContentFolder';
            });

            blade.currentEntity.listEntries = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            setBreadcrumbs();
            blade.isLoading = false;
        });
    };

    blade.publication.contentItems.forEach(function (el) {
        dynamicContentItemsApi.get({ id: el.id }, function (data) {
            var orEl = _.find(blade.parentBlade.origEntity.contentItems, function (contentItem) { return contentItem.id === el.id });
            if (!angular.isUndefined(orEl)) {
                orEl.path = data.path;
                orEl.outline = data.outline;
                orEl.dynamicProperties = data.dynamicProperties;
                orEl.objectType = data.objectType;
            }
            el.path = data.path;
            el.outline = data.outline;
            el.dynamicProperties = data.dynamicProperties;
            el.objectType = data.objectType;
        });
    });

    blade.deleteAllContentItems = function () {
        blade.publication.contentItems = [];
        $scope.gridApi.grid.queueGridRefresh();
    };

    blade.deleteContentItem = function (data) {
        blade.publication.contentItems = _.filter(blade.publication.contentItems, function (place) { return !angular.equals(data.id, place.id); });
        $scope.gridApi.grid.queueGridRefresh();
    };

    blade.selectNode = function (node) {
        if (node.isFolder) {
            browseFolder(node);
        } else {
            blade.publication.contentItems.push(node);
            $scope.gridApi.grid.queueGridRefresh();
        }
    };

    function browseFolder(node) {
        if (!blade.chosenFolder || !angular.equals(blade.chosenFolder, node.id)) {
            blade.chosenFolder = node.id;
            blade.currentEntity = node;
            blade.refresh();
        }
    }

    function setBreadcrumbs() {
        if (blade.breadcrumbs) {
            var breadcrumbs;
            var index = _.findLastIndex(blade.breadcrumbs, { id: blade.chosenFolder });
            if (index > -1) {
                //Clone array (angular.copy leaves the same reference)
                breadcrumbs = blade.breadcrumbs.slice(0, index + 1);
            }
            else {
                breadcrumbs = blade.breadcrumbs.slice(0);
                breadcrumbs.push(generateBreadcrumb(blade.currentEntity));
            }
            blade.breadcrumbs = breadcrumbs;
        } else {
            blade.breadcrumbs = [(generateBreadcrumb({ id: 'ContentItem', name: 'Items' }))];
        }
    }

    function generateBreadcrumb(node) {
        return {
            id: node.id,
            name: node.name,
            navigate: function () { browseFolder(node); }
        };
    }

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;

        gridOptions.onRegisterApi = function (gridApi) {
            gridApi.grid.registerRowsProcessor(function (renderableRows) {
                var visibleCount = 0;
                renderableRows.forEach(function (row) {
                    row.visible = row.entity.isFolder || _.all(blade.publication.contentItems, function (ci) { return ci.id !== row.entity.id; });
                    if (row.visible) visibleCount++;
                });

                $scope.filteredEntitiesCount = visibleCount;
                return renderableRows;
            }, 90);

            gridApi.core.on.sortChanged($scope, function () {
                if (!blade.isLoading) blade.refresh();
            });
        };

        bladeUtils.initializePagination($scope);
    };
}]);