angular.module('virtoCommerce.subscriptionModule')
    .controller('virtoCommerce.subscriptionModule.subscriptionDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.subscriptionModule.subscriptionAPI', 'platformWebApp.dialogService', 'virtoCommerce.orderModule.order_res_customerOrders', '$timeout', 'focus', 'moment',
        function ($scope, bladeNavigationService, subscriptionAPI, dialogService, customerOrders, $timeout, focus, moment) {
            var blade = $scope.blade;
            blade.updatePermission = 'subscription:update';
            blade.customerOrder = {};
            blade.toolbarCommands = [];

            blade.refresh = function () {
                if (blade.id === 'operationDetail') {
                    //if (!blade.isNew)
                    //    blade.initialize(blade.currentEntity);
                }
                else {
                    blade.isLoading = true;
                    subscriptionAPI.get({ id: blade.entityNode.id }, function (result) {
                        blade.initialize(result);
                        blade.customerOrder = blade.currentEntity.customerOrderPrototype;
                        //necessary for scope bounded ACL checks
                        blade.securityScopes = result.scopes;
                    });
                }
            };

            blade.initialize = function (operation) {
                blade.origEntity = operation;
                blade.currentEntity = angular.copy(operation);
                //$timeout(function () {
                //    blade.customInitialize();
                //});

                angular.extend(blade, {
                    title: 'subscription.blades.subscription-detail.title',
                    titleValues: { number: blade.origEntity.number },
                    subtitle: 'subscription.blades.subscription-detail.subtitle'
                });

                initToolbarCommands();

                blade.isLocked = blade.currentEntity.isCancelled;
                blade.isLoading = false;
            };

            // base functions to override as needed
            //blade.customInitialize = function () { };
            blade.setEntityStatus = function (status) {
                blade.currentEntity.subscriptionStatus = status;
            };

            blade.recalculate = function () {
                blade.isLoading = true;
                customerOrders.recalculate(blade.customerOrder, function (result) {
                    angular.copy(result, blade.customerOrder);

                    var idToFocus = document.activeElement.id;
                    if (idToFocus)
                        $timeout(function () {
                            focus(idToFocus);
                            document.getElementById(idToFocus).select();
                        });

                    blade.isLoading = false;
                });
            };

            $scope.saveChanges = function () {
                if (blade.id === 'operationDetail') {
                    angular.copy(blade.currentEntity, blade.origEntity);
                    //if (blade.isNew) {
                    //    blade.realOperationsCollection.push(blade.currentEntity);
                    //    blade.customerOrder.childrenOperations.push(angular.copy(blade.currentEntity));
                    //} else {
                    //    var foundOp = _.findWhere(blade.realOperationsCollection, { id: blade.origEntity.id });
                    //    angular.copy(blade.origEntity, foundOp);
                    //}
                    $scope.bladeClose();

                    if (blade.isTotalsRecalculationNeeded) {
                        blade.recalculate();
                    }
                } else {
                    blade.isLoading = true;
                    subscriptionAPI.update(blade.currentEntity, function () {
                        blade.refresh();
                        blade.parentBlade.refresh();
                    });
                }
            };
            $scope.cancelChanges = function () {
                blade.currentEntity = blade.origEntity;
                $scope.bladeClose();
            };

            function isDirty() {
                return blade.origEntity && !angular.equals(blade.origEntity, blade.currentEntity) && !blade.isNew && blade.hasUpdatePermission();
            }

            function canSave() {
                return isDirty() && (!$scope.formScope || $scope.formScope.$valid);
            }

            $scope.setForm = function (form) { $scope.formScope = form; };

            function initToolbarCommands() {
                blade.toolbarCommands = [
                    {
                        name: "orders.commands.new-document", icon: 'fa fa-plus',
                        executeMethod: function () {
                            var newBlade = {
                                id: "newOperationWizard",
                                customerOrder: blade.customerOrder,
                                currentEntity: blade.currentEntity,
                                stores: blade.stores,
                                availableTypes: blade.knownChildrenOperations,
                                title: "orders.blades.newOperation-wizard.title",
                                subtitle: 'orders.blades.newOperation-wizard.subtitle',
                                controller: 'virtoCommerce.orderModule.newOperationWizardController',
                                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/wizards/newOperation/newOperation-wizard.tpl.html'
                            };
                            bladeNavigationService.showBlade(newBlade, blade);
                        },
                        canExecuteMethod: function () {
                            return !blade.isLocked && _.any(blade.knownChildrenOperations);
                        },
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.save", icon: 'fa fa-save',
                        executeMethod: $scope.saveChanges,
                        canExecuteMethod: canSave,
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.reset", icon: 'fa fa-undo',
                        executeMethod: function () {
                            angular.copy(blade.origEntity, blade.currentEntity);
                        },
                        canExecuteMethod: isDirty,
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.delete", icon: 'fa fa-trash-o',
                        executeMethod: function () {
                            var dialog = {
                                id: "confirmDeleteItem",
                                title: "orders.dialogs.operation-delete.title",
                                message: "orders.dialogs.operation-delete.message",
                                callback: function (remove) {
                                    if (remove) {
                                        if (blade.id === 'operationDetail') {
                                            //var idx = _.findIndex(blade.customerOrder.childrenOperations, function (x) { return x.id === blade.origEntity.id; });
                                            //blade.customerOrder.childrenOperations.splice(idx, 1);
                                            //var idx = _.findIndex(blade.realOperationsCollection, function (x) { return x.id === blade.origEntity.id; });
                                            //blade.realOperationsCollection.splice(idx, 1);

                                            //bladeNavigationService.closeBlade(blade);
                                        }
                                        else {
                                            subscriptionAPI.delete({ ids: blade.origEntity.id }, function () {
                                                blade.parentBlade.refresh();
                                                bladeNavigationService.closeBlade(blade);
                                            });
                                        }
                                    }
                                }
                            };
                            dialogService.showConfirmationDialog(dialog);
                        },
                        canExecuteMethod: function () {
                            return true;
                        },
                        permission: 'order:delete'
                    },
                    {
                        name: "orders.commands.cancel-document", icon: 'fa fa-remove',
                        executeMethod: function () {
                            var dialog = {
                                id: "confirmCancelOperation",
                                callback: function (reason) {
                                    if (reason) {
                                        blade.currentEntity.cancelReason = reason;
                                        blade.currentEntity.cancelledDate = moment().utc();
                                        blade.currentEntity.isCancelled = true;
                                        blade.setEntityStatus('Cancelled');
                                        $scope.saveChanges();
                                    }
                                }
                            };
                            dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.Orders)/Scripts/dialogs/cancelOperation-dialog.tpl.html', 'virtoCommerce.orderModule.confirmCancelDialogController');
                        },
                        canExecuteMethod: function () {
                            return blade.currentEntity && !blade.currentEntity.isCancelled;
                        },
                        permission: blade.updatePermission
                    },
                    {
                        name: "subscription.commands.new-order", icon: 'fa fa-file-text',
                        executeMethod: function () {
                            dialogService.showConfirmationDialog({
                                id: "confirmDialog",
                                title: "subscription.dialogs.new-order.title",
                                message: "subscription.dialogs.new-order.message",
                                callback: function (confirmed) {
                                    if (confirmed) {
                                        bladeNavigationService.closeChildrenBlades(blade, function () {
                                            subscriptionAPI.createOrder(blade.currentEntity, function (result) {
                                                blade.refresh();
                                                blade.parentBlade.refresh();
                                            });
                                        });
                                    }
                                }
                            });
                        },
                        canExecuteMethod: function () {
                            return !blade.isLocked && !isDirty();
                        },
                        permission: 'order:create'
                    }
                ];
            };

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "orders.dialogs.operation-save.title", "orders.dialogs.operation-save.message");
            };

            $scope.cancelOperationResolution = function (resolution) {
                $modalInstance.close(resolution);
            };


            // actions on load
            blade.refresh();
        }]);
