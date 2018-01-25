angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.taskListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.thumbnail.api', 'platformWebApp.uiGridHelper',
        function ($scope, bladeNavigationService, bladeUtils, thumbnailApi, uiGridHelper) {
            debugger;

            $scope.uiGridConstants = uiGridHelper.uiGridConstants;
            var blade = $scope.blade;

            blade.refresh = function() {
                debugger;
                blade.isLoading = true;

                thumbnailApi.getTaskList().then(function (results) {
                    debugger;
                    blade.isLoading = false;
                    blade.currentEntities = results;
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
                }
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {
                debugger;

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
