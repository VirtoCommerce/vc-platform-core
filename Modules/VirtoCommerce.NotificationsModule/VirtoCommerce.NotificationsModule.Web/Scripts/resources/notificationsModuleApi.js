angular.module('virtoCommerce.notificationsModule')
    .factory('virtoCommerce.notificationsModule.notificationsModuleApi', ['$resource', function ($resource) {
        return $resource('api/notifications/:id', { id: '@Id' }, {
            getNotificationList: { method: 'GET', url: 'api/notifications' },
            getNotificationByType: { method: 'GET', url: 'api/notifications/:type', },
            updateNotification: { method: 'PUT', url: 'api/notifications/:type' },
            getTemplates: { method: 'GET', url: 'api/notifications/:type/templates', isArray: true },
            getTemplateById: { method: 'GET', url: 'api/notifications/:type/templates/:id' },
            getTemplate: { method: 'GET', url: 'api/notifications/:type/:language/templates' },
            createTemplate: { method: 'POST', url: 'api/notifications/:type/templates' },
            updateTemplate: { method: 'PUT', url: 'api/notifications/:type/templates/:id' }
//            deleteTemplate: { method: 'DELETE', url: 'api/notifications/template/:id' },
//            prepareTestData: { method: 'GET', url: 'api/notifications/template/:type/getTestingParameters', isArray: true },
//            resolveNotification: { method: 'POST', url: 'api/notifications/template/rendernotificationcontent' },
//            sendNotification: { method: 'POST', url: 'api/notifications/template/sendnotification' },
//            getNotificationJournalList: { method: 'GET', url: 'api/notifications/journal/:objectId/:objectTypeId' },
//            getNotificationJournalDetails: { method: 'GET', url: 'api/notifications/journal/:id' },
//            stopSendingNotifications: { method: 'POST', url: 'api/notifications/stopnotifications' }
        })
    }]);
