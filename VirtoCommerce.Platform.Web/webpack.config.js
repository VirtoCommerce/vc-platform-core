const path = require('path');
const glob = require('glob');

module.exports = [{
        entry: { 'main': './wwwroot/src/js/vendor.js' },
        output: {
            path: path.resolve(__dirname, 'wwwroot/dist'),
            filename: 'vendor.js',
            publicPath: 'dist/'
        },
        mode: 'development'
    },
    {
        entry: glob.sync('./wwwroot/js/**/*.js'),
        output: {
            path: path.resolve(__dirname, 'wwwroot/dist'),
            filename: 'app.js',
            publicPath: 'dist/'
        },
        mode: 'development'
}];
