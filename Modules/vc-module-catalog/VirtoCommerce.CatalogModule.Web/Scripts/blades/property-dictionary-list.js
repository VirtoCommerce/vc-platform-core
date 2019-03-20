angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.propertyDictionaryListController', ['$scope', '$filter', 'platformWebApp.dialogService', 'platformWebApp.settings', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'virtoCommerce.catalogModule.propDictItems', 'platformWebApp.bladeUtils', function ($scope, $filter, dialogService, settings, bladeNavigationService, uiGridHelper, propDictItems, bladeUtils) {
        var blade = $scope.blade;
        $scope.blade.isLoading = false;
        blade.headIcon = 'fa-book';
        $scope.currentEntities = [];

        $scope.uiGridConstants = uiGridHelper.uiGridConstants;

        // simple and advanced filtering
        var filter = blade.filter = $scope.filter = {};

        filter.criteriaChanged = function () {
            if ($scope.pageSettings.currentPage > 1) {
                $scope.pageSettings.currentPage = 1;
            } else {
                blade.refresh();
            }
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () {
                    $scope.selectNode({ values: [], isNew: true, propertyId : blade.property.id })
                },
                canExecuteMethod: function () {
                    return true;
                }
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () {
                    $scope.deleteList($scope.gridApi.selection.getSelectedRows());
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
            }

        ];

        blade.refresh = function () {
            blade.languages = blade.property.multilanguage ? blade.languages : [];
            var criteria = {
                searchPhrase: filter.keyword,
                propertyIds: [blade.property.id],
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            };
          
            propDictItems.search(criteria, function (result) {
                $scope.pageSettings.totalItems = result.totalCount;
                $scope.currentEntities = result.results;
                $scope.currentEntities.forEach(function (dictItem) {
                    blade.languages.forEach(function (lang) {
                        var dictValue = _.find(dictItem.localizedValues, function (x) { return x.languageCode === lang || (!x.languageCode && lang === blade.defaultLanguage) });
                        dictItem[lang] = dictValue ? dictValue.value : undefined;
                    });
                });
            });
        };

      
        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                if (gridApi && gridApi.core) {
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                }
            });
            bladeUtils.initializePagination($scope);
        };

        $scope.deleteList = function (selection) {
            var dialog = {
                id: "confirmDeletePropertyValue",
                title: "catalog.dialogs.dictionary-values-delete.title",
                message: "catalog.dialogs.dictionary-values-delete.message",
                callback: function (remove) {
                    if (remove) {
                        bladeNavigationService.closeChildrenBlades(blade, function () {
                            var dictItemsIds = _.pluck(selection, 'id');
                            propDictItems.remove({ ids: dictItemsIds }, function (data) {
                                blade.refresh();
                            });
                        });
                    }
                }
            };
            dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.Catalog)/Scripts/dialogs/deletePropertyDictionaryItem-dialog.tpl.html', 'platformWebApp.confirmDialogController');
        };

        $scope.deleteDictItem = function (selectedDictItem) {
            $scope.deleteList([selectedDictItem]);
        }


        $scope.selectNode = function (selectedDictItem) {

            if (selectedDictItem.alias) {
                $scope.selectedNodeId = selectedDictItem.alias;
            }
            var newBlade = {
                id: 'propertyDictionaryDetails',
                title: 'catalog.blades.property-dictionary.labels.dictionary-edit',
                controller: 'virtoCommerce.catalogModule.propertyDictionaryDetailsController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-dictionary-details.tpl.html',
                dictionaryItem: selectedDictItem,
                property: blade.property,
                languages: blade.languages,
                defaultLanguage: blade.defaultLanguage,

                onSaveChanges: function (dictItem) {                 
                        blade.refresh();
                },
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };
             
        //blade.refresh();

    }]);
