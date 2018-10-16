angular.module('virtoCommerce.pricingModule')
    .controller('virtoCommerce.pricingModule.pricelistDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.pricingModule.pricelists', 'platformWebApp.settings', 'virtoCommerce.coreModule.currency.currencyUtils',
        function ($scope, bladeNavigationService, pricelists, settings, currencyUtils) {
            var blade = $scope.blade;
            blade.updatePermission = 'pricing:update';

            blade.refresh = function (parentRefresh) {
                if (blade.isNew) {
                    initializeBlade({ productPrices: [], assignments: [] });
                } else {
                    pricelists.get({ id: blade.currentEntityId }, function (data) {
                        initializeBlade(data);
                        if (parentRefresh) {
                            blade.parentBlade.refresh();
                        }
                    });
                }
            };

            function initializeBlade(data) {
                if (!blade.isNew) {
                    blade.title = data.name;
                }

                blade.currentEntity = angular.copy(data);
                blade.origEntity = data;
                blade.isLoading = false;
            }

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return isDirty() && $scope.formScope && $scope.formScope.$valid;
            }

            $scope.cancelChanges = function () {
                angular.copy(blade.origEntity, blade.currentEntity);
                $scope.bladeClose();
            };
            $scope.saveChanges = function () {
                blade.isLoading = true;

                if (blade.isNew) {
                	pricelists.save({}, blade.currentEntity, function (data) {
                		angular.copy(blade.currentEntity, blade.origEntity);
                		$scope.bladeClose();
                        if (blade.saveCallback) {
                        	blade.saveCallback(data);
                        }
                        else {
                        	blade.parentBlade.refresh();
                        }  
                    });
                } else {
                    pricelists.update({}, blade.currentEntity, function (data) {
                        blade.refresh(true);
                    });
                }
            };

            $scope.setForm = function (form) { $scope.formScope = form; };

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "pricing.dialogs.pricelist-save.title", "pricing.dialogs.pricelist-save.message");
            };

            blade.headIcon = blade.parentBlade.headIcon;

            function initializeToolbar() {
                if (!blade.isNew) {
                    blade.toolbarCommands = [
                        {
                            name: "platform.commands.save",
                            icon: 'fa fa-save',
                            executeMethod: $scope.saveChanges,
                            canExecuteMethod: canSave,
                            permission: blade.updatePermission
                        },
                        {
                            name: "platform.commands.reset",
                            icon: 'fa fa-undo',
                            executeMethod: function () {
                                angular.copy(blade.origEntity, blade.currentEntity);
                            },
                            canExecuteMethod: isDirty,
                            permission: blade.updatePermission
                        }
                    ];
                }
            }

            initializeToolbar();
            blade.refresh(false);
            $scope.currencyUtils = currencyUtils;
        }]);