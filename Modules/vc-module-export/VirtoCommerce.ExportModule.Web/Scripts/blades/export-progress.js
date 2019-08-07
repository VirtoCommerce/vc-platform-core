angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportProgressController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, bladeNavigationService, exportApi) {
        var blade = $scope.blade;
        blade.isLoading = true;
        $scope.blade.headIcon = 'fa-upload';

        function initializeBlade() {
            blade.isLoading = false;
            exportApi.runExport(blade.exportDataRequest,
                function (data) {
                    blade.notification = data;
                });
        }

        $scope.$on("new-notification-event", function (event, notification) {
            if (blade.notification && notification.id === blade.notification.id) {
                angular.copy(notification, blade.notification);
                if (notification.errorCount > 0) {
                    bladeNavigationService.setError('Export error', blade);
                }
                
                if (blade.notification.finished) {
                    if (blade.onCompleted) {
                        blade.onCompleted();
                    }
                }
            }
        });

        var commandCancel = {
            name: 'platform.commands.cancel',
            icon: 'fa fa-times',
            canExecuteMethod: function () {
                return blade.notification && !blade.notification.finished;
            },
            executeMethod: function () {
                exportApi.cancel({ jobId: blade.notification.jobId }, function (data) {
                });
            }
        };

        blade.toolbarCommands = [commandCancel];
        initializeBlade();
    }]);
