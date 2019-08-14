//Call this to register our module to main application
var moduleName = "virtoCommerce.orderModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [ /*TODO: Uncomment when migration of theses module will be done 'virtoCommerce.pricingModule'*/, 'virtoCommerce.customerModule', 'virtoCommerce.storeModule', 'virtoCommerce.inventoryModule'])
.config(
  ['$stateProvider', function ($stateProvider) {
      $stateProvider
          .state('workspace.orderModule', {
              url: '/orders',
              templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
              controller: [
                  '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                      var blade = {
                          id: 'orders',
                          title: 'orders.blades.customerOrder-list.title',
                          //subtitle: 'Manage Orders',
                          controller: 'virtoCommerce.orderModule.customerOrderListController',
                          isExpandable: true,
                          template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/customerOrder-list.tpl.html',
                          isClosingDisabled: true
                      };
                      bladeNavigationService.showBlade(blade);
                      //Need for isolate and prevent conflict module css to another modules 
                      //it value included in bladeContainer as ng-class='moduleName'
                      $scope.moduleName = "vc-order";
                  }
              ]
          });
  }]
)
// define known Operations to be accessible platform-wide
.factory('virtoCommerce.orderModule.knownOperations', ['platformWebApp.bladeNavigationService', function (bladeNavigationService) {
    var map = {};

    function registerOperation(op) {
        var copy = angular.copy(op);
        copy.detailBlade = angular.extend({
            id: 'operationDetail',
            knownChildrenOperations: [],
            metaFields: [],
            controller: 'virtoCommerce.orderModule.operationDetailController'
        }, copy.detailBlade);

        map[op.type] = copy;
    }

    function getOperation(type) {
        return map[type];
    }

    return {
        registerOperation: registerOperation,
        getOperation: getOperation
    };
}])
.run(
    ['$rootScope', '$http', '$compile', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', 'platformWebApp.bladeNavigationService', '$state', '$localStorage', 'virtoCommerce.orderModule.order_res_customerOrders', 'platformWebApp.permissionScopeResolver', 'virtoCommerce.storeModule.stores', 'virtoCommerce.orderModule.knownOperations', 'platformWebApp.authService',
        function ($rootScope, $http, $compile, mainMenuService, widgetService, bladeNavigationService, $state, $localStorage, customerOrders, scopeResolver, stores, knownOperations, authService) {
        //Register module in main menu
        var menuItem = {
            path: 'browse/orders',
            icon: 'fa fa-file-text',
            title: 'orders.main-menu-title',
            priority: 90,
            action: function () { $state.go('workspace.orderModule'); },
            permission: 'order:access'
        };
        mainMenuService.addMenuItem(menuItem);

        // register CustomerOrder, PaymentIn and Shipment types as known operations
        knownOperations.registerOperation({
            type: 'CustomerOrder',
            treeTemplateUrl: 'orderOperationDefault.tpl.html',
            detailBlade: {
                id: 'orderDetail',
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/customerOrder-detail.tpl.html',
                knownChildrenOperations: ['Shipment', 'PaymentIn'],
                metaFields: [
                    {
                        name: 'isApproved',
                        title: "orders.blades.customerOrder-detail.labels.approved",
                        valueType: "Boolean",
                        isVisibleFn: function (blade) {
                            return !blade.isNew;
                        }
                    },
                    {
                        name: 'employeeId',
                        title: "orders.blades.customerOrder-detail.labels.employee",
                        templateUrl: 'order-employeeSelector.html'
                    },
                    {
                        name: 'number',
                        isRequired: true,
                        title: "orders.blades.customerOrder-detail.labels.order-number",
                        valueType: "ShortText"
                    },
                    {
                        name: 'createdDate',
                        title: "orders.blades.customerOrder-detail.labels.from",
                        templateUrl: 'createdDate.html'
                    },
                    {
                        name: 'status',
                        templateUrl: 'statusSelector.html'
                    },
                    {
                        name: 'customerName',
                        title: "orders.blades.customerOrder-detail.labels.customer",
                        templateUrl: 'customerSelector.html'
                    },
                    {
                        name: 'discountAmount',
                        title: "orders.blades.customerOrder-items.labels.discount",
                        templateUrl: 'discountAmount.html'
                    },
                    {
                        name: 'storeId',
                        title: "orders.blades.customerOrder-detail.labels.store",
                        templateUrl: 'storeSelector.html'
                    }
                ]
            }
        });

        var paymentInOperation = {
            type: 'PaymentIn',
            description: 'orders.blades.newOperation-wizard.menu.payment-operation.description',
            // treeTemplateUrl: 'orderOperationDefault.tpl.html',
            detailBlade: {
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/payment-detail.tpl.html',
                metaFields: [
                    {
                        name: 'number',
                        isRequired: true,
                        title: "orders.blades.payment-detail.labels.payment-number",
                        valueType: "ShortText"
                    },
                    {
                        name: 'createdDate',
                        title: "orders.blades.payment-detail.labels.from",
                        templateUrl: 'createdDate.html'
                    },
                    {
                        name: 'price',
                        title: "orders.blades.payment-detail.labels.price",
                        templateUrl: 'price.html'
                    },
                    {
                        name: 'priceWithTax',
                        title: "orders.blades.payment-detail.labels.price-with-tax",
                        templateUrl: 'priceWithTax.html'
                    }
                ]
            }
        };
        knownOperations.registerOperation(paymentInOperation);

        var shipmentOperation = {
            type: 'Shipment',
            description: 'orders.blades.newOperation-wizard.menu.shipment-operation.description',
            detailBlade: {
                template: 'Modules/$(VirtoCommerce.Orders)/Scripts/blades/shipment-detail.tpl.html',
                metaFields: [
                    {
                        name: 'number',
                        isRequired: true,
                        title: "orders.blades.shipment-detail.labels.shipment-number",
                        valueType: "ShortText"
                    },
                    {
                        name: 'createdDate',
                        title: "orders.blades.shipment-detail.labels.from",
                        templateUrl: 'createdDate.html'
                    },
                    {
                        name: 'status',
                        templateUrl: 'statusSelector.html'
                    },
                    {
                        name: 'employeeId',
                        title: "orders.blades.shipment-detail.labels.employee",
                        templateUrl: 'shipment-employeeSelector.html'
                    },
                    {
                        name: 'price',
                        title: "orders.blades.shipment-detail.labels.price",
                        templateUrl: 'price.html'
                    },
                    {
                        name: 'priceWithTax',
                        title: "orders.blades.shipment-detail.labels.price-with-tax",
                        templateUrl: 'priceWithTax.html'
                    }
                ]
            }
        };
        knownOperations.registerOperation(shipmentOperation);

        //Register widgets
        widgetService.registerWidget({
            controller: 'virtoCommerce.orderModule.notificationsLogWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/notificationsLogWidget.tpl.html'
        }, 'customerOrderDetailWidgets');

        var operationItemsWidget = {
            controller: 'virtoCommerce.orderModule.customerOrderItemsWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/customerOrder-items-widget.tpl.html'
        };
        widgetService.registerWidget(operationItemsWidget, 'customerOrderDetailWidgets');

        widgetService.registerWidget({
            controller: 'virtoCommerce.orderModule.customerOrderChangeLogWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/customerOrder-change-log-widget.tpl.html' }, 'customerOrderDetailWidgets');

        var shipmentItemsWidget = {
            controller: 'virtoCommerce.orderModule.shipmentItemsWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/shipment-items-widget.tpl.html'
        };
        widgetService.registerWidget(shipmentItemsWidget, 'shipmentDetailWidgets');
        widgetService.registerWidget({
            controller: 'platformWebApp.changeLog.operationsWidgetController',
            template: '$(Platform)/Scripts/app/changeLog/widgets/operations-widget.tpl.html' }, 'shipmentDetailWidgets');


        var customerOrderAddressWidget = {
            controller: 'virtoCommerce.orderModule.customerOrderAddressWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/customerOrder-address-widget.tpl.html'
        };
        widgetService.registerWidget(customerOrderAddressWidget, 'customerOrderDetailWidgets');
        function checkPermissionToReadPrices() {
            return authService.checkPermission('order:read_prices');
        }

        var customerOrderTotalsWidget = {
            controller: 'virtoCommerce.orderModule.customerOrderTotalsWidgetController',
            size: [2, 2],
            isVisible: checkPermissionToReadPrices,
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/customerOrder-totals-widget.tpl.html'
        };
        widgetService.registerWidget(customerOrderTotalsWidget, 'customerOrderDetailWidgets');


        var operationCommentWidget = {
            controller: 'virtoCommerce.orderModule.operationCommentWidgetController',
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/operation-comment-widget.tpl.html'
        };
        widgetService.registerWidget(operationCommentWidget, 'customerOrderDetailWidgets');
        widgetService.registerWidget(operationCommentWidget, 'shipmentDetailWidgets');
        widgetService.registerWidget(operationCommentWidget, 'paymentDetailWidgets');

        var shipmentAddressWidget = {
            controller: 'virtoCommerce.orderModule.shipmentAddressWidgetController',
            size: [2, 1],
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/shipment-address-widget.tpl.html'
        };
        widgetService.registerWidget(shipmentAddressWidget, 'shipmentDetailWidgets');


        var shipmentTotalWidget = {
            controller: 'virtoCommerce.orderModule.shipmentTotalsWidgetController',
            size: [2, 1],
            isVisible: checkPermissionToReadPrices,
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/shipment-totals-widget.tpl.html'
        };
        widgetService.registerWidget(shipmentTotalWidget, 'shipmentDetailWidgets');

        widgetService.registerWidget({
            controller: 'virtoCommerce.orderModule.paymentAddressWidgetController',
            size: [2, 1],
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/payment-address-widget.tpl.html'
        }, 'paymentDetailWidgets');

        var paymentTotalWidget = {
            controller: 'virtoCommerce.orderModule.paymentTotalsWidgetController',
            size: [2, 1],
            isVisible: checkPermissionToReadPrices,
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/payment-totals-widget.tpl.html'
        };
        widgetService.registerWidget(paymentTotalWidget, 'paymentDetailWidgets');

        var paymentTransactionsWidget = {
            controller: 'virtoCommerce.orderModule.paymentTransactionsWidgetController',
            size: [1, 1],
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/payment-transactions-widget.tpl.html'
        };
        widgetService.registerWidget(paymentTransactionsWidget, 'paymentDetailWidgets');
        widgetService.registerWidget({
            controller: 'platformWebApp.changeLog.operationsWidgetController',
            template: '$(Platform)/Scripts/app/changeLog/widgets/operations-widget.tpl.html'
        }, 'paymentDetailWidgets');

        var dynamicPropertyWidget = {
            controller: 'platformWebApp.dynamicPropertyWidgetController',
            template: '$(Platform)/Scripts/app/dynamicProperties/widgets/dynamicPropertyWidget.tpl.html'
        };
        widgetService.registerWidget(dynamicPropertyWidget, 'shipmentDetailWidgets');
        widgetService.registerWidget(dynamicPropertyWidget, 'customerOrderDetailWidgets');
        widgetService.registerWidget(dynamicPropertyWidget, 'paymentDetailWidgets');


        var operationsTreeWidget = {
            controller: 'virtoCommerce.orderModule.operationTreeWidgetController',
            size: [4, 3],
            template: 'Modules/$(VirtoCommerce.Orders)/Scripts/widgets/operation-tree-widget.tpl.html'
        };
        widgetService.registerWidget(operationsTreeWidget, 'customerOrderDetailWidgets');

        // register dashboard widgets
        var statisticsController = 'virtoCommerce.orderModule.dashboard.statisticsWidgetController';
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-revenue.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-customersCount.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-revenuePerCustomer.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-orderValue.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-itemsPurchased.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [2, 1],
            template: 'order-statistics-lineitemsPerOrder.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [3, 2],
            template: 'order-statistics-revenueByQuarter.html'
        }, 'mainDashboard');
        widgetService.registerWidget({
            controller: statisticsController,
            size: [3, 2],
            template: 'order-statistics-orderValueByQuarter.html'
        }, 'mainDashboard');

        $http.get('Modules/$(VirtoCommerce.Orders)/Scripts/widgets/dashboard/statistics-templates.html').then(function (response) {
            // compile the response, which will put stuff into the cache
            $compile(response.data);
        });


        //Register permission scopes templates used for scope bounded definition in role management ui
        var orderStoreScope = {
            type: 'OrderSelectedStoreScope',
            title: 'Only for orders in selected stores',
            selectFn: function (blade, callback) {
                var newBlade = {
                    id: 'store-pick',
                    title: this.title,
                    subtitle: 'Select stores',
                    currentEntity: this,
                    onChangesConfirmedFn: callback,
                    dataPromise: stores.query().$promise,
                    controller: 'platformWebApp.security.scopeValuePickFromSimpleListController',
                    template: '$(Platform)/Scripts/app/security/blades/common/scope-value-pick-from-simple-list.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }
        };
        scopeResolver.register(orderStoreScope);
        var responsibleOrderScope = {
            type: 'OnlyOrderResponsibleScope',
            title: 'Only for order responsible'
        };
        scopeResolver.register(responsibleOrderScope);

        $rootScope.$on('loginStatusChanged', function (event, authContext) {
            if (authContext.isAuthenticated) {
                var now = new Date();
                var startDate = new Date();
                startDate.setFullYear(now.getFullYear() - 1);

                customerOrders.getDashboardStatistics({ start: startDate, end: now }, function (data) {
                    // prepare statistics
                    var statisticsToChartRows = function (statsList, allCurrencies) {
                        var groupedQuarters = _.groupBy(statsList, function (stats) {
                            return stats.year + ' Q' + stats.quarter;
                        });
                        return _.map(groupedQuarters, function (stats, key) {
                            var values = [{
                                v: key
                            }];
                            _.each(allCurrencies, function (x) {
                                var stat = _.findWhere(stats, {
                                    currency: x
                                });
                                values.push({
                                    v: stat ? stat.amount : 0
                                });
                            });
                            return {
                                c: values
                            };
                        });
                    };

                    var allCurrencies = _.unique(_.pluck(data.avgOrderValuePeriodDetails, 'currency').sort());

                    var cols = [{
                        id: "quarter", label: "Quarter", type: "string"
                    }];
                    _.each(allCurrencies, function (x) {
                        cols.push({
                            id: "revenue" + x, label: x, type: "number"
                        });
                    });

                    data.chartRevenueByQuarter = {
                        "type": "LineChart",
                        "data": {
                            cols: cols,
                            rows: statisticsToChartRows(data.revenuePeriodDetails, allCurrencies)
                        },
                        "options": {
                            "title": "Revenue by quarter",
                            "legend": {
                                position: 'top'
                            },
                            "vAxis": {
                                // "title": "Sales unit",
                                gridlines: {
                                    count: 8
                                }
                            },
                            "hAxis": {
                                // "title": "Date"
                                slantedText: true,
                                slantedTextAngle: 20
                            }
                        },
                        "formatters": {}
                    };

                    cols = [{
                        id: "quarter", label: "Quarter", type: "string"
                    }];
                    _.each(allCurrencies, function (x) {
                        cols.push({
                            id: "avg-orderValue" + x, label: x, type: "number"
                        });
                    });

                    data.chartOrderValueByQuarter = {
                        "type": "ColumnChart",
                        "data": {
                            cols: cols,
                            rows: statisticsToChartRows(data.avgOrderValuePeriodDetails, allCurrencies)
                        },
                        "options": {
                            "title": "Average Order value by quarter",
                            "legend": {
                                position: 'top'
                            },
                            "vAxis": {
                                gridlines: {
                                    count: 8
                                }
                            },
                            "hAxis": {
                                slantedText: true,
                                slantedTextAngle: 20
                            }
                        },
                        "formatters": {}
                    };

                    $localStorage.ordersDashboardStatistics = data;
                },
                function (error) {
                    console.log(error);
                });
            }
        });
    }]);
