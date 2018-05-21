angular.module('virtoCommerce.coreModule.packageType')
.controller('virtoCommerce.coreModule.packageType.packageTypeDetailController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.coreModule.packageType.packageTypeApi', 'platformWebApp.settings',
    function ($scope, dialogService, bladeNavigationService, packageTypeApi, settings) {
    	var blade = $scope.blade;

    	$scope.saveChanges = function () {
    		blade.isLoading = true;

    		if (blade.isNew) {
    			blade.currentEntity.id = blade.currentEntity.name;
    			packageTypeApi.save(blade.currentEntity, function () {
    				angular.copy(blade.currentEntity, blade.origEntity);
    				$scope.bladeClose();
    				blade.parentBlade.refresh(true);
    			},function (error) {
    				bladeNavigationService.setError('Error ' + error.status, blade);
    			});
    		} else {
    			packageTypeApi.update(blade.currentEntity, function (data) {
    				angular.copy(blade.currentEntity, blade.origEntity);
    				$scope.bladeClose();
    				blade.parentBlade.refresh(true);
    			}, function (error) {
    				bladeNavigationService.setError('Error ' + error.status, blade);
    			});
    		}

    	};

    	function initializeBlade(data) {
    		if (blade.isNew) data = { };
    		$scope.measureUnits = settings.getValues({ id: 'VirtoCommerce.Core.General.MeasureUnits' });

    		blade.currentEntity = angular.copy(data);
    		blade.origEntity = data;
    		blade.isLoading = false;

    		blade.title = blade.isNew ? 'core.blades.package-type-detail.new-title' : data.name;
    		blade.subtitle = blade.isNew ? 'core.blades.package-type-detail.new-subtitle' : 'core.blades.package-type-detail.subtitle';
    	};

    	$scope.openMeasureUnitsDictionarySettingManagement = function (setting) {
    		var newBlade = {
    			id: 'settingDetailChild',
    			currentEntityId: 'VirtoCommerce.Core.General.MeasureUnits',
    			parentRefresh: function (data) { $scope.measureUnits = data; },
    			isApiSave: true,
    			controller: 'platformWebApp.settingDictionaryController',
    			template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
    		};
    		bladeNavigationService.showBlade(newBlade, blade);
    	};

    	var formScope;
    	$scope.setForm = function (form) {
    		formScope = form;
    	}

    	function isDirty() {
    		return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    	}

    	function canSave() {
    		return isDirty() && formScope && formScope.$valid;
    	}

    	blade.headIcon = 'fa-money';

    	if (!blade.isNew)
    		blade.toolbarCommands = [
                {
                	name: "platform.commands.save", icon: 'fa fa-save',
                	executeMethod: $scope.saveChanges,
                	canExecuteMethod: canSave,
                	permission: 'core:packageType:update'
                },
                {
                	name: "platform.commands.reset", icon: 'fa fa-undo',
                	executeMethod: function () {
                		angular.copy(blade.origEntity, blade.currentEntity);
                	},
                	canExecuteMethod: isDirty
                },
                {
                	name: "platform.commands.delete", icon: 'fa fa-trash-o',
                	executeMethod: deleteEntry,
                	canExecuteMethod: function () {
                		return !blade.origEntity.isPrimary;
                	},
                	permission: 'core:packageType:delete'
                }
    		];

    	function deleteEntry() {
    		var dialog = {
    			id: "confirmDelete",
    			title: "core.dialogs.package-type-delete.title",
    			message: "core.dialogs.package-type-delete.message",
    			callback: function (remove) {
    				if (remove) {
    					blade.isLoading = true;
    					packageTypeApi.remove({ ids: blade.currentEntity.id }, function () {
    						angular.copy(blade.currentEntity, blade.origEntity);
    						$scope.bladeClose();
    						blade.parentBlade.refresh(true);
    					}, function (error) {
    						bladeNavigationService.setError('Error ' + error.status, blade);
    					});
    				}
    			}
    		}
    		dialogService.showConfirmationDialog(dialog);
    	}

    	blade.onClose = function (closeCallback) {
    		bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "core.dialogs.package-type-save.title", "core.dialogs.package-type-save.message");
    	};

    	// actions on load        
    	initializeBlade(blade.data);
    }]);
