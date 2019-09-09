import * as signalR from '@aspnet/signalr';

angular.module('platformWebApp').factory('platformWebApp.signalRHubProxy', ['$rootScope', function ($rootScope) {
    function signalRHubProxyFactory() {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl('/pushNotificationHub')
            .build();

        connection.start();

        return {
            on: function (eventName, callback) {
                connection.on(eventName, function (result) {
                    $rootScope.$apply(function () {
                        if (callback) {
                            callback(result);
                        }
                    });
                });
            },
            off: function (eventName, callback) {
                connection.off(eventName, function (result) {
                    $rootScope.$apply(function () {
                        if (callback) {
                            callback(result);
                        }
                    });
                });
            },
            invoke: function (methodName, callback) {
                connection.invoke(methodName)
                    .done(function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
            },
            connection: connection
        };
    };

    return signalRHubProxyFactory;
}]);
