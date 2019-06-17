angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.newProductWizardController', ['$scope', 'platformWebApp.bladeNavigationService', '$http', 'virtoCommerce.storeModule.stores', 'virtoCommerce.catalogModule.catalogImagesFolderPathHelper', function ($scope, bladeNavigationService, $http, stores, catalogImgHelper) {
    var blade = $scope.blade;
    blade.headIcon = blade.item.productType === 'Digital' ? 'fa fa-file-archive-o' : 'fa fa-truck';

    var initialName = blade.item.name ? blade.item.name : '';
    var lastGeneratedName = initialName;
    var storesPromise = stores.query().$promise;

    $scope.createItem = function () {
        blade.isLoading = true;

        blade.item.$update(null,
            function (dbItem) {
                blade.parentBlade.setSelectedItem(dbItem);
                blade.parentBlade.refresh();

                var newBlade = {
                    id: blade.id,
                    itemId: dbItem.id,
                    catalog: blade.catalog,
                    productType: dbItem.productType,
                    title: dbItem.name,
                    controller: 'virtoCommerce.catalogModule.itemDetailController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade.parentBlade);
            },
            function (error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); });
    };

    $scope.openBlade = function (type) {
        var newBlade = null;
        switch (type) {
            case 'properties':
                newBlade = {
                    id: "newProductProperties",
                    entityType: "product",
                    item: blade.item,
                    catalog: blade.catalog,
                    currentEntity: blade.item,
                    languages: _.pluck(blade.catalog.languages, 'languageCode'),
                    defaultLanguage: blade.catalog.defaultLanguage.languageCode,
                    propGroups: [{ title: 'catalog.properties.product', type: 'Product' }, { title: 'catalog.properties.variation', type: 'Variation' }],
                    controller: 'virtoCommerce.catalogModule.propertyListController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-list.tpl.html'
                };
                break;
            case 'images':
                newBlade = {
                    id: "newProductImages",
                    item: blade.item,
                    folderPath: catalogImgHelper.getImagesFolderPath($scope.blade.item.catalogId, $scope.blade.item.code),
                    catalog: blade.catalog,
                    controller: 'virtoCommerce.catalogModule.imagesController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/images.tpl.html'
                };
                break;
            case 'seo':
                initializeSEO(blade.item, function (seoInfo) {
                    storesPromise.then(function (promiseData) {
                        newBlade = {
                            id: 'seoDetails',
                            data: seoInfo,
                            isNew: !_.any(blade.item.seoInfos),
                            seoContainerObject: blade.item,
                            stores: promiseData,
                            catalog: blade.catalog,
                            languages: _.pluck(blade.catalog.languages, 'languageCode'),
                            updatePermission: 'catalog:create',
                            controller: 'virtoCommerce.coreModule.seo.seoDetailController',
                            template: 'Modules/$(VirtoCommerce.Core)/Scripts/SEO/blades/seo-detail.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    });
                });
                break;
            case 'review':
                newBlade = {
                    id: "newProductEditorialReviewsList",
                    item: blade.item,
                    catalog: blade.catalog,
                    controller: 'virtoCommerce.catalogModule.editorialReviewsListController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/editorialReviews-list.tpl.html'
                };
                break;
        }

        if (newBlade != null) {
            bladeNavigationService.showBlade(newBlade, blade);
        }
    }

    $scope.codeValidator = function (value) {
        var pattern = /[$+;=%{}[\]|@~!^*&()?'<>,]/;
        return !pattern.test(value);
    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    }

    $scope.getUnfilledProperties = function () {
        return _.filter(blade.item.properties, function (p) {
            return p && _.any(p.values) && p.values[0].value;
        });
    }

    function initializeSEO(item, callback) {
        if (_.any(item.seoInfos)) {
            callback(item.seoInfos[0]);
        } else {
            var retVal = { isActive: true };
            var stringForSlug = item.name;
            _.each(item.properties, function (prop) {
                _.each(prop.values, function (val) {
                    stringForSlug += ' ' + val.value;
                });
            });

            if (stringForSlug) {
                $http.get('api/catalog/getslug?text=' + stringForSlug)
                    .then(function (results) {
                        retVal.semanticUrl = results.data;
                        callback(retVal);
                    });
            } else
                callback(retVal);
        }
    }

    $scope.$watch('blade.item.properties', function (currentEntities) {
        // auto-generate item.name from property values if user didn't change it
        if ((lastGeneratedName == blade.item.name || (!lastGeneratedName && !blade.item.name))
            && _.any(blade.childrenBlades, function (x) { return x.controller === 'virtoCommerce.catalogModule.newProductWizardPropertiesController'; })) {
            lastGeneratedName = initialName;
            _.each(currentEntities, function (x) {
                if (_.any(x.values, function (val) { return val.value; })) {
                    var currVal = _.find(x.values, function (val) { return val.value; });
                    if (currVal) {
                        lastGeneratedName += (lastGeneratedName ? ', ' : '') + currVal.value;
                    }
                }
            });
            blade.item.name = lastGeneratedName;
        }
    });


    blade.isLoading = false;
}]);
