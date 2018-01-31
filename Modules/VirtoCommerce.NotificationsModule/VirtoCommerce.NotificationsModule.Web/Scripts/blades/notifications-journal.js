angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsJournalController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'platformWebApp.bladeNavigationService', 'platformWebApp.notifications', 'platformWebApp.bladeUtils', 'platformWebApp.dialogService', 'uiGridConstants', 'platformWebApp.uiGridHelper',
    function ($scope, notificationsService, bladeNavigationService, notifications, bladeUtils, dialogService, uiGridConstants, uiGridHelper) {
        var blade = $scope.blade;
        $scope.uiGridConstants = uiGridConstants;
        // simple and advanced filtering
        var filter = blade.filter = { keyword: blade.filterKeyword };
        function transformByFilters(data) {
            _.each(data, function (x) {
                x.$path = _.any(x.path) ? x.path.join(" \\ ") : '\\';
            });
        }

        blade.refresh = function () {
//            notifications.getNotificationJournalList({
//                objectId: blade.objectId,
//                objectTypeId: blade.objectTypeId,
//                start: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
//                count: $scope.pageSettings.itemsPerPageCount,
//                sort: uiGridHelper.getSortExpression($scope)
//            }, function (data) {
//                blade.currentEntities = data.notifications;
//                $scope.pageSettings.totalItems = data.totalCount;
//                blade.isLoading = false;
//            });
            notificationsService.getNotificationJournalList().then(function(data) {
                transformByFilters(data.listEntries);
                blade.currentEntities = data.notifications;
                $scope.pageSettings.totalItems = data.totalCount;
                blade.isLoading = false;              
            });
        };

        $scope.selectNode = function (data) {
            $scope.selectedNodeId = data.id;

            var newBlade = {
                id: 'notificationDetails',
                title: 'notifications.blades.notification-journal-details.title',
                currentNotificationId: data.id,
                currentEntity: data,
                controller: 'virtoCommerce.notificationsModule.notificationJournalDetailsController',
                template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notification-journal-details.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.stopNotifications = function (list) {
            blade.isLoading = true;
            notifications.stopSendingNotifications(_.pluck(list, 'id'), blade.refresh);
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () { return true; }
            },
            {
                name: "platform.commands.stop-sending", icon: 'fa fa-stop',
                executeMethod: function () {
                    $scope.stopNotifications($scope.gridApi.selection.getSelectedRows());
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                }
            }
        ];
        
        // Search Criteria
        function getSearchCriteria()
        {
            var searchCriteria = {
                catalogId: blade.catalogId,
                categoryId: blade.categoryId,
                keyword: filter.keyword ? filter.keyword : undefined,
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            };
            return searchCriteria;
        }
        
        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                //update gridApi for current grid
                $scope.gridApi = gridApi;
                uiGridHelper.bindRefreshOnSortChanged($scope);
            });
            bladeUtils.initializePagination($scope);
        };
    }]);
