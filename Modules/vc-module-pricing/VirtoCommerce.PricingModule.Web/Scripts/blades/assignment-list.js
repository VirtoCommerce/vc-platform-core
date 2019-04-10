angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.assignmentListController', ['$scope', 'virtoCommerce.pricingModule.pricelistAssignments', 'platformWebApp.dialogService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', 'virtoCommerce.catalogModule.catalogs',
function ($scope, assignments, dialogService, uiGridHelper, bladeUtils, catalogs) {
    $scope.uiGridConstants = uiGridHelper.uiGridConstants;
    var blade = $scope.blade;
    var bladeNavigationService = bladeUtils.bladeNavigationService;

    blade.refresh = function () {
        blade.isLoading = true;
        assignments.search({
            pricelistId: blade.pricelistId,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        }, function (data) {
            //Loading catalogs for assignments because they do not contains them
            //Need to display name of catalog in assignments grid
            catalogs.getCatalogs(function (results) {
                blade.isLoading = false;
                $scope.pageSettings.totalItems = data.totalCount;

                var priceAssignments = data.results;
                _.each(priceAssignments, function (x) {
                    var catalog = _.findWhere(results, { id: x.catalogId });
                    if (catalog) {
                        x.catalog = catalog.name;
                    }
                });

                blade.currentEntities = priceAssignments;
            });
        });
    };

    $scope.selectNode = function (node, isNew) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            id: 'pricelistAssignmentDetail',
            controller: 'virtoCommerce.pricingModule.assignmentDetailController',
            template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/assignment-detail.tpl.html'
        };

        if (isNew) {
            angular.extend(newBlade, {
                isNew: true,
                pricelistId: blade.pricelistId,
                data: node,
                title: 'pricing.blades.assignment-detail.new-title'
            });
        } else {
            angular.extend(newBlade, {
                currentEntityId: node.id,
                title: node.name,
                subtitle: 'pricing.blades.assignment-detail.subtitle'
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
            title: "pricing.dialogs.assignments-delete.title",
            message: "pricing.dialogs.assignments-delete.message",
            callback: function (remove) {
                if (remove) {
                    closeChildrenBlades();

                    var itemIds = _.pluck(list, 'id');
                    assignments.remove({ ids: itemIds }, function () {
                        blade.refresh();
                    }, function (error) {
                        bladeNavigationService.setError('Error ' + error.status, blade);
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    function closeChildrenBlades() {
        angular.forEach(blade.childrenBlades.slice(), function (child) {
            bladeNavigationService.closeBlade(child);
        });
    }

    blade.headIcon = 'fa-anchor';
    blade.subtitle = 'pricing.blades.pricelist-assignment-list.subtitle';

    blade.toolbarCommands = [
        {
            name: "platform.commands.refresh", icon: 'fa fa-refresh',
            executeMethod: blade.refresh,
            canExecuteMethod: function () { return true; }
        },
        {
            name: "platform.commands.add", icon: 'fa fa-plus',
            executeMethod: function () {
                $scope.selectNode({}, true);
            },
            canExecuteMethod: function () { return true; },
            permission: 'pricing:create'
        },
        {
            name: "platform.commands.delete", icon: 'fa fa-trash-o',
            executeMethod: function () {
                $scope.deleteList($scope.gridApi.selection.getSelectedRows());
            },
            canExecuteMethod: function () {
                return isItemsChecked();
            },
            permission: 'pricing:delete'
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
