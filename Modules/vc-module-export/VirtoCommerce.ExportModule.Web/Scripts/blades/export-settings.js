angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportSettingsController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, $translate, bladeNavigationService, exportApi) {
        var blade = $scope.blade;
        blade.canStartProcess = false;
        blade.isLoading = true;
        blade.exportDataRequest = blade.exportDataRequest || {};
        blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
        
        function initializeBlade() {
            exportApi.getProviders(function (result) {
                if (result && result.length) {
                    blade.allProviders = result;
                    fillProviders();
                    if (typeof (blade.exportDataRequest.defaultProvider) !== 'undefined') {
                        blade.selectedProvider = _.find(blade.providers,
                            function (item) { return item.name === blade.exportDataRequest.defaultProvider});
                    }
                }
            });

            if (blade.isExportedTypeSelected) {
                exportApi.getKnownTypes(function (results) {
                    blade.knownTypes = results;
                    if (blade.selectedProvider.isTabular) {
                        results = _.filter(results, function (x) { return x.isTabularExportSupported === true; });
                    }
                    var selectedTypes = _.filter(results,
                        function (x) { return x.typeName === blade.exportDataRequest.exportTypeName; });
                    blade.exportDataRequest.allColumnsOfType = selectedTypes[0].metaData.propertyInfos;
                });
            }            
            blade.isLoading = false;
        }

        function fillProviders() {
            if (blade.allProviders) {
                blade.providers = _.map(blade.allProviders, function (item) { return { id: item.typeName, name: item.typeName, isTabular: item.isTabular} });
                if (blade.selectedProvider && _.findIndex(blade.providers, function(item) { return item.id === blade.selectedProvider.id; }) === -1) {
                    blade.selectedProvider = undefined;
                }
            }
        }

        $scope.providerChanged = function () {
            blade.exportDataRequest = {};
            blade.isExportedTypeSelected = false;
            blade.includedColumnsDescription = null;
        };

        $scope.startExport = function () {
            if (!$scope.validateExportParameters()) {
                return;
            }

            blade.exportDataRequest.providerName = blade.selectedProvider.id;
            var progressBlade = {
                id: 'exportProgress',
                title: 'export.blades.export-progress.title',
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
                title: 'export.blades.export-type-selector.title',
                subtitle: 'export.blades.export-type-selector.subtitle',
                controller: 'virtoCommerce.exportModule.exportTypeSelectorController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-type-selector.tpl.html',
                isClosingDisabled: false,
                knownTypes: blade.knownTypes,
                exportDataRequest: blade.exportDataRequest,
                selectedProvider: blade.selectedProvider,
                onSelected: function(exportDataRequest) {
                    blade.exportDataRequest = angular.extend(blade.exportDataRequest, exportDataRequest);
                    blade.exportDataRequest.dataQuery = angular.copy(exportDataRequest.dataQuery);
                    blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
                    blade.includedColumnsDescription = null;
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
                    if (blade.exportDataRequest.dataQuery.includedColumns.length !== blade.exportDataRequest.allColumnsOfType.length) {
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
            var exportedDatablade = {
                id: 'exportedDataSelector',
                title: 'export.blades.export-generic-viewer.title',
                subtitle: 'export.blades.export-generic-viewer.subtitle',
                controller: 'virtoCommerce.exportModule.exportGenericViewerController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-generic-viewer.tpl.html',
                isClosingDisabled: false,
                exportDataRequest: angular.copy(blade.exportDataRequest),
                onCompleted: function(dataQuery) {
                    blade.exportDataRequest.dataQuery = dataQuery;
                }
            };

            bladeNavigationService.showBlade(exportedDatablade, blade);
        };

        $scope.validateExportParameters = function () {
            return blade.exportDataRequest && blade.exportDataRequest.exportTypeName && blade.selectedProvider && blade.exportDataRequest.dataQuery;
        };

        $scope.blade.headIcon = 'fa-upload';
        initializeBlade();
    }]);
