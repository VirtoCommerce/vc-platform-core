angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.optionListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.thumbnail.api',
        function ($scope, bladeNavigationService, thumbnailApi) {
            var blade = $scope.blade;

            blade.refresh = function (parentRefresh) {

                blade.isLoading = true;

                thumbnailApi.getListOptionByTask(blade.id).then(function (results) {
                    blade.isLoading = false;
                    blade.currentEntities = results;
                });

            };

            blade.setSelectedId = function (selectedNodeId) {
                $scope.selectedNodeId = selectedNodeId;
            };

            function showDetailBlade(listItem) {
                debugger;
                var newBlade = {
                    id: 'optionDetail',
                    itemId: listItem.id,
                    controller: 'platformWebApp.thumbnail.optionDetailController',
                    template: '$(Platform)/Scripts/app/thumbnail/blades/option-detail.tpl.html'
                };
                angular.extend(newBlade, listItem);
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.selectNode = function (listItem) {
                blade.setSelectedId(listItem.id);
                showDetailBlade(listItem);
            };

            blade.toolbarCommands = [
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () {
                        blade.setSelectedId(null);
                        showDetailBlade({ isNew: true });
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
                    permission: 'core:currency:create'
                }
            ];

            blade.refresh();
        }]);
