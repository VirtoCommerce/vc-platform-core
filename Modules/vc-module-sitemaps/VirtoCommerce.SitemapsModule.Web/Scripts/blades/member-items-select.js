angular.module('virtoCommerce.customerModule')
.controller('virtoCommerce.customerModule.memberItemSelectController', ['$scope', '$timeout', 'virtoCommerce.customerModule.members', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'virtoCommerce.customerModule.memberTypesResolverService',
function ($scope, $timeout, members, dialogService, bladeUtils, uiGridHelper, memberTypesResolverService) {
    $scope.uiGridConstants = uiGridHelper.uiGridConstants;
    var bladeNavigationService = bladeUtils.bladeNavigationService;
    var blade = $scope.blade;
    blade.selectedItems = [];

    blade.headIcon = 'fa-user';
    blade.currentEntity = {};
    blade.setSelectedNode = function (listItem) {
        $scope.selectedNodeId = listItem.id;
    };

    blade.refresh = function () {
        blade.isLoading = true;
        members.search({
            memberTypes: blade.options.memberTypes,
            memberType: blade.memberType,
            memberId: blade.currentEntity.id,
            keyword: filter.keyword ? filter.keyword : undefined,
            sort: uiGridHelper.getSortExpression($scope),
            skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
            take: $scope.pageSettings.itemsPerPageCount
        },
        function (data) {
            $scope.listEntries = data.results;
            $scope.pageSettings.totalItems = data.totalCount;
            var memberTypeDefinition;
            _.each(data.results, function (x) {
                if (memberTypeDefinition = memberTypesResolverService.resolve(x.memberType)) {
                    x._memberTypeIcon = memberTypeDefinition.icon;
                }
            });
            setBreadcrumbs();
            blade.isLoading = false;
        });
    };

    $scope.options = angular.extend({
        showCheckingMultiple: true,
        allowCheckingItem: true,
        selectedItemIds: []
    }, blade.options);

    $scope.selectNode = function (listItem) {
        if ($scope.selectedNodeId === listItem.id) {
            return;
        }
        blade.setSelectedNode(listItem);

        if ($scope.options.selectItemFn) {
            $scope.options.selectItemFn(listItem);
        }
        if (listItem.memberType.toLowerCase() === 'organization') {
            var newBlade = {
                id: blade.id,
                breadcrumbs: blade.breadcrumbs,
                subtitle: 'customer.blades.member-list.subtitle',
                subtitleValues: { name: listItem.name },
                currentEntity: listItem,
                disableOpenAnimation: true,
                controller: blade.controller,
                template: blade.template,
                isClosingDisabled: true
            };
            bladeNavigationService.showBlade(newBlade, blade.parentBlade);
        } else {
            blade.showDetailBlade(listItem);
        }
    };

    var filter = $scope.filter = {};
    filter.criteriaChanged = function () {
        if (filter.keyword === null) {
            blade.memberType = undefined;
        }
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    function setBreadcrumbs() {
        if (blade.breadcrumbs) {
            var breadcrumbs = blade.breadcrumbs.slice(0);
            if (_.all(breadcrumbs, function (x) { return x.id !== blade.currentEntity.id; })) {
                var breadCrumb = generateBreadcrumb(blade.currentEntity.id, blade.currentEntity.name);
                breadcrumbs.push(breadCrumb);
            }
            blade.breadcrumbs = breadcrumbs;
        } else {
            blade.breadcrumbs = [generateBreadcrumb(null, 'all')];
        }
    }

    function generateBreadcrumb(id, name) {
        return {
            id: id,
            name: name,
            blade: blade,
            navigate: function (breadcrumb) {
                breadcrumb.blade.disableOpenAnimation = true;
                bladeNavigationService.showBlade(breadcrumb.blade);
                breadcrumb.blade.refresh();
            }
        };
    }

    // ui-grid
    $scope.setGridOptions = function (gridOptions) {
        uiGridHelper.initialize($scope, gridOptions, externalRegisterApiCallback);
        bladeUtils.initializePagination($scope);
    };

    function externalRegisterApiCallback(gridApi) {
        gridApi.grid.registerDataChangeCallback(function (grid) {
            $timeout(function () {
                _.each($scope.items, function (x) {
                    if (_.some($scope.options.selectedItemIds, function (y) { return y === x.id; })) {
                        gridApi.selection.selectRow(x);
                    }
                });
            });
        }, [$scope.uiGridConstants.dataChange.ROW]);

        gridApi.selection.on.rowSelectionChanged($scope, function (row) {
            if ($scope.options.checkItemFn) {
                $scope.options.checkItemFn(row.entity, row.isSelected);
            }
            if (row.isSelected) {
                if (!_.contains($scope.options.selectedItemIds, row.entity.id)) {
                    $scope.options.selectedItemIds.push(row.entity.id);
                    blade.selectedItems.push(row.entity);
                }
            }
            else {
                $scope.options.selectedItemIds = _.without($scope.options.selectedItemIds, row.entity.id);
                blade.selectedItems = _.filter(blade.selectedItems, function (x) { return x.id !== row.entity.id; });
            }
        });

        uiGridHelper.bindRefreshOnSortChanged($scope);
    }
}]);