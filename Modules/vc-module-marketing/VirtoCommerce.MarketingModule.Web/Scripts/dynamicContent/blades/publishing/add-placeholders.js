angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.addPublishingPlaceholdersStepController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.contentPlaces', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', function ($scope, contentPlacesApi, bladeNavigationService, bladeUtils, uiGridHelper) {
    var blade = $scope.blade;
    blade.chosenFolder = 'ContentPlace';
    blade.currentEntity = {};

    blade.refresh = function () {
        blade.isLoading = true;
        contentPlacesApi.search({
            folderId: blade.chosenFolder,
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount,
            sort: uiGridHelper.getSortExpression($scope),
            responseGroup: '20'
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

    _.each(blade.publication.contentPlaces, function (el) {
        contentPlacesApi.get({ id: el.id }, function (data) {
            var orEl = _.find(blade.parentBlade.origEntity.contentPlaces, function (contentPlace) { return contentPlace.id === el.id });
            if (!angular.isUndefined(orEl)) {
                orEl.path = data.path;
                orEl.outline = data.outline;
            }
            el.path = data.path;
            el.outline = data.outline;
        });
    });

    blade.deleteAllPlaceholder = function () {
        blade.publication.contentPlaces = [];
        $scope.gridApi.grid.queueGridRefresh();
    };

    blade.deletePlaceholder = function (data) {
        blade.publication.contentPlaces = _.filter(blade.publication.contentPlaces, function (place) { return !angular.equals(data.id, place.id); });
        $scope.gridApi.grid.queueGridRefresh();
    };

    blade.selectNode = function (node) {
        if (node.isFolder) {
            browseFolder(node);
        } else {
            blade.publication.contentPlaces.push(node);
            $scope.gridApi.grid.queueGridRefresh();
        }
    };

    function browseFolder(node) {
        if (!blade.chosenFolder || !angular.equals(blade.chosenFolder, node.id)) {
            blade.chosenFolder = node.id;
            blade.currentEntity = node;
            blade.refresh();
        } else {
            blade.chosenFolder = node.parentFolderId;
            blade.currentEntity = undefined;
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
            blade.breadcrumbs = [(generateBreadcrumb({ id: 'ContentPlace', name: 'Placeholders' }))];
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
                    row.visible = row.entity.isFolder || _.all(blade.publication.contentPlaces, function (ci) { return ci.id !== row.entity.id; });
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