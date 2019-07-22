angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportSettingsController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, $translate, bladeNavigationService, exportApi) {
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
                    blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
                    blade.includedColumnsDescription = null;
                    fillProviders();
                }
            };

            bladeNavigationService.showBlade(exportedTypeblade, blade);
        };

        $scope.selectExportedColumns = function() {
            var exportedColumnsblade = {
                id: 'exportedColumnsSelector',
                controller: 'virtoCommerce.exportModule.exportColumnsSelectorController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-columns-selector.tpl.html',
                isClosingDisabled: false,
                exportDataRequest: blade.exportDataRequest,
                onSelected: function (includedColumns) {
                    blade.exportDataRequest.dataQuery.includedColumns = includedColumns;
                    if (blade.exportDataRequest.dataQuery.includedColumns.length != blade.exportDataRequest.metaData.propertyInfos.length) {
                        var includedColumnsNames = _.pluck(blade.exportDataRequest.dataQuery.includedColumns, 'name');
                        if (includedColumnsNames.length > 10) {
                            blade.includedColumnsDescription = includedColumnsNames.slice(0, 10).join(', ') + ', ... (+' + (includedColumnsNames.length - 10) + ' ' + $translate.instant('export.blades.export-settings.labels.columns-more') + ')';
                        }
                        else {
                            blade.includedColumnsDescription = includedColumnsNames.join(', ');
                        }
                    }
                    else {
                        blade.includedColumnsDescription = null;
                    }
                }
            };

            bladeNavigationService.showBlade(exportedColumnsblade, blade);
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
