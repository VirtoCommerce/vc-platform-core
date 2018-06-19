angular.module("virtoCommerce.coreModule.searchIndex", [])
.config(['$stateProvider', function ($stateProvider) {
    $stateProvider.state('workspace.searchIndexModule', {
        url: '/searchIndex',
        controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
            var blade = {
                id: 'searchIndex',
                title: 'core.blades.document-type-list.title',
                headIcon: 'fa fa-search',
                controller: 'virtoCommerce.coreModule.searchIndex.indexesListController',
                template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/blades/indexes-list.tpl.html',
                isClosingDisabled: true
            };
            bladeNavigationService.showBlade(blade);
            $scope.moduleName = 'vc-core vc-search-index';
        }],
        templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html'
    });
}])
.run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService', function ($rootScope, mainMenuService, widgetService, $state, pushNotificationTemplateResolver, bladeNavigationService) {

    // register notification template
    pushNotificationTemplateResolver.register({
        priority: 900,
        satisfy: function (notify, place) { return place == 'history' && notify.notifyType == 'IndexProgressPushNotification'; },
        template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/notifications/historyIndex.tpl.html',
        action: function (notify) {
            var blade = {
                id: 'indexProgress',
                notification: notify,
                controller: 'virtoCommerce.coreModule.indexProgressController',
                template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/blades/index-progress.tpl.html'
            };
            bladeNavigationService.showBlade(blade);
        }
    });

    pushNotificationTemplateResolver.register({
        priority: 900,
        satisfy: function (notify, place) { return place == 'menu' && notify.notifyType == 'IndexProgressPushNotification';  },
        template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/notifications/menuIndex.tpl.html',
        action: function (notify) { $state.go('workspace.pushNotificationsHistory', notify) }
    });

    mainMenuService.addMenuItem({
            path: 'browse/searchIndex',
            icon: 'fa fa-search',
            title: 'core.main-menu-title.search-index',
            priority: 25,
            action: function () {
                $state.go('workspace.searchIndexModule');
            }
        });
    }
]);
