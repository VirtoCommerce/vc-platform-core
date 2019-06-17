const glob = require("glob");
const path = require("path");
const webpack = require("webpack");
module.exports = [
    {
        entry: glob.sync('./Scripts/**/*.js', { nosort: true }),
        output: {
            path: path.resolve(__dirname, 'dist'),
            filename: 'app.js'
        },
        devtool: false,
        plugins: [
            new webpack.SourceMapDevToolPlugin({
                namespace: 'VirtoCommerce.Inventory'
            })           
        ]
    }
];
