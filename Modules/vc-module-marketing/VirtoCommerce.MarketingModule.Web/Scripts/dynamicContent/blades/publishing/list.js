angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.publishingDynamicContentListController', ['$scope', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService', 'virtoCommerce.marketingModule.dynamicContent.contentPublications',
function ($scope, bladeUtils, uiGridHelper, dialogService, dynamicContentPublicationsApi) {
    var bladeNavigationService = bladeUtils.bladeNavigationService;
    var blade = $scope.blade;
    blade.headIcon = 'fa-paperclip';
    blade.isLoading = false;

    blade.initialize = function () {
        blade.refresh();
    };

    blade.removePublishings = function (selectedRows) {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            var dialog = {
                id: 'confirmDeletePublishings',
                title: 'marketing.dialogs.publication-delete.title',
                message: 'marketing.dialogs.publication-delete.message',
                callback: function (confirm) {
                    if (confirm) {
                        var publishingIds = _.pluck(selectedRows, 'id');
                        dynamicContentPublicationsApi.remove({ ids: publishingIds }, blade.refresh);
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        });
    };

    blade.refresh = function () {
        blade.isLoading = true;
        dynamicContentPublicationsApi.search({
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount,
            sort: uiGridHelper.getSortExpression($scope),
            responseGroup: '8',
            keyword: blade.searchKeyword
        }, function (data) {
            $scope.listEntries = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            blade.isLoading = false;
        });
    };

    blade.toolbarCommands = [{
        name: 'platform.commands.refresh', icon: 'fa fa-refresh',
        canExecuteMethod: function () { return true; },
        executeMethod: blade.refresh
    }, {
        name: 'platform.commands.add', icon: 'fa fa-plus',
        canExecuteMethod: function () { return true; },
        executeMethod: function () { $scope.selectNode({}, true); }
    }, {
        name: 'platform.commands.delete', icon: 'fa fa-trash',
        canExecuteMethod: isItemsChecked,
        executeMethod: function () {
            return blade.removePublishings($scope.gridApi.selection.getSelectedRows());
        }
    }];

    function isItemsChecked() {
        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
    }

    $scope.clearKeyword = function () {
        blade.searchKeyword = null;
        blade.refresh();
    };

    $scope.selectNode = function (node, isNew) {
        $scope.selectedNodeId = node.id;

        var newBlade = {
            isNew: isNew,
            controller: 'virtoCommerce.marketingModule.publicationDetailController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/publishing/publication-detail.tpl.html'
        };

        if (isNew) {
            angular.extend(newBlade, {
                id: 'add_publishing_element',
                title: 'marketing.blades.publishing.publishing-main-step.title-new',
                subtitle: 'marketing.blades.publishing.publishing-main-step.subtitle-new'
            });
        } else {
            angular.extend(newBlade, {
                id: 'edit_publishing_element',
                title: 'marketing.blades.publishing.publishing-main-step.title',
                subtitle: 'marketing.blades.publishing.publishing-main-step.subtitle',
                currentEntity: node
            });
        }
        bladeNavigationService.showBlade(newBlade, blade);
    };

    $scope.deleteItems = function (items) {
        var dialog = {
            id: "confirmDeleteContentPublications",
            title: "marketing.dialogs.content-item-folder-delete.title",
            message: "marketing.dialogs.content-item-folder-delete.message",
            callback: function (remove) {
                if (remove) {
                    dynamicContentPublicationsApi.remove({
                        ids: _.pluck(items, 'id')
                    }, function () {
                        blade.refresh();
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    $scope.setGridOptions = function (gridOptions) {
        $scope.gridOptions = gridOptions;

        gridOptions.onRegisterApi = function (gridApi) {
            gridApi.core.on.sortChanged($scope, function () {
                if (!blade.isLoading) blade.refresh();
            });
        };

        bladeUtils.initializePagination($scope);
    };
}]);