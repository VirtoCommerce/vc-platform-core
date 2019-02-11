angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.publicationDetailController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.contentPublications', 'platformWebApp.bladeNavigationService', 'virtoCommerce.coreModule.common.dynamicExpressionService', 'virtoCommerce.storeModule.stores', 'platformWebApp.dialogService', function ($scope, contentPublications, bladeNavigationService, dynamicExpressionService, stores, dialogService) {
    var blade = $scope.blade;
    blade.updatePermission = 'marketing:update';

    blade.initializeBlade = function () {
        if (!blade.isNew) {
            contentPublications.get({ id: blade.currentEntity.id }, function (data) {
                initializeBlade(data);

                blade.toolbarCommands = [
				    {
				        name: "platform.commands.save", icon: 'fa fa-save',
				        executeMethod: function () {
				            blade.saveChanges();
				        },
				        canExecuteMethod: function () {
				            return blade.checkDifferense();
				        },
				        permission: blade.updatePermission
				    },
				    {
				        name: "platform.commands.reset", icon: 'fa fa-undo',
				        executeMethod: function () {
				            angular.copy(blade.origEntity, blade.currentEntity);
				        },
				        canExecuteMethod: function () {
				            return blade.checkDifferense();
				        },
				        permission: blade.updatePermission
				    },
				    {
				        name: "platform.commands.delete", icon: 'fa fa-trash-o',
				        executeMethod: function () {
				            var dialog = {
				                id: "confirmDeleteContentItem",
				                title: "marketing.dialogs.publication-delete.title",
				                message: "marketing.dialogs.publication-delete.message",
				                callback: function (remove) {
				                    if (remove) {
				                        blade.isLoading = true;
				                        contentPublications.delete({ ids: [blade.currentEntity.id] }, function () {
				                            blade.parentBlade.initialize();
				                            bladeNavigationService.closeBlade(blade);
				                        });
				                    }
				                }
				            };
				            dialogService.showConfirmationDialog(dialog);
				        },
				        canExecuteMethod: function () { return true; },
				        permission: blade.updatePermission
				    }
                ];
            });
        }
        else {
            contentPublications.getNew(initializeBlade);
        }
    }

    function initializeBlade(data) {
        if (data.dynamicExpression) {
            _.each(data.dynamicExpression.children, extendElementBlock);
            groupAvailableChildren(data.dynamicExpression.children[0]);
        }

        blade.currentEntity = data;
        blade.origEntity = angular.copy(blade.currentEntity);
        blade.isLoading = false;
        $scope.$watch('blade.currentEntity', blade.autogenerateName, true);
    }

    blade.addPlaceholders = function () {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            blade.selectedNodeId = 'addPlaceholders';
            var newBlade = {
                id: 'publishing_add_placeholders',
                title: 'marketing.blades.publishing.add-placeholders.title',
                subtitle: 'marketing.blades.publishing.add-placeholders.subtitle',
                publication: blade.currentEntity,
                controller: 'virtoCommerce.marketingModule.addPublishingPlaceholdersStepController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/publishing/add-placeholders.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    }

    blade.addContentItems = function () {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            blade.selectedNodeId = 'addContentItems';
            var newBlade = {
                id: 'publishing_add_content_items',
                title: 'marketing.blades.publishing.add-content-items.title',
                subtitle: 'marketing.blades.publishing.add-content-items.subtitle',
                publication: blade.currentEntity,
                controller: 'virtoCommerce.marketingModule.addPublishingContentItemsStepController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/dynamicContent/blades/publishing/add-content-items.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        });
    }

    blade.saveChanges = function () {
        bladeNavigationService.closeChildrenBlades(blade, function () {
            blade.isLoading = true;
            if (blade.currentEntity.dynamicExpression) {
                blade.currentEntity.dynamicExpression.availableChildren = undefined;
                _.each(blade.currentEntity.dynamicExpression.children, stripOffUiInformation);
            }

            if (blade.isNew) {
                contentPublications.save({}, blade.currentEntity, function (data) {
                    blade.currentEntity = data;
                    blade.origEntity = angular.copy(data);

                    blade.isNew = false;
                    blade.initializeBlade();
                    blade.parentBlade.initialize();
                });
            }
            else {
                contentPublications.update({}, blade.currentEntity, function (data) {
                    blade.currentEntity = data;
                    blade.origEntity = angular.copy(data);

                    blade.isNew = false;
                    blade.initializeBlade();
                    blade.parentBlade.initialize();
                }, function (error) {
                    bladeNavigationService.setError(error.statusText, blade);
                });
            }
        });
    }

    blade.availableSave = function () {
        return !$scope.formScope.$invalid &&
			blade.currentEntity.contentItems.length > 0 &&
			blade.currentEntity.contentPlaces.length > 0;
    };
    
    blade.checkDifferense = function () {
        var retVal = !$scope.formScope.$invalid &&
							blade.currentEntity.contentItems.length > 0 &&
							blade.currentEntity.contentPlaces.length > 0;

        if (retVal) {
            retVal = !angular.equals(blade.currentEntity, blade.origEntity);

            if (!retVal) {
                var ciIdse = blade.currentEntity.contentItems.map(function (v) {
                    return v.id;
                });

                var ciIdsoe = blade.origEntity.contentItems.map(function (v) {
                    return v.id;
                });

                retVal = _.intersection(ciIdse, ciIdsoe).length < Math.max(ciIdse.length, ciIdsoe.length);
            }

            if (!retVal) {
                var cpIdse = blade.currentEntity.contentPlaces.map(function (v) {
                    return v.id;
                });

                var cpIdsoe = blade.origEntity.contentPlaces.map(function (v) {
                    return v.id;
                });

                retVal = _.intersection(cpIdse, cpIdsoe).length < Math.max(cpIdse.length, cpIdsoe.length);
            }
        }

        return retVal;
    };

    $scope.setForm = function (form) { $scope.formScope = form; };

    // datepicker
    $scope.datepickers = {
        endDate: false,
        startDate: false,
    }
    $scope.today = new Date();

    $scope.open = function ($event, which) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.datepickers[which] = true;
    };

    $scope.dateOptions = {
        'year-format': "'yyyy'",
        'starting-day': 1
    };

    $scope.formats = ['shortDate', 'dd-MMMM-yyyy', 'yyyy/MM/dd'];
    $scope.format = $scope.formats[0];

    blade.headIcon = 'fa-paperclip';

    // Dynamic ExpressionBlock
    function extendElementBlock(expressionBlock) {
        var retVal = dynamicExpressionService.expressions[expressionBlock.id];
        if (!retVal) {
            retVal = { displayName: 'unknown element: ' + expressionBlock.id };
        }

        _.extend(expressionBlock, retVal);

        if (!expressionBlock.children) {
            expressionBlock.children = [];
        }

        _.each(expressionBlock.children, extendElementBlock);
        _.each(expressionBlock.availableChildren, extendElementBlock);
        return expressionBlock;
    }

    function groupAvailableChildren(expressionBlock) {
        results = _.groupBy(expressionBlock.availableChildren, 'groupName');
        expressionBlock.availableChildren = _.map(results, function (items, key) { return { displayName: key, subitems: items }; });
    }

    function stripOffUiInformation(expressionElement) {
        expressionElement.availableChildren = undefined;
        expressionElement.displayName = undefined;
        expressionElement.getValidationError = undefined;
        expressionElement.groupName = undefined;
        expressionElement.newChildLabel = undefined;
        expressionElement.templateURL = undefined;

        _.each(expressionElement.children, stripOffUiInformation);
    };

    String.prototype.trimLeft = function (charlist) {
        if (charlist === undefined)
            charlist = "\s";

        return this.replace(new RegExp("^[" + charlist + "]+"), "");
    };
    String.prototype.trimRight = function (charlist) {
        if (charlist === undefined)
            charlist = "\s";

        return this.replace(new RegExp("[" + charlist + "]+$"), "");
    };

    blade.focusNameInput = false;
    blade.autogenerateName = function () {
        if (!blade.focusNameInput) {


            var placeholderPublicationNamePart = '';
            var contentItemPublicationNamePart = '';

            if (!angular.isUndefined(blade.currentEntity)) {
                if (!angular.isUndefined(blade.currentEntity.contentPlaces) && blade.currentEntity.contentPlaces.length == 1) {
                    placeholderPublicationNamePart = blade.currentEntity.contentPlaces[0].name;
                }

                if (!angular.isUndefined(blade.currentEntity.contentItems) && blade.currentEntity.contentItems.length == 1) {
                    contentItemPublicationNamePart = blade.currentEntity.contentItems[0].name;
                }
            }

            var newName = (placeholderPublicationNamePart + '_' + contentItemPublicationNamePart).trimLeft('_').trimRight('_');

            //Here we disable replacing of entered text
            if (!angular.isUndefined(blade.currentEntity.name) && blade.currentEntity.name === '') {
                if (!angular.isUndefined(blade.currentEntity.name) && blade.currentEntity.name !== null && blade.currentEntity.name !== '' && newName !== '_') {
                    if (blade.currentEntity.name.indexOf(placeholderPublicationNamePart) >= 0 || blade.currentEntity.name.indexOf(contentItemPublicationNamePart) >= 0) {
                        blade.currentEntity.name = newName;
                    }
                }
                else if (newName !== '_') {
                    blade.currentEntity.name = newName;
                }
            }
        }
    }

    blade.initializeBlade();
    $scope.stores = stores.query();
}]);