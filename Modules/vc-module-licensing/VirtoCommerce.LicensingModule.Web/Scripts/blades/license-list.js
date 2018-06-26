angular.module('virtoCommerce.licensingModule')
.controller('virtoCommerce.licensingModule.licenseListController', ['$scope', 'virtoCommerce.licensingModule.licenseApi', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
function ($scope, licenseApi, dialogService, uiGridHelper, bladeUtils) {
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;

    blade.refresh = function () {
        blade.isLoading = true;
        return licenseApi.search({
            keyword: filter.keyword,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        }, function (data) {
            blade.isLoading = false;
            blade.currentEntities = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            return data;
        }).$promise;
    };

    $scope.selectNode = function (node, isNew) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            id: 'listItemChild',
            data: node,
            controller: 'virtoCommerce.licensingModule.licenseDetailController',
            template: 'Modules/$(VirtoCommerce.Licensing)/Scripts/blades/license-detail.tpl.html'
        };

        if (isNew) {
            angular.extend(newBlade, {
                isNew: true,
                saveCallback: function (newlicense) {
                    blade.refresh().then(function () {
                        $scope.selectNode(newlicense);
                    });
                }
            });
        }

        bladeNavigationService.showBlade(newBlade, blade);
    };

    function isItemsChecked() {
        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
    }

    $scope.deleteList = function (list) {
        var dialog = {
            id: "confirmDeleteItem",
            title: "licensing.dialogs.licenses-delete.title",
            message: "licensing.dialogs.licenses-delete.message",
            callback: function (remove) {
                if (remove) {
                    bladeNavigationService.closeChildrenBlades(blade, function () {
                        licenseApi.remove({ ids: _.pluck(list, 'id') }, blade.refresh);
                    });
                }
            }
        };
        dialogService.showConfirmationDialog(dialog);
    };

    blade.headIcon = 'fa-id-card';
    blade.subtitle = 'licensing.blades.license-list.subtitle';

    blade.toolbarCommands = [
        {
            name: "platform.commands.refresh", icon: 'fa fa-refresh',
            executeMethod: blade.refresh,
            canExecuteMethod: function () { return true; }
        },
        {
            name: "platform.commands.add", icon: 'fa fa-plus',
            executeMethod: function () {
                $scope.selectNode({ expirationDate: new Date(new Date().setFullYear(new Date().getFullYear() + 1)) }, true);
            },
            canExecuteMethod: function () {
                return true;
            },
            permission: 'licensing:create'
        },
        {
            name: "platform.commands.delete", icon: 'fa fa-trash-o',
            executeMethod: function () {
                $scope.deleteList($scope.gridApi.selection.getSelectedRows());
            },
            canExecuteMethod: isItemsChecked,
            permission: 'licensing:delete'
        }
    ];

    var filter = $scope.filter = {};
    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;

        gridOptions.onRegisterApi = function (gridApi) {
            gridApi.core.on.sortChanged($scope, function () {
                if (!blade.isLoading) blade.refresh();
            });
        };

        bladeUtils.initializePagination($scope);
    };

    // actions on load
    //blade.refresh();
}]);