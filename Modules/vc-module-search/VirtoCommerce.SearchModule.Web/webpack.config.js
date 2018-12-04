const glob = require("glob");
const path = require("path");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');

const rootPath = path.resolve(__dirname, 'dist');

function getEntrypoints() {
    const result = [
        ...glob.sync('./Scripts/**/*.js', { ignore: ['./Scripts/dist/**/*.*'], nosort: true }),
        ...glob.sync('./Content/**/*.css', { nosort: true })
    ];

    return result;
}

module.exports = [
    {
        entry: getEntrypoints(),
        output: {
            path: rootPath,
            filename: 'app.js'
        },
        module: {
            rules: [
                {
                    test: /\.css$/,
                    loaders: [MiniCssExtractPlugin.loader, "css-loader"]
                }
            ]
        },
        plugins: [
            new CleanWebpackPlugin(rootPath, { verbose: true }),
            new MiniCssExtractPlugin({
                filename: 'style.css'
            })
        ]
    }
];
