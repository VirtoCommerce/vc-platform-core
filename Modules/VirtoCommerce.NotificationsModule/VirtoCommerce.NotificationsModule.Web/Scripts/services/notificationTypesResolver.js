angular.module('virtoCommerce.notificationsModule')
.factory('virtoCommerce.notificationsModule.notificationTypesResolverService', function () {
    return {
        objects: [],
        registerType: function (notificationTypeDefinition) {
            notificationTypeDefinition.detailBlade = angular.extend({
                id: "notificationDetail",
                metaFields: [],
                controller: 'virtoCommerce.notificationsModule.editTemplateController',
                notificationTypeDefinition: notificationTypeDefinition
            }, notificationTypeDefinition.detailBlade);

            notificationTypeDefinition.knownChildrenTypes = notificationTypeDefinition.knownChildrenTypes || [];

            this.objects.push(notificationTypeDefinition);
        },
        resolve: function (type) {
            return _.findWhere(this.objects, { type: type });
        }
    };
});﻿
