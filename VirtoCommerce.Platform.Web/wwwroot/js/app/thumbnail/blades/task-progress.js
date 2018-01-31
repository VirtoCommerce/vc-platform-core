angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.taskRunController', ['$scope', 'platformWebApp.thumbnail.api', function ($scope, thumbnailApi) {
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
                thumbnailApi.cancel({ tasksId: [blade.notification.id] });
            }
        }];

        blade.title = blade.notification.title;
        blade.headIcon = 'fa-search';
        blade.isLoading = false;
    }]);
