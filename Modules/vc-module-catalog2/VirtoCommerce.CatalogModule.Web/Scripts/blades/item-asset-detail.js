angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.itemAssetController', ['$scope', '$translate', 'platformWebApp.bladeNavigationService', '$filter', 'platformWebApp.uiGridHelper', function ($scope, $translate, bladeNavigationService, $filter, uiGridHelper) {
        var blade = $scope.blade;

        blade.headIcon = 'fa-chain';

        blade.toolbarCommands = [
            {
                name: 'platform.commands.remove',
                icon: 'fa fa-trash-o',
                executeMethod: function () { $scope.removeAction(); },
                canExecuteMethod: function () {
                    var retVal = false;
                    if (blade.currentEntities && $scope.gridApi) {
                        retVal = $scope.gridApi.selection.getSelectedRows().length > 0;
                    }
                    return retVal;
                }
            },
            {
                name: "Add",
                icon: 'fa fa-plus',
                executeMethod: function () {
                    var newBlade = {
                        title: "catalog.blades.asset-upload.title",
                        item: blade.item,
                        onSelect: linkAssets,
                        controller: 'virtoCommerce.catalogModule.itemAssetAddController',
                        template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-asset-add.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                },
                canExecuteMethod: function () { return true; }
            },
            {
                name: "Link",
                icon: 'fa fa-link',
                executeMethod: function () {
                    var newBlade = {
                        title: 'catalog.blades.asset-select.title',
                        //folder: "catalog",
                        onSelect: linkAssets,
                        controller: 'platformWebApp.assets.assetSelectController'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                },
                canExecuteMethod: function () { return true; }
            }];

        function linkAssets(assets) {
            _.each(assets, function (asset) {
                var converted = angular.copy(asset);
                converted.mimeType = asset.contentType;
                blade.currentEntities.push(converted);
            });
        }

        blade.isLoading = false;

        blade.refresh = function (item) {
            initialize(item);
        }

        $scope.isValid = true;

        $scope.saveChanges = function () {
            blade.item.assets = blade.currentEntities;
            $scope.bladeClose();
        };

        function initialize(item) {
            blade.item = item;

            blade.title = item.name;
            blade.subtitle = 'catalog.widgets.itemAsset.blade-subtitle';

            blade.currentEntities = item.assets ? angular.copy(item.assets) : [];
        };

        $scope.toggleAssetSelect = function (e, asset) {
            if (e.ctrlKey === 1) {
                asset.selected = !asset.selected;
            } else {
                if (asset.selected) {
                    asset.selected = false;
                } else {
                    asset.selected = true;
                }
            }
        }

        $scope.removeAction = function (asset) {
            var idx = blade.currentEntities.indexOf(asset);
            if (idx >= 0) {
                blade.currentEntities.splice(idx, 1);
            }
        };

        $scope.copyUrl = function (data) {
            $translate('catalog.blades.item-asset-detail.labels.copy-url-prompt').then(function (promptMessage) {
                window.prompt(promptMessage, data.url);
            });
        }


        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions);
        };


        $scope.removeItem = function (image) {
            var idx = blade.currentEntities.indexOf(image);
            if (idx >= 0) {
                blade.currentEntities.splice(idx, 1);
            }
        };


        $scope.downloadUrl = function (image) {
            window.open(image.url, '_blank');
        }

        $scope.removeAction = function (selectedImages) {
            if (selectedImages == undefined) {
                selectedImages = $scope.gridApi.selection.getSelectedRows();
            }

            angular.forEach(selectedImages, function (image) {
                var idx = blade.currentEntities.indexOf(image);
                if (idx >= 0) {
                    blade.currentEntities.splice(idx, 1);
                }
            });
        };

        initialize(blade.item);

    }]);