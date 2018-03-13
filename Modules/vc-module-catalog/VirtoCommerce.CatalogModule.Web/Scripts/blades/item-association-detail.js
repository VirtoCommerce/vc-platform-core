angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemAssociationDetailController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings',
    function ($scope, dialogService, bladeNavigationService, settings) {
        var blade = $scope.blade;

        $scope.isValid = false;

        $scope.$watch("blade.currentEntity", function () {
            $scope.isValid = $scope.formScope && $scope.formScope.$valid;
        }, true);

        function initialize(item) {
            $scope.associationGroups = settings.getValues({ id: 'Catalog.AssociationGroups' }, function (data) {
                if (data && data.length > 0) {
                    blade.groupName = data[0];
                }
            });

            blade.currentEntity = angular.copy(item);
            blade.isLoading = false;
        };

        blade.toolbarCommands = [
            {
                name: "catalog.commands.open-item",
                icon: 'fa fa-edit',
                executeMethod: function () { openCurrentItem(); },
                canExecuteMethod: function () { return true; }
            }
        ];

        function openCurrentItem() {
            var newBlade = {
                id: 'associationDetail',
                itemId: blade.currentEntity.associatedObjectId,
                catalog: blade.catalog,
                currentEntityId: blade.currentEntity.associatedObjectId
            };
            if (blade.currentEntity.associatedObjectType === 'product') {
                newBlade.controller = 'virtoCommerce.catalogModule.itemDetailController';
                newBlade.template = 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html';
            }
            else if (blade.currentEntity.associatedObjectType === 'category') {
                newBlade.controller = 'virtoCommerce.catalogModule.categoryDetailController';
                newBlade.template = 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/category-detail.tpl.html';
            }
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.openDictionarySettingManagement = function () {
            var newBlade = {
                id: 'settingDetailChild',
                isApiSave: true,
                currentEntityId: 'Catalog.AssociationGroups',
                parentRefresh: function (data) { $scope.associationGroups = data; },
                controller: 'platformWebApp.settingDictionaryController',
                template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.setForm = function (form) { $scope.formScope = form; };

        $scope.saveChanges = function () {
            angular.copy(blade.currentEntity, blade.origEntity);
            if (_.any(blade.currentEntity.tags)) {
                blade.origEntity.tags = _.map(blade.currentEntity.tags, function (x) {
                    return x.value;
                });
            }
            $scope.bladeClose();
        };

        initialize(blade.origEntity);
    }]);