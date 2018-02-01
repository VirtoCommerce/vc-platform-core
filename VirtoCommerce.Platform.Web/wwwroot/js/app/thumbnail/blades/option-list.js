angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.optionListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.thumbnail.api',
        function ($scope, bladeNavigationService, thumbnailApi) {
            var blade = $scope.blade;

            blade.refresh = function (parentRefresh) {

                blade.isLoading = true;

                thumbnailApi.getListOptions().then(function (results) {
                    blade.isLoading = false;
                    blade.currentEntities = results;
                });

                if (parentRefresh && blade.parentRefresh) {
                    blade.parentRefresh(results);
                }

            };

            blade.setSelectedId = function (selectedNodeId) {
                $scope.selectedNodeId = selectedNodeId;
            };

            function showDetailBlade(bladeData) {
                var newBlade = {
                    id: 'optionDetail',
                    controller: 'platformWebApp.thumbnail.optionDetailController',
                    template: '$(Platform)/Scripts/app/thumbnail/blades/option-detail.tpl.html'
                };
                angular.extend(newBlade, bladeData);
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.selectNode = function (listItem) {
                blade.setSelectedId(listItem.id);
                showDetailBlade({ data: listItem });
            };

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () {
                        blade.setSelectedId(null);
                        showDetailBlade({ isNew: true });
                    },
                    canExecuteMethod: function () {
                        return true;
                    }
                }
            ];

            blade.refresh();
        }]);
