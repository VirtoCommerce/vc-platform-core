angular.module('virtoCommerce.sitemapsModule')
.controller('virtoCommerce.sitemapsModule.sitemapItemsAddController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.sitemapsModule.sitemapApi', 'virtoCommerce.sitemapsModule.knownSitemapItemTypes', function ($scope, bladeNavigationService, sitemapApi, knownSitemapItemTypes) {
    var blade = $scope.blade;
    $scope.availableTypes = knownSitemapItemTypes.types;
    blade.isLoading = false;

    $scope.addItem = function (node) {
        var newBlade = angular.extend(node.newBlade, {
            id: node.newBlade.id || 'sitemapItem-details',
            selectedItems: [],
            storeId: blade.storeId,
            checkItemFn: updateSelectedItemsList,
            confirmChangesFn: saveNewSitemapItems
        });

        bladeNavigationService.showBlade(newBlade, blade.parentBlade);
    };

    function updateSelectedItemsList(selectedItems, listItem, isSelected) {
        if (isSelected) {
            if (_.all(selectedItems, function (x) { return x.id != listItem.id; })) {
                selectedItems.push(listItem);
            }
        } else {
            selectedItems = _.reject(selectedItems, function (x) { return x.id == listItem.id; });
        }
        blade.error = undefined;
        return selectedItems;
    }

    function saveNewSitemapItems(sitemapItems, currentBlade) {
        currentBlade.isLoading = true;

        sitemapApi.addSitemapItems({ sitemapId: blade.sitemap.id }, sitemapItems, function () {
            bladeNavigationService.closeBlade(currentBlade, blade.parentRefresh);
        });
    }

}])

.run(['virtoCommerce.sitemapsModule.knownSitemapItemTypes', function (knownSitemapItemTypes) {
    function itemToSitemapItem(item) {
        return {
            title: item.name,
            imageUrl: item.imageUrl,
            objectId: item.id,
            objectType: item.type || item.seoObjectType
        };
    }

    // register known item types
    var catalogItemSelectBlade = {
        id: 'addSitemapCatalogItems',
        title: 'sitemapsModule.blades.addCatalogItems.title',
        controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
        template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
        breadcrumbs: [],
        toolbarCommands: [{
            name: 'sitemapsModule.blades.addCatalogItems.toolbar.addSelected', icon: 'fa fa-plus',
            executeMethod: function (catalogBlade) {
                var sitemapItems = _.map(catalogItemSelectBlade.selectedItems, itemToSitemapItem);
                catalogItemSelectBlade.confirmChangesFn(sitemapItems, catalogBlade);
            },
            canExecuteMethod: function () { return _.any(catalogItemSelectBlade.selectedItems); }
        }],
        options: {
            allowCheckingCategory: true,
            checkItemFn: function (listItem, isSelected) {
                catalogItemSelectBlade.selectedItems = catalogItemSelectBlade.checkItemFn(catalogItemSelectBlade.selectedItems, listItem, isSelected);
            }
        }
    };
    knownSitemapItemTypes.registerType({
        type: 'CatalogItem',
        icon: 'fa-folder',
        newBlade: catalogItemSelectBlade
    });

    knownSitemapItemTypes.registerType({
        type: 'VendorItem',
        icon: 'fa-balance-scale',
        newBlade: {
            title: 'sitemapsModule.blades.addVendorItems.title',
            controller: 'virtoCommerce.customerModule.memberItemSelectController',
            template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/member-items-select.tpl.html',
            toolbarCommands: [{
                name: 'sitemapsModule.blades.addVendorItems.toolbar.addSelected', icon: 'fa fa-plus',
                executeMethod: function (vendorsBlade) {
                    var sitemapItems = _.map(vendorsBlade.selectedItems, itemToSitemapItem);
                    vendorsBlade.confirmChangesFn(sitemapItems, vendorsBlade);
                },
                canExecuteMethod: function (vendorsBlade) { return _.any(vendorsBlade.selectedItems); }
            }],
            options: {
                memberTypes: ['vendor']
            }
        },
    });

    knownSitemapItemTypes.registerType({
        type: 'CustomItem',
        icon: 'fa-link',
        newBlade: {
            id: 'addCustomItemBlade',
            title: 'sitemapsModule.blades.addCustomItem.title',
            controller: 'virtoCommerce.sitemapsModule.sitemapItemsAddCustomItemController',
            template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/sitemap-add-custom-item.tpl.html'
        }
    });

    var staticContentBlade = {
        id: 'addSitemapStaticContentItems',
        title: 'sitemapsModule.blades.addStaticContentItems.title',
        controller: 'virtoCommerce.sitemapsModule.staticContentItemSelectController',
        template: 'Modules/$(VirtoCommerce.Sitemaps)/Scripts/blades/static-content-items-select.tpl.html',
        headIcon: 'fa-code',
        breadcrumbs: null,
        currentEntity: {}
    }

    knownSitemapItemTypes.registerType({
        type: 'StaticContentItem',
        icon: 'fa-code',
        newBlade: staticContentBlade
    });
}])
// define known sitemap item types to be accessible platform-wide
.factory('virtoCommerce.sitemapsModule.knownSitemapItemTypes', ['platformWebApp.bladeNavigationService', function (bladeNavigationService) {
    return {
        types: [],
        registerType: function (typeDefinition) {
            this.types.push(typeDefinition);
        }
    };
}]);