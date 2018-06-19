angular.module('virtoCommerce.coreModule.searchIndex')
.controller('virtoCommerce.coreModule.searchIndex.indexesListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'virtoCommerce.coreModule.searchIndex.searchIndexation', 'platformWebApp.ui-grid.extension',
    function ($scope, bladeNavigationService, dialogService, bladeUtils, searchIndexationApi, gridOptionExtension) {
        var blade = $scope.blade;
        blade.isLoading = false;

        blade.refresh = function () {
            blade.isLoading = true;
            searchIndexationApi.get({}, function (response) {
                blade.currentEntities = response;
                blade.isLoading = false;
            });
        }

        $scope.rebuildIndex = function(documentTypes) {
            var dialog = {
                id: "confirmRebuildIndex",
                callback: function (doReindex) {
                    var options = _.map(documentTypes, function (x) {
                        return {
                            documentType: x.documentType,
                            deleteExistingIndex: doReindex
                        };
                    });
                    searchIndexationApi.index(options, function openProgressBlade(data) {
                        // show indexing progress
                        var newBlade = {
                            id: 'indexProgress',
                            notification: data,
                            parentRefresh: blade.parentRefresh,
                            controller: 'virtoCommerce.coreModule.indexProgressController',
                            template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/blades/index-progress.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade.parentBlade || blade);
                    });
                }
            }
            dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/dialogs/reindex-dialog.tpl.html', 'platformWebApp.confirmDialogController');
        }

        blade.toolbarCommands = [{
            name: 'platform.commands.refresh',
            icon: 'fa fa-refresh',
            canExecuteMethod: function () {
                return true;
            },
            executeMethod: function () {
                blade.refresh();
            }
        }, {
            name: 'core.commands.rebuild-index',
            icon: 'fa fa-recycle',
            canExecuteMethod: function () {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            },
            executeMethod: function () {
                $scope.rebuildIndex($scope.gridApi.selection.getSelectedRows());
            },
            permission: 'core:search:index:rebuild'
        }];

        // ui-grid
        $scope.setGridOptions = function (gridId, gridOptions) {
            $scope.gridOptions = gridOptions;
            gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);
            return gridOptions;
        };
        blade.refresh();
    }
]);