angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationsListController', ['$scope', 'notificationsModuleApi', 'notificationsService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension',
    function ($scope, notificationsModuleApi, notificationsService, dialogService, bladeUtils, uiGridHelper, gridOptionExtension) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        var blade = $scope.blade;
        blade.title = 'Notifications';
        var bladeNavigationService = bladeUtils.bladeNavigationService;

        //TODO del notificationsService
        // blade.refresh = function () {
        //     notificationsModuleApi.getNotificationList(function (data) {
        //         blade.data = data.result;
        //         blade.isLoading = false;
        //     });
        // }
        blade.refresh = function (parentRefresh) {
            blade.isLoading = true;
            var searchCriteria = getSearchCriteria();
            notificationsService.getNotificationList(searchCriteria).then(function(data) {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    $scope.listEntries = data.results ? data.results : [];
                }
            );
            if (parentRefresh && blade.parentRefresh) {
                blade.parentRefresh();
            }
        };
        
        blade.showDetailBlade = function (listItem, isNew) {
            blade.setSelectedNode(listItem);

            var foundTemplate = memberTypesResolverService.resolve(listItem.memberType);
            if (foundTemplate) {
                var newBlade = angular.copy(foundTemplate.detailBlade);
                newBlade.currentEntity = listItem;
                newBlade.currentEntityId = listItem.id;
                newBlade.isNew = isNew;
                bladeNavigationService.showBlade(newBlade, blade);
            } else {
                dialogService.showNotificationDialog({
                    id: "error",
                    title: "customer.dialogs.unknown-member-type.title",
                    message: "customer.dialogs.unknown-member-type.message",
                    messageValues: { memberType: listItem.memberType },
                });
            }
        };

        $scope.delete = function (data) {
            deleteList([data]);
        };

        function deleteList(selection) {
            var dialog = {
                id: "confirmDeleteItem",
                title: "customer.dialogs.members-delete.title",
                message: "customer.dialogs.members-delete.message",
                callback: function (remove) {
                    if (remove) {
                        bladeNavigationService.closeChildrenBlades(blade, function () {
                            var memberIds = _.pluck(selection, 'id');

                            if (($scope.gridApi != undefined) && $scope.gridApi.selection.getSelectAllState()) {
                                var searchCriteria = getSearchCriteria();
                                members.delete(searchCriteria, function () {
                                        $scope.gridApi.selection.clearSelectedRows();
                                        blade.refresh(true);
                                    }
                                );
                            }
                            else if (_.any(memberIds)) {
                                members.remove({ ids: memberIds },
                                    function () { blade.refresh(true); });
                            }
                        });
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        }

        blade.setSelectedNode = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectNode = function (listItem) {
            blade.setSelectedNode(listItem);

            var foundTemplate = memberTypesResolverService.resolve(listItem.memberType);
            if (foundTemplate && foundTemplate.knownChildrenTypes && foundTemplate.knownChildrenTypes.length) {
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

        blade.headIcon = 'fa-user __customers';

        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () {
                    return true;
                }
            },
            {
                name: "platform.commands.add", icon: 'fa fa-plus',
                executeMethod: function () {
                    var newBlade = {
                        id: 'listItemChild',
                        currentEntity: blade.currentEntity,
                        title: 'customer.blades.member-add.title',
                        subtitle: 'customer.blades.member-add.subtitle',
                        controller: 'virtoCommerce.customerModule.memberAddController',
                        template: 'Modules/$(VirtoCommerce.NotificationsModule)/Scripts/blades/notifications-add.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                },
                canExecuteMethod: function () {
                    return true;
                },
                permission: 'notification:create'
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () { deleteList($scope.gridApi.selection.getSelectedRows()); },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
                permission: 'notification:delete'
            }
        ];

        // filtering
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

        // ui-grid
        $scope.setGridOptions = function (gridId, gridOptions) {
            $scope.gridOptions = gridOptions;
            gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);

            gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
                gridApi.core.on.sortChanged($scope, function () {
                    if (!blade.isLoading) blade.refresh();
                });
            };

            bladeUtils.initializePagination($scope);
        };

        function getSearchCriteria() {
            var searchCriteria = {
                searchPhrase: filter.keyword ? filter.keyword : undefined,
                deepSearch: filter.keyword ? true : false,
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            };
            return searchCriteria;
        }

        //blade.refresh();
    }]);
