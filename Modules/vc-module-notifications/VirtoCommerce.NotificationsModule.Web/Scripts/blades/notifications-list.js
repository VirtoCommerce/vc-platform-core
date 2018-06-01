angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationsListController', ['$scope', '$translate', 'virtoCommerce.notificationsModule.notificationsModuleApi', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension', 'platformWebApp.settings', 'platformWebApp.i18n', 'platformWebApp.authService',
    function ($scope, $translate, notifications, notificationTypesResolverService, dialogService, bladeUtils, uiGridHelper, gridOptionExtension, settings, i18n, authService) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        var blade = $scope.blade;
        blade.title = 'Notifications';
        blade.selectedType = null;
        blade.currentLanguage = i18n.getLanguage();
        var bladeNavigationService = bladeUtils.bladeNavigationService;
        
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
        
        function getSearchCriteria() {
            var searchCriteria = {
                keyword: filter.keyword ? filter.keyword : undefined,
                sort: uiGridHelper.getSortExpression($scope),
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount
            };
            return searchCriteria;
        }

        blade.refresh = function () {
            var searchCriteria = getSearchCriteria();
            notifications.getNotificationList(searchCriteria, function (data) {
                blade.isLoading = false;
                $scope.pageSettings.totalItems = data.totalCount;
                $scope.listEntries = data.results ? data.results : [];
                _.map($scope.listEntries, function(entity) {
                    var foundNotification = notificationTypesResolverService.resolve(entity.kind); 
                    if (foundNotification) {
                        entity.icon = foundNotification.icon;
                    }
                    return entity; 
                });
            });
        }

        function resolveType(kind) {
            var foundNotification = notificationTypesResolverService.resolve(kind);
            if (foundNotification && foundNotification.knownChildrenTypes && foundNotification.knownChildrenTypes.length) {
                    return foundNotification;
                } else {
                    dialogService.showNotificationDialog({
                        id: "error",
                        title: "notifications.dialogs.unknown-kind.title",
                        message: "notifications.dialogs.unknown-kind.message",
                        messageValues: { kind: kind },
                });
            }
        }

		if (authService.checkPermission('notifications:access')) {
			blade.editNotification = function (item) {
                var foundNotification = resolveType(item.kind);
				var newBlade = {
					id: 'editNotification',
					title: 'notifications.blades.notification-details.title',
					titleValues: { displayName: $translate.instant('notificationTypes.' + item.type + '.displayName') },
					type: item.type,
					tenantId: blade.tenantId,
					tenantType: blade.tenantType,
					controller: foundNotification.detailBlade.controller,
                    template: foundNotification.detailBlade.template
				};

				bladeNavigationService.showBlade(newBlade, blade);
			};
		};

        blade.setSelectedNode = function (listItem) {
            $scope.selectedNodeId = listItem.type;
        };

        $scope.selectNode = function (type) {
           blade.setSelectedNode(type);
           blade.selectedType = type;
      	   blade.editNotification(type);
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

        blade.headIcon = 'fa-list';
        //blade.refresh();
    }]);