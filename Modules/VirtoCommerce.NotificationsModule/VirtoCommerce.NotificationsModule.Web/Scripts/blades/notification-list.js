var fakeData = 'json=' + encodeURIComponent(angular.toJson([{
        "displayName": "Registration notification",
        "description": "This notification is sent by email to a client when he finishes registration",
        "isEmail": true,
        "isSms": false,
        "type": "RegistrationEmailNotification",
        "isActive": true,
        "isSuccessSend": false,
        "attemptCount": 0,
        "maxAttemptCount": 10
    }]));

angular.module('virtoCommerce.notificationsModule')
    .controller('virtoCommerce.notificationsModule.notificationsListController', ['$scope', 'notificationsModuleApi', 'notificationsService', function ($scope, notificationsModuleApi, notificationsService) {
        var blade = $scope.blade;
        blade.title = 'Notifications';

        // blade.refresh = function () {
        //     notificationsModuleApi.getNotificationList(function (data) {
        //         blade.data = data.result;
        //         blade.isLoading = false;
        //     });
        // }
        blade.refresh = function () {
          notificationsService.getNotificationList()
            .success(function(result) {
                blade.data = fakeData;
                blade.isLoading = false;
            })
            .error(function(response, status) {
                console.log("Failed to get the name, response is " + response + " and status is " + status);
            });
        };

        blade.refresh();
    }]);
