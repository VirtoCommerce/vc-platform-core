const glob = require("glob");
const path = require("path");
const webpack = require("webpack");

module.exports = [
    {
        entry: glob.sync('./Scripts/**/*.js', { ignore: ['./Scripts/dist/**/*.*'], nosort: true }),
        output: {
            path: path.resolve(__dirname, 'Scripts/dist'),
            filename: 'app.js'
        },
        plugins: [
            new webpack.ProvidePlugin({
                $: 'jquery',
                jQuery: 'jquery',
                'window.jQuery': 'jquery'
            })
        ]
    }
];
