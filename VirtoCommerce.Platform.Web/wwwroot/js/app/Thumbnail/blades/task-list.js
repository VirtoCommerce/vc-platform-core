angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.taskListController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.thumbnail.api', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService',
        function ($scope, bladeNavigationService, bladeUtils, thumbnailApi, uiGridHelper, dialogService) {
            var blade = $scope.blade;

            $scope.uiGridConstants = uiGridHelper.uiGridConstants;
            $scope.hasMore = true;
            $scope.items = [];

            blade.refresh = function () {
                blade.isLoading = true;

                thumbnailApi.getTaskList().then(function (results) {

                    blade.isLoading = false;
                    $scope.items = results;
                    $scope.hasMore = results.length === $scope.pageSettings.itemsPerPageCount;
                });
            };

            blade.setSelectedItem = function (listItem) {
                $scope.selectedNodeId = listItem.id;
            };

            $scope.selectItem = function (e, listItem) {
                blade.setSelectedItem(listItem);

                    var newBlade = {
                        id: "listTaskDetail",
                        itemId: listItem.id,
                        title: 'platform.blades.thumbnail.blades.task-detail.title',
                        subtitle: 'platform.blades.thumbnail.blades.task-detail.subtitle',
                        controller: 'platformWebApp.thumbnail.taskDetailController',
                        template: '$(Platform)/Scripts/app/thumbnail/blades/task-detail.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
            };


            $scope.taskRun = function (itemsSelect) {
                debugger;
                var dialog = {
                    id: "confirmTaskRun",
                    callback: function (doReindex) {
                        var options = _.map(documentTypes, function (x) {
                            return {
                                documentType: x.documentType,
                                deleteExistingIndex: doReindex
                            };
                        });
                        thumbnailApi.taskRun(options).then(function openProgressBlade(data) {
                            debugger;
                            var newBlade = {
                                id: 'thumbnailProgress',
                                notification: data,
                                parentRefresh: blade.parentRefresh,
                                controller: 'virtoCommerce.coreModule.indexProgressController',
                                template: '$(Platform)/Scripts/app/thumbnail/blades/task-detail.tpl.html'
                            };
                            bladeNavigationService.showBlade(newBlade, blade.parentBlade || blade);
                        });
                    }
                }
                debugger;
                dialogService.showDialog(dialog, '$(Platform)/Scripts/app/thumbnail/dialogs/run-dialog.tpl.html', 'platformWebApp.confirmDialogController');
            }


            blade.headIcon = 'fa fa-picture-o';
            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh",
                    icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.add",
                    icon: 'fa fa-plus',
                    executeMethod: blade.refresh,
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.run",
                    icon: 'fa fa-exclamation',
                    canExecuteMethod: function () {
                        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                    },
                    executeMethod: function () {
                        $scope.taskRun($scope.gridApi.selection.getSelectedRows());
                    }
                },
                {
                    name: "platform.commands.delete",
                    icon: 'fa fa-trash-o',
                    canExecuteMethod: function () {
                        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                    },
                    executeMethod: function () {
                        $scope.taskDelete($scope.gridApi.selection.getSelectedRows());
                    }
                }
            ];

            // ui-grid
            $scope.setGridOptions = function (gridOptions) {

                //disable watched
                bladeUtils.initializePagination($scope, true);
                //сhoose the optimal amount that ensures the appearance of the scroll
                $scope.pageSettings.itemsPerPageCount = 50;

                uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                    //update gridApi for current grid
                    $scope.gridApi = gridApi;

                    uiGridHelper.bindRefreshOnSortChanged($scope);
                    //$scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, showMore);
                });

                blade.refresh();
            };
        }]);
