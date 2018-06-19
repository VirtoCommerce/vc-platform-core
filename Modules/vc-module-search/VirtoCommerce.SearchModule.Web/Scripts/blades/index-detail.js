angular.module('virtoCommerce.coreModule.searchIndex')
.controller('virtoCommerce.coreModule.searchIndex.indexDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.coreModule.searchIndex.searchIndexation', function ($scope, bladeNavigationService, searchIndexationApi) {
    var blade = $scope.blade;

    blade.initialize = function (data) {
        blade.currentEntity = data;
        blade.isLoading = false;
    };

    blade.headIcon = 'fa-search';


    blade.toolbarCommands = [
     {
       name: "core.commands.rebuild-index",
       icon: 'fa fa-recycle',
       index: 2,
       executeMethod: function (blade) {
           searchIndexationApi.index([{ documentType: blade.documentType, documentIds: [blade.currentEntityId]}], function openProgressBlade(data) {
               // show indexing progress
               var newBlade = {
                   id: 'indexProgress',
                   notification: data,
                   parentRefresh: blade.parentRefresh,
                   controller: 'virtoCommerce.coreModule.indexProgressController',
                   template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/blades/index-progress.tpl.html'
               };
               bladeNavigationService.showBlade(newBlade, blade.parentBlade || blade);
           });
       },
       canExecuteMethod: function () { return true; },
       permission: 'core:search:index:rebuild'
     }
    ];

    blade.title = blade.currentEntity.name;
    blade.subtitle = 'core.blades.index-detail.subtitle';
    if (!blade.data) {
        blade.title = 'core.blades.index-detail.title-new';
        blade.subtitle = undefined;
    }

    blade.initialize(blade.data);
}]);