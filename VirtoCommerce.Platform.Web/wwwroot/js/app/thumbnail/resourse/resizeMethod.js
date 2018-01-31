angular.module('platformWebApp')
    .factory('platformWebApp.thumbnail.resizeMethod', function () {
        return {
            get: function () {
                return [
                    {
                        value: "FixedSize",
                        title: "platform.blades.thumbnail.blades.setting-detail.resize-method.fixed-size"
                    },
                    {
                        value: "FixedWidth",
                        title: "platform.blades.thumbnail.blades.setting-detail.resize-method.fixed-width"
                    },
                    {
                        value: "FixedHeight",
                        title: "platform.blades.thumbnail.blades.setting-detail.resize-method.fixed-height"
                    },
                    {
                        value: "Crop",
                        title: "platform.blades.thumbnail.blades.setting-detail.resize-method.crop"
                    }
                ];
            }
        };
    });
