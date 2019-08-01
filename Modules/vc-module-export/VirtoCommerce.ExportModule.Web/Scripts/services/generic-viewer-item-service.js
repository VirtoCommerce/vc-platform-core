angular.module('virtoCommerce.customerModule')
.factory('virtoCommerce.exportModule.genericViewerItemService', function () {
    var viewers = {};
    
    this.registerViewer = function (typeName, blade) {
        viewers[typeName] = blade;
    }
    
    this.getViewer = function (typeName) {
        return viewers[typeName];
    }

    return this;
})
