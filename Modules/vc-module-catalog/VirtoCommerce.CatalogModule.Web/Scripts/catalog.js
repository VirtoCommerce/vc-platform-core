//Call this to register our module to main application
var catalogsModuleName = "virtoCommerce.catalogModule";

if (AppDependencies != undefined) {
    AppDependencies.push(catalogsModuleName);
}

angular.module(catalogsModuleName, ['ui.grid.validate', 'ui.grid.infiniteScroll'])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('workspace.catalog', {
                url: '/catalog',
                templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                controller: [
                    '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {

                        var blade = {
                            id: 'categories',
                            title: 'catalog.blades.catalogs-list.title',
                            breadcrumbs: [],
                            subtitle: 'catalog.blades.catalogs-list.subtitle',
                            controller: 'virtoCommerce.catalogModule.catalogsListController',
                            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/catalogs-list.tpl.html',
                            isClosingDisabled: true
                        };
                        bladeNavigationService.showBlade(blade);
                        $scope.moduleName = 'vc-catalog';
                    }
                ]
            });
    }])

    // define search filters to be accessible platform-wide
    .factory('virtoCommerce.catalogModule.predefinedSearchFilters', ['$localStorage', function ($localStorage) {
        $localStorage.catalogSearchFilters = $localStorage.catalogSearchFilters || [];

        return {
            register: function (currentFiltersUpdateTime, currentFiltersStorageKey, newFilters) {
                _.each(newFilters, function (newFilter) {
                    var found = _.find($localStorage.catalogSearchFilters, function (x) {
                        return x.id == newFilter.id;
                    });
                    if (found) {
                        if (found && (!found.lastUpdateTime || found.lastUpdateTime < currentFiltersUpdateTime)) {
                            angular.copy(newFilter, found);
                        }
                    } else if (!$localStorage[currentFiltersStorageKey] || $localStorage[currentFiltersStorageKey] < currentFiltersUpdateTime) {
                        $localStorage.catalogSearchFilters.splice(0, 0, newFilter);
                    }
                });

                $localStorage[currentFiltersStorageKey] = currentFiltersUpdateTime;
            }
        };
    }])

    .factory('virtoCommerce.catalogModule.itemTypesResolverService', function () {
        return {
            objects: [],
            registerType: function (itemTypeDefinition) {
                this.objects.push(itemTypeDefinition);
            },
            resolve: function (type) {
                return _.findWhere(this.objects, { productType: type });
            }
        };
    })

    .factory('virtoCommerce.catalogModule.catalogImagesFolderPathHelper', [function () {
        return {
            getImagesFolderPath: function (catalogId, code) {
                var catalogShortName = catalogId.length > 5 ? catalogId.substring(0, 5) : catalogId;
                return catalogShortName + '/' + code;
            }
        };
    }])
    .run(
        ['platformWebApp.authService', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.catalogImportService', 'virtoCommerce.catalogModule.catalogExportService', 'platformWebApp.permissionScopeResolver', 'virtoCommerce.catalogModule.catalogs', 'virtoCommerce.catalogModule.predefinedSearchFilters', 'platformWebApp.metaFormsService', 'virtoCommerce.catalogModule.itemTypesResolverService', '$http', '$compile', 'virtoCommerce.exportModule.genericViewerItemService', 
            function (authService, mainMenuService, widgetService, $state, pushNotificationTemplateResolver, bladeNavigationService, catalogImportService, catalogExportService, scopeResolver, catalogs, predefinedSearchFilters, metaFormsService, itemTypesResolverService, $http, $compile, genericViewerItemService) {

            //Register module in main menu
            var menuItem = {
                path: 'browse/catalog',
                icon: 'fa fa-folder',
                title: 'catalog.main-menu-title',
                priority: 20,
                action: function () { $state.go('workspace.catalog'); },
                permission: 'catalog:access'
            };
            mainMenuService.addMenuItem(menuItem);


            //Register image widget
            var entryImageWidget = {
                controller: 'virtoCommerce.catalogModule.catalogEntryImageWidgetController',
                size: [2, 2],
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/catalogEntryImageWidget.tpl.html'
            };
            widgetService.registerWidget(entryImageWidget, 'itemDetail');

            //Register item property widget
            var itemPropertyWidget = {
                controller: 'virtoCommerce.catalogModule.itemPropertyWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/itemPropertyWidget.tpl.html'
            };
            widgetService.registerWidget(itemPropertyWidget, 'itemDetail');

            //Register item associations widget
            var itemAssociationsWidget = {
                controller: 'virtoCommerce.catalogModule.itemAssociationsWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/itemAssociationsWidget.tpl.html'
            };
            widgetService.registerWidget(itemAssociationsWidget, 'itemDetail');

            //Register item seo widget
            var itemSeoWidget = {
                controller: 'virtoCommerce.coreModule.seo.seoWidgetController',
                template: 'Modules/$(VirtoCommerce.Core)/Scripts/SEO/widgets/seoWidget.tpl.html',
                objectType: 'CatalogProduct',
                getDefaultContainerId: function (blade) { return undefined; },
                getLanguages: function (blade) { return _.pluck(blade.catalog.languages, 'languageCode'); }
            };
            widgetService.registerWidget(itemSeoWidget, 'itemDetail');



            //Register dimensions widget
            var dimensionsWidget = {
                controller: 'virtoCommerce.catalogModule.itemDimensionWidgetController',
                isVisible: function (blade) { return blade.productType == 'Physical'; },
                size: [2, 1],
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/itemDimensionWidget.tpl.html'
            };
            widgetService.registerWidget(dimensionsWidget, 'itemDetail');
            //Register item editorialReview widget
            var editorialReviewWidget = {
                controller: 'virtoCommerce.catalogModule.editorialReviewWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/editorialReviewWidget.tpl.html'
            };
            widgetService.registerWidget(editorialReviewWidget, 'itemDetail');

            //Register variation widget
            var variationWidget = {
                controller: 'virtoCommerce.catalogModule.itemVariationWidgetController',
                isVisible: function (blade) { return blade.id !== 'variationDetail'; },
                size: [1, 1],
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/itemVariationWidget.tpl.html'
            };
            widgetService.registerWidget(variationWidget, 'itemDetail');
            //Register asset widget
            var itemAssetWidget = {
                controller: 'virtoCommerce.catalogModule.itemAssetWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/itemAssetWidget.tpl.html'
            };
            widgetService.registerWidget(itemAssetWidget, 'itemDetail');

            //Register widgets to categoryDetail
            widgetService.registerWidget(entryImageWidget, 'categoryDetail');

            var categoryPropertyWidget = {
                controller: 'virtoCommerce.catalogModule.categoryPropertyWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/categoryPropertyWidget.tpl.html'
            };
            widgetService.registerWidget(categoryPropertyWidget, 'categoryDetail');

            //Register category seo widget
            var categorySeoWidget = {
                controller: 'virtoCommerce.coreModule.seo.seoWidgetController',
                template: 'Modules/$(VirtoCommerce.Core)/Scripts/SEO/widgets/seoWidget.tpl.html',
                objectType: 'Category',
                getDefaultContainerId: function (blade) { return undefined; },
                getLanguages: function (blade) { return _.pluck(blade.catalog.languages, 'languageCode'); }
            };
            widgetService.registerWidget(categorySeoWidget, 'categoryDetail');

            //Register catalog widgets
            var catalogLanguagesWidget = {
                controller: 'virtoCommerce.catalogModule.catalogLanguagesWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/catalogLanguagesWidget.tpl.html'
            };
            widgetService.registerWidget(catalogLanguagesWidget, 'catalogDetail');

            var catalogPropertyWidget = {
                isVisible: function (blade) { return !blade.isNew; },
                controller: 'virtoCommerce.catalogModule.catalogPropertyWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/catalogPropertyWidget.tpl.html'
            };
            widgetService.registerWidget(catalogPropertyWidget, 'catalogDetail');


            //Security scopes
            //Register permission scopes templates used for scope bounded definition in role management ui

            var catalogSelectScope = {
                type: 'SelectedCatalogScope',
                title: 'catalog.permissions.catalog-scope.title',
                selectFn: function (blade, callback) {
                    var newBlade = {
                        id: 'catalog-pick',
                        title: this.title,
                        subtitle: 'catalog.permissions.catalog-scope.blade.subtitle',
                        currentEntity: this,
                        onChangesConfirmedFn: callback,
                        dataPromise: catalogs.query().$promise,
                        controller: 'platformWebApp.security.scopeValuePickFromSimpleListController',
                        template: '$(Platform)/Scripts/app/security/blades/common/scope-value-pick-from-simple-list.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                }
            };
            scopeResolver.register(catalogSelectScope);

            var categorySelectScope = {
                type: 'CatalogSelectedCategoryScope',
                title: 'catalog.permissions.category-scope.title',
                selectFn: function (blade, callback) {
                    var selectedListItems = _.map(this.assignedScopes, function (x) { return { id: x.scope, name: x.label }; });
                    var options = {
                        showCheckingMultiple: false,
                        allowCheckingItem: false,
                        allowCheckingCategory: true,
                        selectedItemIds: _.map(this.assignedScopes, function (x) { return x.scope; }),
                        checkItemFn: function (listItem, isSelected) {
                            if (isSelected) {
                                if (_.all(selectedListItems, function (x) { return x.id != listItem.id; })) {
                                    selectedListItems.push(listItem);
                                }
                            }
                            else {
                                selectedListItems = _.reject(selectedListItems, function (x) { return x.id == listItem.id; });
                            }
                        }
                    };
                    var scopeOriginal = this.scopeOriginal;
                    var newBlade = {
                        id: "CatalogItemsSelect",
                        title: "catalog.blades.catalog-items-select.title",
                        controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
                        template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
                        options: options,
                        breadcrumbs: [],
                        toolbarCommands: [
                            {
                                name: "platform.commands.confirm",
                                icon: 'fa fa-plus',
                                executeMethod: function (blade) {
                                    var scopes = _.map(selectedListItems, function (x) {
                                        return angular.extend({ scope: x.id, label: x.name }, scopeOriginal);
                                    });
                                    callback(scopes);
                                    bladeNavigationService.closeBlade(blade);

                                },
                                canExecuteMethod: function () {
                                    return selectedListItems.length > 0;
                                }
                            }]
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                }
            };
            scopeResolver.register(categorySelectScope);


            // register WIDGETS
            var indexWidget = {
                controller: 'virtoCommerce.searchModule.indexWidgetController',
                // size: [3, 1],
                template: 'Modules/$(VirtoCommerce.Search)/Scripts/widgets/index-widget.tpl.html'
            };

            // integration: index in product details
            var widgetToRegister = angular.extend({}, indexWidget, { documentType: 'Product' });
            widgetService.registerWidget(widgetToRegister, 'itemDetail');
            // integration: index in CATEGORY details
            widgetToRegister = angular.extend({}, indexWidget, { documentType: 'Category' });
            widgetService.registerWidget(widgetToRegister, 'categoryDetail');

            // Aggregation properties in store details
            widgetService.registerWidget({
                controller: 'virtoCommerce.catalogModule.aggregationPropertiesWidgetController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/widgets/aggregationPropertiesWidget.tpl.html'
            }, 'storeDetail');

            // predefine search filters for catalog search
            predefinedSearchFilters.register(1477584000000, 'catalogSearchFiltersDate', [
                { name: 'catalog.blades.categories-items-list.labels.filter-new' },
                { keyword: '', searchInVariations: true, id: 5, name: 'catalog.blades.categories-items-list.labels.filter-display-variations' },
                { keyword: 'is:hidden', id: 4, name: 'catalog.blades.categories-items-list.labels.filter-notActive' },
                { keyword: 'price_usd:[100 TO 200]', id: 3, name: 'catalog.blades.categories-items-list.labels.filter-priceRange' },
                { keyword: 'is:priced', id: 2, name: 'catalog.blades.categories-items-list.labels.filter-withPrice' },
                { keyword: 'is:unpriced', id: 1, name: 'catalog.blades.categories-items-list.labels.filter-priceless' }
            ]);

            // register item types
            itemTypesResolverService.registerType({
                itemType: 'catalog.blades.categories-items-add.menu.physical-product.title',
                description: 'catalog.blades.categories-items-add.menu.physical-product.description',
                productType: 'Physical',
                icon: 'fa-truck'
            });
            itemTypesResolverService.registerType({
                itemType: 'catalog.blades.categories-items-add.menu.digital-product.title',
                description: 'catalog.blades.categories-items-add.menu.digital-product.description',
                productType: 'Digital',
                icon: 'fa-file-archive-o'
            });

            //meta-form used only for external extensions 
            //We did not include the default product fields in meta-form, because the resulting form looks ugly
            //TODO: need to improve meta-form to support more flexible layout management
            //metaFormsService.registerMetaFields("productDetail", metafieldsDefinitions.productMetafields);
            //metaFormsService.registerMetaFields("categoryDetail", metafieldsDefinitions.categoryMetafields);

            //
            metaFormsService.registerMetaFields('VirtoCommerce.CatalogModule.Core.Model.Export.ExportableProduct' + 'ExportFilter', [
                {
                    name: 'catalogSelector',
                    title: "catalog.selectors.titles.catalogs",
                    templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/selectors/catalog-selector.tpl.html'
                },
                {
                    name: 'categorySelector',
                    title: "catalog.selectors.titles.categories",
                    templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/selectors/category-selector.tpl.html'
                },
                {
                    name: 'searchInVariations',
                    title: "catalog.selectors.titles.search-in-variations",
                    valueType: "Boolean"
                },
                {
                    name: 'searchInChildren',
                    title: "catalog.selectors.titles.search-in-children",
                    valueType: "Boolean"
                }

            ]);

            metaFormsService.registerMetaFields('VirtoCommerce.CatalogModule.Core.Model.Export.ExportableCatalogFull' + 'ExportFilter', [
                {
                    name: 'catalogSelector',
                    title: "catalog.selectors.titles.catalogs",
                    templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/selectors/catalog-selector.tpl.html'
                }
            ]);

            genericViewerItemService.registerViewer('CatalogProduct', function (item) {
                var itemCopy = angular.copy(item);

                return {
                    id: "itemmDetail",
                    itemId: itemCopy.id,
                    productType: itemCopy.productType,
                    title: itemCopy.name,
                    controller: 'virtoCommerce.catalogModule.itemDetailController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/item-detail.tpl.html'
                };
            });

            $http.get('Modules/$(VirtoCommerce.Catalog)/Scripts/directives/itemSearch.tpl.html').then(function (response) {
                // compile the response, which will put stuff into the cache
                $compile(response.data);
            });

            catalogExportService.register({
                name: 'Generic Export',
                description: 'Export products filtered by catalogs or categories to JSON or CSV',
                icon: 'fa-fw fa fa-database',
                controller: 'virtoCommerce.exportModule.exportSettingsController',
                template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/export-settings.tpl.html',
                id: 'catalogGenericExport',
                title: 'catalog.blades.exporter.productTitle',
                subtitle: 'catalog.blades.exporter.productSubtitle',
                isNew: true,
                onInitialize: function (newBlade) {
                    var exportDataRequest = {
                        exportTypeName: 'VirtoCommerce.CatalogModule.Core.Model.Export.ExportableProduct',
                        dataQuery: {
                            exportTypeName: 'ProductExportDataQuery',
                            categoryIds: _.pluck(newBlade.selectedCategories, 'id'),
                            objectIds: _.pluck(newBlade.selectedProducts, 'id'),
                            catalogIds: [newBlade.catalog.id],
                            searchInChildren: true,
                            isAllSelected: true
                        }
                    };
                    newBlade.exportDataRequest = exportDataRequest;
                    newBlade.totalItemsCount = (newBlade.selectedProducts || []).length;
                }
            });
        }]);
