//Call this to register our module to main application
var moduleName = "virtoCommerce.customerModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
.config(
  ['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.customerModule', {
              url: '/customers',
              templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
              controller: [
                  '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                      var newBlade = {
                          id: 'memberList',
                          currentEntity: { id: null },
                          controller: 'virtoCommerce.customerModule.memberListController',
                          template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/member-list.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(newBlade);
                  }
              ]
          });
  }]
)
.run(
    ['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'virtoCommerce.customerModule.memberTypesResolverService', 'platformWebApp.settings', 'virtoCommerce.customerModule.members', function ($rootScope, mainMenuService, widgetService, $state, memberTypesResolverService, settings, members) {
      //Register module in main menu
      var menuItem = {
          path: 'browse/member',
          icon: 'fa fa-user __customers',
          title: 'customer.main-menu-title',
          priority: 180,
          action: function () { $state.go('workspace.customerModule'); },
          permission: 'customer:access'
      };
      mainMenuService.addMenuItem(menuItem);

      var accountsWidget = {
          isVisible: function (blade) { return !blade.isNew; },
          controller: 'virtoCommerce.customerModule.customerAccountsWidgetController',
          template: 'Modules/$(VirtoCommerce.Customer)/Scripts/widgets/customerAccountsWidget.tpl.html'
      };
      var addressesWidget = {
          controller: 'virtoCommerce.customerModule.memberAddressesWidgetController',
          template: 'Modules/$(VirtoCommerce.Customer)/Scripts/widgets/memberAddressesWidget.tpl.html'
      };
      var emailsWidget = {
          controller: 'virtoCommerce.customerModule.memberEmailsWidgetController',
          template: 'Modules/$(VirtoCommerce.Customer)/Scripts/widgets/memberEmailsWidget.tpl.html'
      };
      var phonesWidget = {
          controller: 'virtoCommerce.customerModule.memberPhonesWidgetController',
          template: 'Modules/$(VirtoCommerce.Customer)/Scripts/widgets/memberPhonesWidget.tpl.html'
      };
      var dynamicPropertyWidget = {
          controller: 'platformWebApp.dynamicPropertyWidgetController',
          template: '$(Platform)/Scripts/app/dynamicProperties/widgets/dynamicPropertyWidget.tpl.html',
          isVisible: function (blade) { return !blade.isNew; }
      }
      var vendorSeoWidget = {
          controller: 'virtoCommerce.coreModule.seo.seoWidgetController',
          template: 'Modules/$(VirtoCommerce.Core)/Scripts/SEO/widgets/seoWidget.tpl.html',
          objectType: 'Vendor',
          getDefaultContainerId: function (blade) { return undefined; },
          getLanguages: function (blade) {
              return settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });
          },
          isVisible: function (blade) { return !blade.isNew; }
      };

      // register WIDGETS
      var indexWidget = {
          documentType: 'Member',
          controller: 'virtoCommerce.coreModule.searchIndex.indexWidgetController',
          // size: [3, 1],
          template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/widgets/index-widget.tpl.html',
          isVisible: function (blade) { return !blade.isNew; }
      };

     

      //Register widgets in customer details
      widgetService.registerWidget(accountsWidget, 'customerDetail1');
      widgetService.registerWidget(addressesWidget, 'customerDetail1');
      widgetService.registerWidget(emailsWidget, 'customerDetail1');
      widgetService.registerWidget(phonesWidget, 'customerDetail2');
      widgetService.registerWidget(dynamicPropertyWidget, 'customerDetail2');
      widgetService.registerWidget(indexWidget, 'customerDetail2');

      //Register widgets in organization details
      widgetService.registerWidget(addressesWidget, 'organizationDetail1');
      widgetService.registerWidget(emailsWidget, 'organizationDetail1');
      widgetService.registerWidget(phonesWidget, 'organizationDetail1');
      widgetService.registerWidget(dynamicPropertyWidget, 'organizationDetail2');
      widgetService.registerWidget(indexWidget, 'organizationDetail2');

      //Register widgets in employee details
      widgetService.registerWidget(accountsWidget, 'employeeDetail1');
      widgetService.registerWidget(addressesWidget, 'employeeDetail1');
      widgetService.registerWidget(emailsWidget, 'employeeDetail1');
      widgetService.registerWidget(phonesWidget, 'employeeDetail2');
      widgetService.registerWidget(dynamicPropertyWidget, 'employeeDetail2');
      widgetService.registerWidget(indexWidget, 'employeeDetail2');

      //Register widgets in vendor details
      widgetService.registerWidget(addressesWidget, 'vendorDetail1');
      widgetService.registerWidget(emailsWidget, 'vendorDetail1');
      widgetService.registerWidget(phonesWidget, 'vendorDetail1');
      widgetService.registerWidget(dynamicPropertyWidget, 'vendorDetail2');
      widgetService.registerWidget(vendorSeoWidget, 'vendorDetail2');
      widgetService.registerWidget(indexWidget, 'vendorDetail2');

      // register member types
      memberTypesResolverService.registerType({
          memberType: 'Organization',
          description: 'customer.blades.member-add.organization.description',
          fullTypeName: 'VirtoCommerce.Domain.Customer.Model.Organization',
          icon: 'fa-university',
          detailBlade: {
              template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/organization-detail.tpl.html',
              metaFields: [{
                  name: 'businessCategory',
                  title: "customer.blades.organization-detail.labels.business-category",
                  valueType: "ShortText"
              }]
          },
          knownChildrenTypes: ['Organization', 'Employee', 'Contact']
      });
      memberTypesResolverService.registerType({
          memberType: 'Employee',
          description: 'customer.blades.member-add.employee.description',
          fullTypeName: 'VirtoCommerce.Domain.Customer.Model.Employee',
          icon: ' fa-user',
          detailBlade: {
              template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/employee-detail.tpl.html',
              metaFields: [{
                  name: 'defaultLanguage',
                  title: "customer.blades.employee-detail.labels.defaultLanguage",
                  valueType: "ShortText"
              },
              {
                  name: 'photoUrl',
                  title: "customer.blades.employee-detail.labels.photo-url",
                  valueType: "Url"
              }]
          }
      });
      memberTypesResolverService.registerType({
          memberType: 'Contact',
          description: 'customer.blades.member-add.contact.description',
          fullTypeName: 'VirtoCommerce.Domain.Customer.Model.Contact',
          icon: 'fa-smile-o',
          detailBlade: {
              template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/customer-detail.tpl.html',
              metaFields: [{
                  name: 'preferredCommunication',
                  title: "customer.blades.contact-detail.labels.preferred-communication",
                  valueType: "ShortText"
              },
              {
                  name: 'preferredDelivery',
                  title: "customer.blades.contact-detail.labels.preferred-delivery",
                  valueType: "ShortText"
              }]
          }
      });
      memberTypesResolverService.registerType({
          memberType: 'Vendor',
          description: 'customer.blades.member-add.vendor.description',
          fullTypeName: 'VirtoCommerce.Domain.Customer.Model.Vendor',
          icon: 'fa-balance-scale',
          detailBlade: {
              template: 'Modules/$(VirtoCommerce.Customer)/Scripts/blades/vendor-detail.tpl.html',
              metaFields: [{
                  name: 'description',
                  title: "customer.blades.vendor-detail.labels.description",
                  valueType: "LongText"
              }]
          }
      });
  }]);