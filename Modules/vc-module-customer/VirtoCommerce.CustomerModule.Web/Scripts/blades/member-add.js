angular.module('virtoCommerce.customerModule')
.controller('virtoCommerce.customerModule.memberAddController', ['$scope', 'virtoCommerce.customerModule.memberTypesResolverService', function ($scope, memberTypesResolverService) {
    var blade = $scope.blade;

    $scope.addMember = function (node) {
        $scope.bladeClose(function () {
            blade.parentBlade.showDetailBlade({ memberType: node.memberType }, true);
        });
    };
    
    if (blade.currentEntity && blade.currentEntity.memberType) {
        var parentType = memberTypesResolverService.resolve(blade.currentEntity.memberType);
        $scope.availableTypes = _.map(parentType.knownChildrenTypes, function (type) {
            return memberTypesResolverService.resolve(type);
        }) ;
    } else{
        $scope.availableTypes =  memberTypesResolverService.objects;
    }
    

    blade.isLoading = false;
}]);