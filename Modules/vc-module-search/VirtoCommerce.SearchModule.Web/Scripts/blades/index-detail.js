angular.module('virtoCommerce.searchModule')
.controller('virtoCommerce.searchModule.indexDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.searchModule.searchIndexation', function ($scope, bladeNavigationService, searchIndexationApi) {
    var blade = $scope.blade;

    blade.initialize = function (data) {
        blade.currentEntity = data;
        blade.isLoading = false;
    };

    blade.headIcon = 'fa-search';


    blade.toolbarCommands = [
     {
       name: "search.commands.rebuild-index",
       icon: 'fa fa-recycle',
       index: 2,
       executeMethod: function (blade) {
           searchIndexationApi.index([{ documentType: blade.documentType, documentIds: [blade.currentEntityId]}], function openProgressBlade(data) {
               // show indexing progress
               var newBlade = {
                   id: 'indexProgress',
                   notification: data,
                   parentRefresh: blade.parentRefresh,
                   controller: 'virtoCommerce.searchModule.indexProgressController',
                   template: 'Modules/$(VirtoCommerce.Search)/Scripts/blades/index-progress.tpl.html'
               };
               bladeNavigationService.showBlade(newBlade, blade.parentBlade || blade);
           });
       },
       canExecuteMethod: function () { return true; },
       permission: 'search:index:rebuild'
     }
    ];

    blade.title = blade.currentEntity.name;
    blade.subtitle = 'search.blades.index-detail.subtitle';
    if (!blade.data) {
        blade.title = 'search.blades.index-detail.title-new';
        blade.subtitle = undefined;
    }

    blade.initialize(blade.data);
}]);