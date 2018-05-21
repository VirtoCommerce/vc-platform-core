var moduleTemplateName = "virtoCommerce.notificationsSampleModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
.run(['virtoCommerce.notificationsModule.notificationTypesResolverService', 'virtoCommerce.notificationsModule.notificationTemplatesResolverService', function(notificationTypesResolverService, notificationTemplatesResolverService) {
    notificationTypesResolverService.registerType({
        type: 'TwitterNotification',
        icon: 'fa fa-twitter',
        detailBlade: {
          template: 'Modules/$(VirtoCommerce.NotificationsSample)/Scripts/blades/notifications-twitter-details.tpl.html',
          controller: 'virtoCommerce.notificationsSampleModule.editTwitterController',
          tenantIdentity: { tenantId: "NotificationSampleId", tenantType: "NotificationSampleType"}
        },
        knownChildrenTypes: ['Twitter']
    }); 

    // register templates
    notificationTemplatesResolverService.registerTemplate({
        type: 'TwitterNotification',
        icon: 'fa fa-twitter',
        detailBlade: {
          template: 'Modules/$(VirtoCommerce.NotificationsSample)/Scripts/blades/notifications-twitter-edit-template.tpl.html',
        },
        knownChildrenTypes: ['Twitter']
    }); 
}]);
