angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsListController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension', 'platformWebApp.settings',
    function ($scope, notificationsService, notificationTypesResolverService, dialogService, bladeUtils, uiGridHelper, gridOptionExtension, settings) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        var blade = $scope.blade;
        blade.title = 'Notifications';
        blade.selectedType = null;
        var bladeNavigationService = bladeUtils.bladeNavigationService;
        if (!blade.languages) {
            var languages = ["default", "en-US", "de-DE"];
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

        blade.editTemplate = function (item) {
      		var newBlade = {
      			id: 'editNotification',
      			title: 'notifications.blades.notification-details.title',
                titleValues: { displayName: item.displayName },
      			notificationType: item.notificationType,
      			objectId: blade.objectId,
      			objectTypeId: blade.objectTypeId,
      			controller: 'virtoCommerce.notificationsModule.notificationsEditController',
      			template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notification-details.tpl.html'
      		};

      		bladeNavigationService.showBlade(newBlade, blade);
      	};

        blade.setSelectedNode = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectNode = function (type) {
           blade.setSelectedNode(type);
           blade.selectedType = type;
      	   blade.editTemplate(type);
        };

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

        blade.headIcon = 'fa-list';
        //blade.refresh();
    }]);