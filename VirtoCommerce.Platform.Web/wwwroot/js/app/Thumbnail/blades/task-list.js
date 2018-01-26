angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.taskListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.thumbnail.api', 'platformWebApp.uiGridHelper',
        function ($scope, bladeNavigationService, bladeUtils, thumbnailApi, uiGridHelper) {
            var blade = $scope.blade;

            $scope.uiGridConstants = uiGridHelper.uiGridConstants;
            $scope.hasMore = true;
            $scope.items = [];

            blade.refresh = function () {
                blade.isLoading = true;

                thumbnailApi.getTaskList().then(function (results) {

                    blade.isLoading = false;
                    $scope.items = results;
                    $scope.hasMore = results.length === $scope.pageSettings.itemsPerPageCount;
                });
            };

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh",
                    icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.add",
                    icon: 'fa fa-plus',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.run",
                    icon: 'fa fa-exclamation',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.delete",
                    icon: 'fa fa-trash-o',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                }
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {

                //disable watched
                bladeUtils.initializePagination($scope, true);
                //сhoose the optimal amount that ensures the appearance of the scroll
                $scope.pageSettings.itemsPerPageCount = 50;

                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    //update gridApi for current grid
                    $scope.gridApi = gridApi;

                    uiGridHelper.bindRefreshOnSortChanged($scope);
                    $scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, showMore);
                });

                blade.refresh();
            };
        }]);
