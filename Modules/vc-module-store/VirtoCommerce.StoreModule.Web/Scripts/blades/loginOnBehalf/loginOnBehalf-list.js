angular.module('virtoCommerce.storeModule')
    .controller('virtoCommerce.storeModule.loginOnBehalfListController', ['$scope', '$window', '$modal', 'virtoCommerce.storeModule.stores', 'platformWebApp.bladeNavigationService', function ($scope, $window, $modal, stores, bladeNavigationService) {
    var blade = $scope.blade;
    $scope.selectedNodeId = null;

    blade.refresh = function () {
        stores.queryLoginOnBehalfStores({ userId: blade.currentEntityId }, function (data) {
            blade.isLoading = false;
            blade.currentEntities = data;
        },
        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    }

    $scope.selectNode = function (store) {
        $scope.selectedNodeId = store.id;

        if (!store.secureUrl || !store.defaultLanguage) {
            showEnterUrlDialog(store.secureUrl);
        } else {
            openUrlOnBehalf(store.secureUrl);
        }
    }

    function openUrlOnBehalf(secureUrl) {
        // {store_secure_url}/account/login?UserId={customer_id}
        var url = secureUrl + '/account/impersonate/' + blade.currentEntityId;
        $window.open(url, '_blank');
    }

    function showEnterUrlDialog(secureUrl) {
        var confirmDialog = {
            id: 'enterStoreUrl',
            secureUrl: secureUrl,
            templateUrl: 'Modules/$(VirtoCommerce.Store)/Scripts/dialogs/enter-store-url-dialog.tpl.html',
            controller: 'virtoCommerce.storeModule.enterStoreUrlDialogController',
            resolve: {
                dialog: function () {
                    return confirmDialog;
                }
            }
        };
        var dialogInstance = $modal.open(confirmDialog);
        dialogInstance.result.then(function (enteredUrl) {
            if (enteredUrl) {
                openUrlOnBehalf(enteredUrl);
            }
        });
    }

    blade.headIcon = 'fa-key';

    blade.refresh();
}]);
