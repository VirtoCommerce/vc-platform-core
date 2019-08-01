angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportSettingsController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', '$localStorage', function ($scope, $translate, bladeNavigationService, exportApi, $localStorage) {
        var blade = $scope.blade;
        blade.canStartProcess = false;
        blade.isLoading = true;
        blade.exportDataRequest = blade.exportDataRequest || {};
        blade.allColumnsOfType = [];
        blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
        blade.localizedTypeName = null;
        blade.defaultProvider = $localStorage.defaultExportProvider || 'JsonExportProvider';


        function resetState() {
            blade.dataSelected = 0;
            blade.dataTotal = 0;
            blade.columnSelected = 0;
            blade.columnTotal = 0;  
            blade.isTabularExportSupported = false;
        }
        
        function initializeBlade() {
            resetState();
            if (blade.isExportedTypeSelected) {
                getKnownTypes();
            }

            if (blade.exportDataRequest.dataQuery.objectIds &&
                blade.exportDataRequest.dataQuery.objectIds.length) {
                blade.dataSelected = blade.exportDataRequest.dataQuery.objectIds.length;
            }

            exportApi.getProviders(function (result) {
                if (result && result.length) {
                    blade.allProviders = result;
                    fillProviders();
                    blade.selectedProvider = _.find(blade.providers,
                        function (item) { return item.name === blade.defaultProvider });
                }
            });

            blade.isLoading = false;
        }

        function getKnownTypes()
        {
            exportApi.getKnownTypes(function (results) {
                blade.knownTypes = results;
                var selectedType = _.find(results,
                    function (x) { return x.typeName === blade.exportDataRequest.exportTypeName; });
                if (selectedType) {
                    blade.allColumnsOfType = selectedType.metaData.propertyInfos;
                    blade.isTabularExportSupported = selectedType.isTabularExportSupported;
                    blade.columnSelected = blade.allColumnsOfType.length;
                    blade.columnTotal = blade.columnSelected;

                    getDataTotalCount();
                } else {
                    resetState();
                }
            });
        }

        function fillProviders() {
            if (blade.allProviders) {
                var filterNonTabular = blade.exportDataRequest.exportTypeName && !blade.isTabularExportSupported;
                var providers = blade.allProviders;
                if (filterNonTabular) {
                    providers = _.filter(providers, function (item) { return !item.isTabular });
                }
                blade.providers = _.map(providers, function (item) { return { id: item.typeName, name: $translate.instant('export.provider-names.' + item.typeName) } });
                if (blade.selectedProvider && _.findIndex(blade.providers, function (item) { return item.id === blade.selectedProvider.id; }) === -1) {
                    blade.selectedProvider = undefined;
                }
            }
        }

        function getDataTotalCount() {
            var dataQuery = {
                exportTypeName: blade.exportDataRequest.dataQuery.exportTypeName,
                includedColumns: [],
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
                    if (blade.dataSelected === 0) {
                        blade.dataSelected = blade.dataTotal;
                    }
            });

            
        }

        $scope.providerChanged = function () {
            $localStorage.defaultExportProvider = blade.selectedProvider.name;
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
                onSelected: function (selectedTypeData) {
					resetState();
                    blade.exportDataRequest = angular.extend(blade.exportDataRequest, selectedTypeData.exportDataRequest);
                    blade.exportDataRequest.dataQuery = angular.copy(selectedTypeData.exportDataRequest.dataQuery);
                    blade.allColumnsOfType = selectedTypeData.allColumnsOfType;
                    blade.localizedTypeName = selectedTypeData.localizedTypeName;
                    blade.isTabularExportSupported = selectedTypeData.isTabularExportSupported;
                    blade.isExportedTypeSelected = typeof (blade.exportDataRequest.exportTypeName) !== 'undefined';
                    if (blade.isExportedTypeSelected) {
                        blade.columnSelected = blade.exportDataRequest.dataQuery.includedColumns.length;
                        blade.columnTotal = selectedTypeData.allColumnsOfType.length;
                        fillProviders();
                        getKnownTypes();
                    }
                    
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
                allColumnsOfType: blade.allColumnsOfType,
                onSelected: function (includedColumns) {
                    blade.exportDataRequest.dataQuery.includedColumns = includedColumns;
                    blade.columnSelected = includedColumns.length;
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
                exportDataRequest: blade.exportDataRequest,
                onCompleted: function(dataQuery) {
                    blade.exportDataRequest.dataQuery = dataQuery;
                    blade.dataSelected =
                        (blade.exportDataRequest.dataQuery.objectIds && blade.exportDataRequest.dataQuery.objectIds.length)
                        ? blade.exportDataRequest.dataQuery.objectIds.length
                        : blade.dataTotal;
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
