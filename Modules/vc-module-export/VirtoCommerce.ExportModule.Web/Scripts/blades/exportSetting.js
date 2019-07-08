angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exporterController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, bladeNavigationService, exportApi) {
        var blade = $scope.blade;
        blade.canStartProcess = false;
        blade.isLoading = true;
        
        function initializeBlade() {
            exportApi.getProviders(function (result) {
                if (result && result.length) {
                    $scope.providers = _.map(result, function (item) { return { id: item.typeName, name: item.typeName } });
                    $scope.selectedProvider = $scope.providers[0];
                }
            });
            blade.isLoading = false;
        }

        $scope.startExport = function () {
            blade.exportDataRequest.providerName = $scope.selectedProvider.name;
                    var progressBlade = {
                        id: 'exportProgress',
                        title: 'Exporting...',
                        controller: 'virtoCommerce.exportModule.exportProcessController',
                        template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/exportProgress.tpl.html',
                        exportDataRequest: blade.exportDataRequest,
                        isClosingDisabled: false
                    };
            bladeNavigationService.showBlade(progressBlade);
        };

        $scope.blade.headIcon = 'fa-upload';
        initializeBlade();
    }]);
