angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportSettingsController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', '$localStorage', function ($scope, $translate, bladeNavigationService, exportApi, $localStorage) {
        var blade = $scope.blade;
        blade.canStartProcess = false;
        blade.isLoading = true;
        blade.exportDataRequest = blade.exportDataRequest || {};
        blade.allPropertiesOfType = [];
        blade.dataSelected = 0;
        blade.dataTotal = 0;
        blade.propertySelected = 0;
        blade.propertyTotal = 0;
        blade.defaultProvider = $localStorage.defaultExportProvider || 'JsonExportProvider';
        blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
        blade.isTabularExportSupported = blade.exportDataRequest.isTabularExportSupported || false;

        function initializeBlade() {
            exportApi.getProviders(function (result) {
                if (result && result.length) {
                    blade.allProviders = result;
                    fillProviders();
                    blade.selectedProvider = _.find(blade.providers,
                        function (item) { return item.id === blade.defaultProvider });

                    if (blade.isExportedTypeSelected) {
                        getKnownTypes();
                    }
                }
            });

            blade.isLoading = false;
        }

        function getKnownTypes() {
            exportApi.getKnownTypes(function (results) {
                blade.knownTypes = results;
                var selectedType = _.find(results,
                    function (x) { return x.typeName === blade.exportDataRequest.exportTypeName; });
                if (selectedType) {
                    blade.selectedType = selectedType;
                    blade.selectedType.localizedName = $translate.instant('export.types.' + blade.exportDataRequest.exportTypeName + '.name');
                    resetPropertyInfo();
                    getDataTotalCount();
                    getDataSelectedCount();
                }
            });
        }

        function resetPropertyInfo() {
            blade.allPropertiesOfType = blade.selectedProvider.isTabular ? blade.selectedType.tabularMetaData.propertyInfos : blade.selectedType.metaData.propertyInfos;
            blade.exportDataRequest.dataQuery.includedProperties = blade.allPropertiesOfType;
            blade.isTabularExportSupported = blade.selectedType.isTabularExportSupported;
            blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
            blade.propertySelected = blade.allPropertiesOfType.length;
            blade.propertyTotal = blade.propertySelected;
        }

        function fillProviders() {
            if (blade.allProviders) {
                var filterNonTabular = blade.exportDataRequest.exportTypeName && !blade.isTabularExportSupported;
                var providers = blade.allProviders;
                if (filterNonTabular) {
                    providers = _.filter(providers, function (item) { return !item.isTabular });
                }
                blade.providers = _.map(providers, function (item) { return { id: item.typeName, name: $translate.instant('export.provider-names.' + item.typeName), isTabular: item.isTabular } });
                if (blade.selectedProvider && _.findIndex(blade.providers, function (item) { return item.id === blade.selectedProvider.id; }) === -1) {
                    blade.selectedProvider = undefined;
                }
            }
        }

        function getDataTotalCount(fnSuccessCallback) {
            var dataQuery = {
                exportTypeName: blade.exportDataRequest.dataQuery.exportTypeName,
                includedProperties: [],
                skip: 0,
                take: 0
            };

            exportApi.getData(
                {
                    exportTypeName: blade.exportDataRequest.exportTypeName,
                    dataQuery: dataQuery
                }
                , function (data) {
                    blade.dataTotal = data.totalCount;
                    if (fnSuccessCallback) {
                        fnSuccessCallback(data);
                    }
                });
        }

        function getDataSelectedCount() {
            var dataQuery = angular.copy(blade.exportDataRequest.dataQuery);
            dataQuery.includedProperties = [];

            if (!dataQuery.isAllSelected && !(dataQuery.objectIds && dataQuery.objectIds.length)) {
                blade.dataSelected = 0;
            }
            else {
                exportApi.getData(
                {
                    exportTypeName: blade.exportDataRequest.exportTypeName,
                    dataQuery: dataQuery
                }
                , function (data) {
                    blade.dataSelected = data.totalCount;
                });
            }
        }

        $scope.providerChanged = function () {
            $localStorage.defaultExportProvider = blade.selectedProvider.id;
            if (blade.isExportedTypeSelected) {
                resetPropertyInfo(); // Beacuse tabular->nontabular or vice-versa
            }
        };

        $scope.startExport = function () {
            if (!$scope.validateExportParameters()) {
                return;
            }

            blade.exportDataRequest.providerName = blade.selectedProvider.id;
            delete blade.exportDataRequest.dataQuery.skip;
            delete blade.exportDataRequest.dataQuery.take;
            blade.isExporting = true;

            var progressBlade = {
                id: 'exportProgress',
                title: 'export.blades.export-progress.title',
                controller: 'virtoCommerce.exportModule.exportProgressController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-progress.tpl.html',
                exportDataRequest: blade.exportDataRequest,
                isClosingDisabled: true,
                onCompleted: function () {
                    blade.isExporting = false;
                }
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
                onSelected: function (selectedTypeData) {
                    if (!blade.selectedType || blade.selectedType.name != selectedTypeData.selectedType.name) {
                        blade.exportDataRequest = angular.extend(blade.exportDataRequest, selectedTypeData.exportDataRequest);
                        blade.exportDataRequest.dataQuery = angular.copy(selectedTypeData.exportDataRequest.dataQuery);
                        blade.selectedType = selectedTypeData.selectedType;
                        resetPropertyInfo(); // Property set changed due to changing export type
                        fillProviders(); // Refill providers combo for new type
                        getDataTotalCount(
                            function (data) {
                                blade.dataSelected = data.totalCount; // Drop data selection (selected all immediately after exported type selection)
                            }
                            ); // Recalc total available records

                    }
                }
            };

            bladeNavigationService.showBlade(exportedTypeblade, blade);
        };

        $scope.selectExportedProperties = function () {
            var exportedPropertiesblade = {
                id: 'exportedPropertiesSelector',
                controller: 'virtoCommerce.exportModule.exportPropertiesSelectorController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-properties-selector.tpl.html',
                isClosingDisabled: false,
                exportDataRequest: blade.exportDataRequest,
                allPropertiesOfType: blade.allPropertiesOfType,
                onSelected: function (includedProperties) {
                    blade.exportDataRequest.dataQuery.includedProperties = includedProperties;
                    blade.propertySelected = includedProperties.length;
                }
            };

            bladeNavigationService.showBlade(exportedPropertiesblade, blade);
        };

        $scope.selectExportedData = function () {
            var exportedDatablade = {
                id: 'exportedDataSelector',
                title: 'export.blades.export-generic-viewer.title',
                subtitle: 'export.blades.export-generic-viewer.subtitle',
                controller: 'virtoCommerce.exportModule.exportGenericViewerController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-generic-viewer.tpl.html',
                isClosingDisabled: false,
                exportDataRequest: blade.exportDataRequest,
                onCompleted: function (dataQuery) {
                    blade.exportDataRequest.dataQuery = dataQuery;
                    getDataSelectedCount();
                }
            };

            bladeNavigationService.showBlade(exportedDatablade, blade);
        };

        $scope.validateExportParameters = function () {
            return !blade.isExporting &&
                blade.exportDataRequest &&
                blade.exportDataRequest.exportTypeName &&
                blade.selectedProvider &&
                blade.exportDataRequest.dataQuery &&
                blade.dataSelected;
        };

        $scope.blade.headIcon = 'fa-upload';
        initializeBlade();
    }]);
