angular.module('virtoCommerce.shippingModule')
    .controller('virtoCommerce.shippingModule.shippingMethodDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.shippingModule.shippingMethods', 'platformWebApp.settings',
        function ($scope, bladeNavigationService, shippingMethods, settings) {
            var blade = $scope.blade;

            function initializeBlade(data) {
                blade.title = 'shipping.labels.' + data.typeName + '.name';
                blade.currentEntity = angular.copy(data);
                blade.origEntity = data;
                blade.isLoading = false;
            }

            blade.refresh = function (parentRefresh) {
                blade.isLoading = true;
                if (blade.shippingMethod.id) {
                    shippingMethods.get({ id: blade.shippingMethod.id }, function (data) {
                        initializeBlade(data);
                        if (parentRefresh) {
                            blade.parentBlade.refresh();
                        }
                    },
                        function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
                }
                else {
                    initializeBlade(blade.shippingMethod);
                }
            }

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return isDirty();
            }

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "shipping.dialogs.shipping-method-save.title", "shipping.dialogs.shipping-method-save.message");
            };

            $scope.cancelChanges = function () {
                $scope.bladeClose();
            };

            $scope.saveChanges = function () {
                blade.isLoading = true;
                blade.currentEntity.storeId = blade.storeId;
                shippingMethods.update({}, blade.currentEntity, function (data) {
                    blade.shippingMethod.id = data.id;
                    blade.refresh(true);
                }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            };

            $scope.setForm = function (form) {
                $scope.formScope = form;
            }

            $scope.getDictionaryValues = function (setting, callback) {
                callback(setting.allowedValues);
            };

            blade.openDictionarySettingManagement = function (setting) {
                var newBlade = {
                    id: 'settingDetailChild',
                    isApiSave: true,
                    controller: 'platformWebApp.settingDictionaryController',
                    template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
                };
                switch (setting) {
                    case 'TaxTypes':
                        _.extend(newBlade, {
                            currentEntityId: 'VirtoCommerce.Core.General.TaxTypes',
                            parentRefresh: function (data) { blade.taxTypes = data; }
                        });
                        break;
                }

                bladeNavigationService.showBlade(newBlade, blade);
            };

            blade.headIcon = 'fa-archive';

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

            blade.taxTypes = settings.getValues({ id: 'VirtoCommerce.Core.General.TaxTypes' });

            blade.refresh();

        }]);
