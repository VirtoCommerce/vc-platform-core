angular.module('virtoCommerce.catalogModule')
    .factory('virtoCommerce.catalogModule.valueTypes', function () {
        return {
            get: function() {
                return [
                    {
                        valueType: "ShortText",
                        title: "platform.properties.short-text.title",
                        description: "platform.properties.short-text.description"
                    },
                    {
                        valueType: "LongText",
                        title: "platform.properties.long-text.title",
                        description: "platform.properties.long-text.description"
                    },
                    {
                        valueType: "Number",
                        title: "platform.properties.decimal.title",
                        description: "platform.properties.decimal.description"
                    },
                    {
                        valueType: "DateTime",
                        title: "platform.properties.date-time.title",
                        description: "platform.properties.date-time.description"
                    },
                    {
                        valueType: "Boolean",
                        title: "platform.properties.boolean.title",
                        description: "platform.properties.boolean.description"
                    },
                    {
                        valueType: "Integer",
                        title: "platform.properties.integer.title",
                        description: "platform.properties.integer.description"
                    },
                    {
                        valueType: "GeoPoint",
                        title: "catalog.properties.geoPoint.title",
                        description: "catalog.properties.geoPoint.description"
                    }
                ];
            }
        };
    });
