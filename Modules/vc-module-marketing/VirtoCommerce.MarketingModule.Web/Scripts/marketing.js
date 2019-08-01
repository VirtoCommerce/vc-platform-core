//Call this to register our module to main application
var moduleName = "virtoCommerce.marketingModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.config(
  ['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.marketing', {
              url: '/marketing',
              templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
              controller: [
                  '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                      var blade = {
                          id: 'marketing',
                          title: 'marketing.blades.marketing-main.title',
                          controller: 'virtoCommerce.marketingModule.marketingMainController',
                          template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/common/marketing-main.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(blade);
                  }
              ]
          });
  }]
)
.run(
    ['$http', '$compile', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', 'platformWebApp.toolbarService', '$state', 'platformWebApp.authService', 'virtoCommerce.storeModule.stores', 'platformWebApp.permissionScopeResolver', 'platformWebApp.bladeNavigationService'
    , function ($http, $compile, mainMenuService, widgetService, toolbarService, $state, authService, stores, permissionScopeResolver, bladeNavigationService) {
      // // test toolbar commands and content
      //toolbarService.register({
      //    name: "ADDITIONAL COMMAND", icon: 'fa fa-cloud',
      //    executeMethod: function (blade) {
      //        console.log('test: ' + this.name + this.icon + blade);
      //    },
      //    canExecuteMethod: function () { return true; },
      //    index: 2
      //}, 'virtoCommerce.marketingModule.itemsDynamicContentListController');
      //toolbarService.register({
      //    name: "EXTERNAL ACTION", icon: 'fa fa-bolt',
      //    executeMethod: function (blade) {
      //        console.log('test: ' + this.name + this.icon + blade);
      //    },
      //    canExecuteMethod: function () { return true; },
      //    index: 0
      //}, 'virtoCommerce.marketingModule.itemsDynamicContentListController');
      
      //Register module in main menu
      var menuItem = {
          path: 'browse/marketing',
          icon: 'fa fa-flag',
          title: 'marketing.main-menu-title',
          priority: 40,
          action: function () { $state.go('workspace.marketing'); },
          permission: 'marketing:access'
      };
      mainMenuService.addMenuItem(menuItem);

      widgetService.registerWidget({
          controller: 'virtoCommerce.marketingModule.couponsWidgetController',
          template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/widgets/couponsWidget.tpl.html'
      }, 'promotionDetail');

        //Register permission scopes templates used for scope bounded definition in role management ui
      var marketingStoreScope = {
          type: 'MarketingStoreSelectedScope',
          title: 'Only for promotions in selected stores',
          selectFn: function (blade, callback) {
              var newBlade = {
                  id: 'store-pick',
                  title: this.title,
                  subtitle: 'Select stores',
                  currentEntity: this,
                  onChangesConfirmedFn: callback,
                  dataPromise: stores.query().$promise,
                  controller: 'platformWebApp.security.scopeValuePickFromSimpleListController',
                  template: '$(Platform)/Scripts/app/security/blades/common/scope-value-pick-from-simple-list.tpl.html'
              };
              bladeNavigationService.showBlade(newBlade, blade);
          }
      };
      permissionScopeResolver.register(marketingStoreScope);

      //Register dashboard widgets
      //widgetService.registerWidget({
      //    isVisible: function (blade) { return authService.checkPermission('marketing:read'); },
      //    controller: 'virtoCommerce.marketingModule.dashboard.promotionsWidgetController',
      //    template: 'tile-count.html'
      //}, 'mainDashboard');

      $http.get('Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicConditions/templates.html').then(function (response) {
        // compile the response, which will put stuff into the cache
        $compile(response.data);
    });
  }]);
