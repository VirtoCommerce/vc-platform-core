angular.module('virtoCommerce.imageToolsModule')
    .controller('virtoCommerce.imageToolsModule.taskListController', ['$scope','$timeout', 'platformWebApp.bladeNavigationService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.dialogService', 'virtoCommerce.imageToolsModule.taskApi',
        function ($scope, $timeout, bladeNavigationService, bladeUtils, uiGridHelper, dialogService, taskApi) {
            var blade = $scope.blade;

            $scope.uiGridConstants = uiGridHelper.uiGridConstants;
            $scope.hasMore = true;
            $scope.items = [];

            blade.refresh = function () {
                blade.isLoading = true;

                if ($scope.pageSettings.currentPage !== 1)
                    $scope.pageSettings.currentPage = 1;

                var searchCriteria = getSearchCriteria();

                taskApi.search(searchCriteria,
                    function (data) {
                        addDescriptionItem(data.result);
                        $scope.items = data.results;

                        $scope.hasMore = data.totalCount === $scope.pageSettings.itemsPerPageCount;

                            $timeout(function () {
                                // wait for grid to ingest data changes
                                if ($scope.gridApi && $scope.gridApi.selection.getSelectAllState()) {
                                    $scope.gridApi.selection.selectAllRows();
                                }
                            });
                    }).$promise.finally(function () {
                        blade.isLoading = false;
                    });
                //reset state grid
                resetStateGrid();
            };

            function showMore() {
                if ($scope.hasMore) {
                        ++$scope.pageSettings.currentPage;
                        $scope.gridApi.infiniteScroll.saveScrollPercentage();
                        blade.isLoading = true;
                        var searchCriteria = getSearchCriteria();

                        taskApi.search(searchCriteria,
                            function (data) {
                                addDescriptionItem(data.result);
                                $scope.items = $scope.items.concat(data.result);
                                $scope.hasMore = data.listEntries.length === $scope.pageSettings.itemsPerPageCount;
                                $scope.gridApi.infiniteScroll.dataLoaded();

                                $timeout(function () {
                                    // wait for grid to ingest data changes
                                    if ($scope.gridApi.selection.getSelectAllState()) {
                                        $scope.gridApi.selection.selectAllRows();
                                    }
                                });

                        }).$promise.finally(function () {
                            blade.isLoading = false;
                        });
                    };
            }

            //add description for item
            function addDescriptionItem(items) {
                angular.forEach(items, function (item) {
                    var firstThree = _.first(item.thumbnailOptions, 3);
                    firstThree = _.map(firstThree, function (option) { return option.width + 'x' + option.height });
                    if (firstThree && firstThree.length) {
                        item.description = '(' + firstThree.join(', ') + ')';
                    }
                });
            }

            // Search Criteria
            function getSearchCriteria() {
                var searchCriteria = {
                    sort: uiGridHelper.getSortExpression($scope),
                    skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                    take: $scope.pageSettings.itemsPerPageCount
                };
                return searchCriteria;
            }

            //reset state grid (header checkbox, scroll)
            function resetStateGrid() {
                if ($scope.gridApi) {
                    $scope.items = [];
                    $scope.gridApi.selection.clearSelectedRows();
                    $scope.gridApi.infiniteScroll.resetScroll(true, true);
                    $scope.gridApi.infiniteScroll.dataLoaded();
                }
            };

            blade.setSelectedItem = function (listItem) {
                $scope.selectedNodeId = listItem.id;
            };

            $scope.selectItem = function (e, listItem) {
                blade.setSelectedItem(listItem);
                var newBlade = {
                    id: "listTaskDetail",
                    currentEntityId: listItem.id,
                    title: 'imageTools.blades.task-detail.title',
                    subtitle: 'imageTools.blades.task-detail.subtitle',
                    controller: 'virtoCommerce.imageToolsModule.taskDetailController',
                    template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/blades/task-detail.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };


            $scope.selectNode = function (node, isNew) {
                $scope.selectedNodeId = node.id;

                var newBlade = {
                    id: 'listTaskDetail',
                    controller: 'virtoCommerce.imageToolsModule.taskDetailController',
                    template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/blades/task-detail.tpl.html'
                };

                if (isNew) {
                    angular.extend(newBlade, {
                        title: 'imageTools.blades.task-detail.title',
                        isNew: true,
                        saveCallback: function (newPricelist) {
                            newBlade.isNew = false;
                            blade.refresh(true).then(function () {
                                newBlade.currentEntityId = newPricelist.id;
                                bladeNavigationService.showBlade(newBlade, blade);
                            });
                        }
                        // onChangesConfirmedFn: callback,
                    });
                } else {
                    angular.extend(newBlade, {
                        currentEntityId: node.id,
                        title: node.name,
                        subtitle: 'imageTools.blades.task-detail.subtitle'
                    });
                }

                bladeNavigationService.showBlade(newBlade, blade);
            };


            $scope.taskRun = function (itemsSelect) {
                var dialog = {
                    id: "confirmTaskRun",
                    isFirstRun: _.some(itemsSelect, function (item) { return !item.lastRun }),
                    callback: function (regenerate) {
                        var request = {
                            taskIds: _.pluck(itemsSelect, "id"),
                            regenerate: regenerate
                        };

                        taskApi.taskRun(request, function (notification) {
                            var newBlade = {
                                id: 'thumbnailProgress',
                                notification: notification,
                                controller: 'virtoCommerce.imageToolsModule.taskRunController',
                                template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/blades/task-progress.tpl.html'
                            };

                                $scope.$on("new-notification-event", function (event, notification) {
                                    if (notification && notification.id == newBlade.notification.id) {
                                        blade.canImport = notification.finished != null;
                                    }
                                });

                            bladeNavigationService.showBlade(newBlade, blade.parentBlade || blade);
                        }, function (error) {
                                bladeNavigationService.setError('Error ' + error.status, blade);
                            }
                        );
                    }
                }

                dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.ImageTools)/Scripts/dialogs/run-dialog.tpl.html', 'platformWebApp.confirmDialogController');
            }

            function isItemsChecked() {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            }

            function getSelectedItems() {
                return $scope.gridApi.selection.getSelectedRows();
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
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: function () {
                        $scope.selectNode({}, true);
                    },
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "imageTools.commands.run",
                    icon: 'fa fa-exclamation',
                    canExecuteMethod: isItemsChecked,
                    executeMethod: function () {
                        $scope.taskRun(getSelectedItems());
                    }
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: function () {
                        deleteList(getSelectedItems());
                    },
                    canExecuteMethod: isItemsChecked
                }
            ];

            function deleteList(selection) {
                var dialog = {
                    id: "confirmDelete",
                    title: "imageTools.dialogs.task-delete.title",
                    message: "imageTools.dialogs.task-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;
                            bladeNavigationService.closeChildrenBlades(blade);
                            var ids = _.map(selection, function(task) { return task.id });
                            taskApi.delete({ ids: ids }, function() {
                                    blade.refresh();
                                })};
                        }
                }
                dialogService.showConfirmationDialog(dialog);
            }

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
                    $scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, showMore);
                });

                blade.refresh();
            };
        }]);
