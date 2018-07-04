angular.module('virtoCommerce.imageToolsModule')
    .controller('virtoCommerce.imageToolsModule.taskRunController', ['$scope', 'virtoCommerce.imageToolsModule.taskApi', function ($scope, taskApi) {
        var blade = $scope.blade;

        $scope.$on("new-notification-event", function (event, notification) {
            if (blade.notification && notification.id == blade.notification.id) {
                angular.copy(notification, blade.notification);
            }
        });

        blade.toolbarCommands = [{
            name: 'platform.commands.cancel',
            icon: 'fa fa-times',
            canExecuteMethod: function () {
                return blade.notification && !blade.notification.finished;
            },
            executeMethod: function () {
                taskApi.taskCancel({ jobId: blade.notification.jobId }, null, function(data) {
                    
                });
            }
        }];

        blade.title = blade.notification.title;
        blade.headIcon = 'fa-search';
        blade.isLoading = false;
    }]);
