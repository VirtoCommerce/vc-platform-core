//Call this to register our module to main application
var moduleTemplateName = "virtoCommerce.notificationsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.notificationsModule', {
                    url: '/notifications?objectId&objectTypeId',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var blade = {
                                id: 'notifications',
                                title: 'platform.menu.notifications',
                                subtitle: 'platform.blades.notifications-menu.subtitle',
                                controller: 'virtoCommerce.notificationsModule.notificationsMenuController',
                                template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-menu.tpl.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(blade);
                        }
                    ]
                });
        }
    ])
    // define search filters to be accessible platform-wide
    .factory('virtoCommerce.notificationsModule.predefinedSearchFilters', ['$localStorage', function ($localStorage) {
        $localStorage.notificationJournalSearchFilters = $localStorage.notificationJournalSearchFilters || [];
        return {
            register: function (currentFiltersUpdateTime, currentFiltersStorageKey, newFilters) {
                _.each(newFilters, function (newFilter) {
                    var found = _.find($localStorage.notificationJournalSearchFilters, function (x) {
                        return x.id == newFilter.id;
                    });
                    if (found) {
                        if (found && (!found.lastUpdateTime || found.lastUpdateTime < currentFiltersUpdateTime)) {
                            angular.copy(newFilter, found);
                        }
                    } else if (!$localStorage[currentFiltersStorageKey] || $localStorage[currentFiltersStorageKey] < currentFiltersUpdateTime) {
                        $localStorage.notificationJournalSearchFilters.splice(0, 0, newFilter);
                    }
                });

                $localStorage[currentFiltersStorageKey] = currentFiltersUpdateTime;
            }
        };
    }])
    .run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'virtoCommerce.notificationsModule.notificationTypesResolverService',
        function ($rootScope, mainMenuService, widgetService, $state, notificationTypesResolverService) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/notificationsModule',
                icon: 'fa fa-envelope',
                title: 'Notifications',
                priority: 7,
                action: function () { $state.go('workspace.notificationsModule'); },
                permission: 'notificationsModulePermission'
            };
            mainMenuService.addMenuItem(menuItem);

            widgetService.registerWidget({
      	        controller: 'virtoCommerce.notificationsModule.notificationsTemplatesWidgetController',
      	        template: 'Modules/$(VirtoCommerce.notificationsModule)/Scripts/widgets/notificationsTemplatesWidget.tpl.html'
      	    }, 'notificationsDetail');
            widgetService.registerWidget({
      	        controller: 'virtoCommerce.notificationsModule.notificationsLogWidgetController',
      	        template: 'Modules/$(VirtoCommerce.notificationsModule)/Scripts/widgets/notificationsLogWidget.tpl.html'
      	    }, 'notificationsDetail');
            widgetService.registerWidget({
      	        controller: 'virtoCommerce.notificationsModule.notificationsAttachmentsWidgetController',
      	        template: 'Modules/$(VirtoCommerce.notificationsModule)/Scripts/widgets/notificationsAttachmentsWidget.tpl.html'
      	    }, 'notificationsDetail');
            
            // register types
            notificationTypesResolverService.registerType({
                type: 'Email',
                icon: 'fa fa-envelope',
                detailBlade: {
                  template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-edit-template.tpl.html'
                },
                knownChildrenTypes: ['Email', 'Sms']
            });  
        }
          
        
    ]);
