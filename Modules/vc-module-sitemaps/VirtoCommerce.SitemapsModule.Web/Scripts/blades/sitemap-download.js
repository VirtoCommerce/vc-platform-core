angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.sitemapDownloadController', ['$scope', 'virtoCommerce.sitemapsModule.sitemapApi', 'platformWebApp.bladeNavigationService', function ($scope, sitemapApi, bladeNavigationService) {
    var blade = $scope.blade;
    blade.isLoading = false;

    $scope.$on("new-notification-event", function (event, notification) {
        if (blade.notification && notification.id == blade.notification.id) {
            angular.copy(notification, blade.notification);
        }
    });

    sitemapApi.download({
        storeId: blade.storeId,
        baseUrl: blade.baseUrl
    },
    function (response) {
        blade.notification = response;
    }, function (error) {
        bladeNavigationService.setError('Error ' + error.status, $scope.blade);
    });
}]);