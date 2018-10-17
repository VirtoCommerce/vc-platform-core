angular.module('virtoCommerce.customerModule')
.controller('virtoCommerce.customerModule.memberDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.customerModule.members', 'platformWebApp.dynamicProperties.api', 'virtoCommerce.customerModule.organizations', function ($scope, bladeNavigationService, members, dynamicPropertiesApi, organizationsResource) {
    var blade = $scope.blade;
    blade.updatePermission = 'customer:update';
    blade.currentEntityId = blade.currentEntity.id;

    blade.refresh = function (parentRefresh) {
        if (blade.isNew) {
            blade.currentEntity = angular.extend({
                dynamicProperties: [],
                addresses: [],
                phones: [],
                emails: []
            }, blade.currentEntity);
        } else {
            blade.isLoading = true;

            members.get({ id: blade.currentEntity.id }, initializeBlade);

            if (parentRefresh) {
                blade.parentBlade.refresh(true);
            }
        }
    };

    blade.fillDynamicProperties = function () {
        dynamicPropertiesApi.query({ id: blade.memberTypeDefinition.fullTypeName }, function (results) {
            _.each(results, function (x) {
                x.displayNames = undefined;
                x.values = [];
            });
            blade.currentEntity.dynamicProperties = results;
            initializeBlade(blade.currentEntity);
        });
    };

    function initializeBlade(data) {
        blade.currentEntity = angular.copy(data);
        blade.origEntity = data;
        blade.customInitialize();
        blade.isLoading = false;
    }

    // base function to override as needed
    blade.customInitialize = function () {
        if (!blade.isNew) {
            blade.title = blade.currentEntity.name;
        }
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && !blade.isNew && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope && $scope.formScope.$valid;
    }

    $scope.saveChanges = function () {
        blade.isLoading = true;

        if (blade.isNew) {
            members.save(blade.currentEntity,
                function () {
                    blade.parentBlade.refresh(true);
                    blade.origEntity = blade.currentEntity;
                    $scope.bladeClose();
                });
        } else {
            members.update(blade.currentEntity,
                function () { blade.refresh(true); });
        }
    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "customer.dialogs.member-save.title", "customer.dialogs.member-save.message");
    };

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

    blade.headIcon = blade.memberTypeDefinition.icon;

    // pageSize amount must be enough to show scrollbar in dropdown list container.
    // If scrollbar doesn't appear auto loading won't work.
    $scope.pageSize = 50;

    $scope.fetchOrganizations = function ($select) {
        $select.page = 0;
        $scope.organizations = [];
        loadCustomerOrganizations();
        $scope.fetchNextOrganizations($select);
    }

    $scope.fetchNextOrganizations = function ($select) {
        members.search(
            {
                memberType: 'Organization',
                SearchPhrase: $select.search,
                deepSearch: true,
                take: $scope.pageSize,
                skip: $select.page * $scope.pageSize
            },
            function (data) {
                joinOrganizations(data.results);
                $select.page++;
            });
    };

    function loadCustomerOrganizations() {
        if (blade.currentEntity.organizations && blade.currentEntity.organizations.length > 0) {
            organizationsResource.getByIds({ ids: blade.currentEntity.organizations }, function (data) {
                    joinOrganizations(data);
                }
            );
        };
    };

    function joinOrganizations(organizations) {
        $scope.organizations = $scope.organizations.concat(organizations);

    };

    blade.refresh(false);
}]);