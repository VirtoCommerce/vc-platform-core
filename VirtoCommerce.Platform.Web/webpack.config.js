const path = require('path');

module.exports = {
    entry: { 'main': './wwwroot/src/js/vendor.js' },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: 'vendor.js',
        publicPath: 'dist/'
    }
};
