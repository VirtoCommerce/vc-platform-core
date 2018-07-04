angular.module('virtoCommerce.imageToolsModule')
    .factory('virtoCommerce.imageToolsModule.resizeMethod', function () {
        return {
            get: function () {
                return [
                    {
                        value: "FixedSize",
                        title: "imageTools.blades.setting-detail.resize-method.fixed-size"
                    },
                    {
                        value: "FixedWidth",
                        title: "imageTools.blades.setting-detail.resize-method.fixed-width"
                    },
                    {
                        value: "FixedHeight",
                        title: "imageTools.blades.setting-detail.resize-method.fixed-height"
                    },
                    {
                        value: "Crop",
                        title: "imageTools.blades.setting-detail.resize-method.crop"
                    }
                ];
            }
        };
    }).factory('virtoCommerce.imageToolsModule.anchorPosition', function () {
        return {
            get: function () {
                return [
                    {
                        value: "TopLeft",
                        title: "imageTools.blades.setting-detail.anchor-position.top-left"
                    },
                    {
                        value: "TopCenter",
                        title: "imageTools.blades.setting-detail.anchor-position.top-center"
                    },
                    {
                        value: "TopRight",
                        title: "imageTools.blades.setting-detail.anchor-position.top-right"
                    },
                    {
                        value: "CenterLeft",
                        title: "imageTools.blades.setting-detail.anchor-position.center-left"
                    },
                    {
                        value: "Center",
                        title: "imageTools.blades.setting-detail.anchor-position.center"
                    },
                    {
                        value: "CenterRight",
                        title: "imageTools.blades.setting-detail.anchor-position.center-right"
                    },
                    {
                        value: "BottomLeft",
                        title: "imageTools.blades.setting-detail.anchor-position.bottom-left"
                    },
                    {
                        value: "BottomCenter",
                        title: "imageTools.blades.setting-detail.anchor-position.bottom-center"
                    },
                    {
                        value: "BottomRight",
                        title: "imageTools.blades.setting-detail.anchor-position.bottom-right"
                    }
                ];
            }
        };
    });







