angular.module('platformWebApp')
    .factory('platformWebApp.thumbnail.api', ['$resource', '$timeout', function ($resource, $timeout) {
        return {
            getTaskList: function() {
                var list = [
                    {
                        id: '778810c7629c44688db7ce997c2f18f1',
                        createdDate: '2016-08-13T16:20:14.493Z',
                        modifiedDate: '2018-07-13T16:20:14.493Z',
                        createdBy: 'admin',
                        modifiedBy: 'admin',
                        name: 'Clother catalog test thumbnail',
                        lastRun: '2018-01-25T16:20:14.493Z',
                        workPath: '/catalog/clother',
                        thumbnailOptions: {
                            name: 'options',
                            fileSuffix: 'thb_',
                            resizeMethod: 'fixedWidth',
                            width: '50',
                            height: '50',
                            backgroundColor: '#555'
                        }
                    },
                    {
                        id: '397010c7629c44688db7ce997c2f18f1',
                        createdDate: '2017-07-13T16:20:14.493Z',
                        modifiedDate: '2018-07-13T16:20:14.493Z',
                        createdBy: 'admin',
                        modifiedBy: 'admin',
                        name: 'Electronics catalog test thumbnail',
                        lastRun: '2018-01-25T16:20:14.493Z',
                        workPath: '/catalog/electronics',
                        thumbnailOptions: {
                            name: 'options',
                            fileSuffix: 'thb_',
                            resizeMethod: 'crop',
                            width: '100',
                            height: '100',
                            backgroundColor: '#333333'
                        }
                    }
                ];
                return $timeout(function() { return list; }, 1000);
            },

            getTask: function(id) {
                var item =
                {
                    id: '778810c7629c44688db7ce997c2f18f1',
                    createdDate: '2016-08-13T16:20:14.493Z',
                    modifiedDate: '2018-07-13T16:20:14.493Z',
                    createdBy: 'admin',
                    modifiedBy: 'admin',
                    name: 'Clother catalog test thumbnail',
                    lastRun: '',
                    workPath: '/catalog/clother',
                    thumbnailOptions: [
                        {
                            id: '778810',
                            name: '50x50',
                            fileSuffix: 'thb_',
                            resizeMethod: 'FixedSize',
                            width: '50',
                            height: '50'
                        },
                        {
                            id: '77890',
                            name: '100x100',
                            fileSuffix: 'img_',
                            resizeMethod: 'Crop',
                            width: '100',
                            height: '100',
                            backgroundColor: '#777'
                        }
                    ]
                };

                return $timeout(function() { return item; }, 1000);
            },

            getListOptions: function () {
                var result = [
                    {
                        id: '778810',
                        name: '50x50',
                        fileSuffix: 'thb_',
                        resizeMethod: 'FixedSize',
                        width: '50',
                        height: '50'
                    },
                    {
                        id: '77890',
                        name: '100x100',
                        fileSuffix: 'img_',
                        resizeMethod: 'Crop',
                        width: '100',
                        height: '100',
                        backgroundColor: '#777'
                    },
                    {
                        id: '7789011',
                        name: '75x175',
                        fileSuffix: '_',
                        resizeMethod: 'FixedHeight',
                        width: '175',
                        height: '75',
                        backgroundColor: '#888'
                    }
                ];

                return $timeout(function () { return result; }, 1000);
            },

            saveOption: function (data, callback, error) {
                $timeout(function () { return {}; }, 1000).then(function () {
                    callback(data);
                });
            },

            updateOption: function (data, callback, error) {
                $timeout(function () { return {}; }, 1000).then(function() {
                    callback(data);
                });
            },

            removeOptions: function (data, callback, error) {
                callback();
            },

            deleteTask: function () { },

            taskRun: function() {
                return $timeout(function () { return {}; }, 1000);
            },

            taskCancel: function () {
                return $timeout(function () { return true; }, 1000);
            }
    }

    }]);
