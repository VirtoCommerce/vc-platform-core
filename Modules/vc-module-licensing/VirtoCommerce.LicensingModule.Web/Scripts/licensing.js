//Call this to register our module to main application
var moduleName = "virtoCommerce.licensingModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.config(
  ['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.licensingModule', {
              url: '/licensing',
              templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
              controller: [
                  '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                      var blade = {
                          id: 'license-list',
                          title: 'licensing.blades.license-list.title',
                          subtitle: 'licensing.blades.license-list.subtitle',
                          controller: 'virtoCommerce.licensingModule.licenseListController',
                          template: 'Modules/$(VirtoCommerce.Licensing)/Scripts/blades/license-list.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(blade);
                      //Need for isolate and prevent conflict module css to other modules 
                      $scope.moduleName = "vc-licensing";
                  }
              ]
          });
  }]
)
.run(
  ['platformWebApp.mainMenuService', '$state', 'platformWebApp.widgetService', function (mainMenuService, $state, widgetService) {
      //Register module in main menu
      var menuItem = {
          path: 'browse/licensing',
          icon: 'fa fa-id-card',
          title: 'licensing.main-menu-title',
          priority: 200,
          action: function () { $state.go('workspace.licensingModule'); },
          permission: 'licensing:access'
      };
      mainMenuService.addMenuItem(menuItem);

      widgetService.registerWidget({
          controller: 'platformWebApp.changeLog.operationsWidgetController',
          isVisible: function (blade) { return !blade.isNew; },
          template: '$(Platform)/Scripts/app/changeLog/widgets/operations-widget.tpl.html'
      }, 'licenseDetail');
  }]);
