angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportTypeSelectorController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.exportModule.exportModuleApi', function ($scope, bladeNavigationService, exportApi) {
        var typeTree;
        var blade = $scope.blade;
        blade.title = 'export.blades.export-settings.labels.exported-type';
        blade.headIcon = 'fa-folder';

        blade.refresh = function (disableOpenAnimation) {
            blade.isLoading = true;

            exportApi.getKnowntypes(function (results) {
                    _.each(results, function (item) {
                        item.groupName = item.group + '|' + item.typeName;
                        var lastIndex = item.typeName.lastIndexOf('.');
                        item.name = item.typeName.substring(lastIndex + 1);
                        item.isTabularExportSupported = item.isTabularExportSupported;
                    });
                blade.allGroups = results;
                typeTree = {};
                _.each(results, function (exportedType ) {
                    var paths = (exportedType.groupName ? exportedType.groupName : 'General').split('|');
                    var lastParent = typeTree;
                    var lastParentId = '';
                    _.each(paths, function (path, i) {
                        lastParentId += '|' + path;
                        if (!lastParent[path]) {
                            var lastIndex = path.lastIndexOf('.');
                            var treeNode = { name: path.substring(lastIndex + 1), groupName: lastParentId.substring(1) }
                            lastParent[path] = treeNode;
                            if (exportedType.groupName && _.all(blade.allGroups, function (x) { return x.groupName !== treeNode.groupName; })) {
                                blade.allGroups.push(treeNode);
                            }
                        }

                        if (i < paths.length - 1) {
                            if (!lastParent[path].children) {
                                lastParent[path].children = {};
                            }
                            lastParent = lastParent[path].children;
                        }
                    });
                });

                blade.isLoading = false;

                // restore previous selection
                if (blade.searchText) {
                    blade.currentEntities = typeTree;
                } else {
                    // reconstruct tree by breadCrumbs
                    var lastchildren = typeTree;
                    for (var i = 1; i < blade.breadcrumbs.length; i++) {
                        lastchildren = lastchildren[blade.breadcrumbs[i].name].children;
                    }
                    blade.currentEntities = lastchildren;
                }

                // open previous settings detail blade if possible
                if ($scope.selectedNodeId) {
                    $scope.selectNode({ groupName: $scope.selectedNodeId }, disableOpenAnimation);
                }
            },
                function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        };

        $scope.selectNode = function (node, disableOpenAnimation) {
            bladeNavigationService.closeChildrenBlades(blade, function () {
                $scope.selectedNodeId = node.groupName;
                if (node.children) {
                    blade.searchText = null;
                    blade.currentEntities = node.children;

                    setBreadcrumbs(node);
                } else {
                    var selectedTypes = _.filter(blade.allGroups, function (x) { return x.groupName === node.groupName || (node.groupName === 'General' && !x.groupName); });
                    var selectedType = selectedTypes[0];
                    var exportDataRequest = {
                        exportTypeName: selectedType.typeName,
                        metaData: selectedType.metaData,
                        isTabularExportSupported: selectedType.isTabularExportSupported,
                        dataQuery: {
                            exportTypeName: selectedType.exportDataQueryType,
                            includedColumns: selectedType.metaData.propertyInfos,
                            take: 10000
                        }
                    };
                    if (blade.onSelected) {
                        blade.onSelected(exportDataRequest);
                        bladeNavigationService.closeBlade(blade);
                    }
                }
            });
        };

        //Breadcrumbs
        function setBreadcrumbs(node) {
            blade.breadcrumbs.splice(1, blade.breadcrumbs.length - 1);

            if (node.groupName) {
                var lastParentId = '';
                var lastchildren = typeTree;
                var paths = node.groupName.split('|');
                _.each(paths, function (path) {
                    lastchildren = lastchildren[path].children;
                    lastParentId += '|' + path;
                    var breadCrumb = {
                        id: lastParentId.substring(1),
                        name: path,
                        children: lastchildren,
                        navigate: function () {
                            $scope.selectNode({ groupName: this.id, children: this.children });
                        }
                    };

                    blade.breadcrumbs.push(breadCrumb);
                });
            }
        }

        blade.breadcrumbs = [{
            id: null,
            name: "platform.navigation.bread-crumb-top",
            navigate: function () {
                $scope.selectNode({ groupName: null, children: typeTree });
            }
        }];

        $scope.$watch('blade.searchText', function (newVal) {
            if (newVal) {
                blade.currentEntities = typeTree;
                setBreadcrumbs({ groupName: null });
            }
        });

        // actions on load
        blade.refresh();
    }]);
