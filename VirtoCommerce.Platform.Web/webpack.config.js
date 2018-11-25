const path = require('path');
const glob = require('glob');
const webpack = require('webpack');
const appDependencies = require('./wwwroot/js/app/appDependencies.json').appDependencies;

//console.log(glob.sync('./Modules/**/*.Web/Scripts/**/*.js', { ignore: ['./Modules/Module1/**/*.*', './**/allPackages.js'], nosort: true } ));

module.exports = [
    {
        entry: { 'main': './wwwroot/src/js/vendor.js' },
        output: {
            path: path.resolve(__dirname, 'wwwroot/dist'),
            filename: 'vendor.js',
            publicPath: 'dist/'
        },
        mode: 'development',
        module: {
            rules: [
                {
                    test: /\.css$/,
                    loaders: ["style-loader", "css-loader"]
                },
                {
                    test: /\.(jpe?g|png|gif)$/i,
                    loader: "file-loader",
                    options: {
                        name: '[name].[ext]',
                        outputPath: 'assets/images/'
                        //the images will be emited to dist/assets/images/ folder
                    }
                },
                {
                    test: /\.modernizrrc\.js$/,
                    loader: 'webpack-modernizr-loader'
                }
            ]
        },
        plugins: [
            new webpack.ProvidePlugin({
                $: 'jquery',
                jQuery: 'jquery',
                'window.jQuery': 'jquery'
            })
        ],
        resolve: {
            alias: {
                Vendor: path.resolve(__dirname, 'wwwroot/vendor'),
                modernizr$: path.resolve(__dirname, ".modernizrrc.js")
            }
        }
    },
    {
        entry: glob.sync('./wwwroot/js/**/*.js'),
        output: {
            path: path.resolve(__dirname, 'wwwroot/dist'),
            filename: 'app.js',
            publicPath: 'dist/'
        },
        mode: 'development',
        plugins: [
            new webpack.ProvidePlugin({
                _: 'underscore'
            }),
            new webpack.DefinePlugin({
                'AppDependencies': JSON.stringify(appDependencies)
            })
        ]
    }
];
