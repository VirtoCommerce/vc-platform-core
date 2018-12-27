angular.module('virtoCommerce.customerModule')
.factory('virtoCommerce.customerModule.memberTypesResolverService', function () {
    return {
        objects: [],
        registerType: function (memberTypeDefinition) {
            memberTypeDefinition.detailBlade = angular.extend({
                id: "memberDetail",
                metaFields: [],
                controller: 'virtoCommerce.customerModule.memberDetailController',
                memberTypeDefinition: memberTypeDefinition
            }, memberTypeDefinition.detailBlade);

            memberTypeDefinition.knownChildrenTypes = memberTypeDefinition.knownChildrenTypes || [];

            this.objects.push(memberTypeDefinition);
        },
        resolve: function (type) {
            return _.findWhere(this.objects, { memberType: type });
        }
    };
});