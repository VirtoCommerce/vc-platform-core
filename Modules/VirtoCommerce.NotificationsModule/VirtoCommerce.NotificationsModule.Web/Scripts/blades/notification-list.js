angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationsListController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension', 'platformWebApp.settings',
    function ($scope, notificationsService, notificationTypesResolverService, dialogService, bladeUtils, uiGridHelper, gridOptionExtension, settings) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        var blade = $scope.blade;
        blade.title = 'Notifications';
        blade.selectedType = null;
        var bladeNavigationService = bladeUtils.bladeNavigationService;
        if (!blade.languages) {
          var languages = [{"language":"default"}, {"language":"en-US"}, {"language":"de-DE"}];
      		blade.languages = languages;
      	}

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

        blade.openList = function (item) {
      		var newBlade = {
      			id: 'templatesList',
      			title: 'platform.blades.notification-templates-list.title',
      			notificationType: item.notificationType,
            sendGatewayType: item.sendGatewayType,
      			objectId: blade.objectId,
      			objectTypeId: blade.objectTypeId,
      			languages: blade.languages,
      			controller: 'virtoCommerce.notificationsModule.notificationTemplatesListController',
      			template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notification-templates-list.tpl.html'
      		};

      		bladeNavigationService.showBlade(newBlade, blade);
      	}

        $scope.delete = function (data) {
            //deleteList([data]);
        };

        // function deleteList(selection) {
        //     var dialog = {
        //         id: "confirmDeleteItem",
        //         //TODO localization
        //         title: "customer.dialogs.members-delete.title",
        //         message: "customer.dialogs.members-delete.message",
        //         callback: function (remove) {
        //             if (remove) {
        //                 bladeNavigationService.closeChildrenBlades(blade, function () {
        //                     var memberIds = _.pluck(selection, 'id');
        //
        //                     if (($scope.gridApi != undefined) && $scope.gridApi.selection.getSelectAllState()) {
        //                         var searchCriteria = getSearchCriteria();
        //                         members.delete(searchCriteria, function () {
        //                                 $scope.gridApi.selection.clearSelectedRows();
        //                                 blade.refresh(true);
        //                             }
        //                         );
        //                     }
        //                     else if (_.any(memberIds)) {
        //                         members.remove({ ids: memberIds },
        //                             function () { blade.refresh(true); });
        //                     }
        //                 });
        //             }
        //         }
        //     };
        //     dialogService.showConfirmationDialog(dialog);
        // }

        blade.setSelectedNode = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectNode = function (type) {
            blade.setSelectedNode(type);
            blade.selectedType = type;
        		blade.openList(type);
        };

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
                        //TODO localization
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
                blade.name = undefined;
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
            //gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);

            gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
                // gridApi.core.on.sortChanged($scope, function () {
                //     if (!blade.isLoading) blade.refresh();
                // });
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

        blade.headIcon = 'fa-list';
        //blade.refresh();
    }]);
