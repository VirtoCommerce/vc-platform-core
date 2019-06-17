angular.module('platformWebApp')
.controller('platformWebApp.changeLog.operationListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.changeLogApi', function ($scope, bladeNavigationService, changeLogApi) {
    $scope.blade.isLoading = false;

    if (!$scope.blade.currentEntities) {
        $scope.blade.isLoading = true;
        changeLogApi.search({
            objectIds: [ $scope.blade.tenantId ],
            objectType: $scope.blade.tenantType.split('.').pop(),
            take: 50
        }, function (data) {
            $scope.blade.isLoading = false;
            $scope.blade.currentEntities = data.results;
        });
    }

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;
    };
}]);
