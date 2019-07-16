angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportSettingsController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, bladeNavigationService, exportApi) {
        var blade = $scope.blade;
        blade.canStartProcess = false;
        blade.isLoading = true;
        blade.exportDataRequest = {};
        blade.isExportedTypeSelected = false;
        
        function initializeBlade() {
            exportApi.getProviders(function (result) {
                if (result && result.length) {
                    blade.allProviders = result;
                    fillProviders();
                }
            });
            blade.isLoading = false;
        }

        function fillProviders() {
            if (blade.allProviders) {
                var filterNonTabular = blade.exportDataRequest.exportTypeName && !blade.exportDataRequest.isTabularExportSupported;
                var providers = blade.allProviders; 
                if (filterNonTabular) {
                    providers = _.filter(providers, function (item) { return !item.isTabular});
                }
                blade.providers = _.map(providers, function (item) { return { id: item.typeName, name: item.typeName } });
                if (blade.selectedProvider && _.findIndex(blade.providers, function(item) { return item.id === blade.selectedProvider.id; }) === -1) {
                    blade.selectedProvider = undefined;
                }
            }
        }

        $scope.startExport = function () {
            if (!$scope.validateExportParameters()) {
                return;
            }

            blade.exportDataRequest.providerName = blade.selectedProvider.id;
            var progressBlade = {
                id: 'exportProgress',
                title: 'Exporting...',
                controller: 'virtoCommerce.exportModule.exportProgressController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-progress.tpl.html',
                exportDataRequest: blade.exportDataRequest,
                isClosingDisabled: false
            };
            
            bladeNavigationService.showBlade(progressBlade, blade);
        };

        $scope.selectExportedType = function () {
            var exportedTypeblade = {
                id: 'exportedTypeSelector',
                controller: 'virtoCommerce.exportModule.exportTypeSelectorController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-type-selector.tpl.html',
                isClosingDisabled: false,
                exportDataRequest: blade.exportDataRequest,
                onSelected: function(exportDataRequest) {
                    blade.exportDataRequest = angular.extend(blade.exportDataRequest, exportDataRequest);
                    blade.exportDataRequest.dataQuery = angular.copy(exportDataRequest.dataQuery);
                    blade.isExportedTypeSelected = typeof(blade.exportDataRequest.exportTypeName)  !== 'undefined';
                    fillProviders();
                }
            };

            bladeNavigationService.showBlade(exportedTypeblade, blade);
        };

        $scope.selectExportedColumns = function() {
            alert("Select exported columns");
        };

        $scope.selectExportedData = function() {
            alert("Select exported data");
        };

        $scope.validateExportParameters = function () {
            return blade.exportDataRequest && blade.exportDataRequest.exportTypeName && blade.selectedProvider && blade.exportDataRequest.dataQuery;
        };

        $scope.blade.headIcon = 'fa-upload';
        initializeBlade();
    }]);
