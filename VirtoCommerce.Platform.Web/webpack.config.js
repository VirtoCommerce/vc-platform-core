const path = require('path');
const glob = require('glob');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');

const rootPath = path.resolve(__dirname, 'wwwroot/dist');

module.exports = env => {
    return [
        {
            entry: { 'main': './wwwroot/src/js/vendor.js' },
            output: {
                path: rootPath,
                filename: 'vendor.js',
                publicPath: 'dist/'
            },
            module: {
                rules: [
                    {
                        test: /\.css$/,
                        loaders: [ MiniCssExtractPlugin.loader, "css-loader" ]
                    },
                    {
                        test: /\.(jpe?g|png|gif)$/i,
                        loader: "file-loader",
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'images/'
                        }
                    },
                    {
                        test: /\.modernizrrc\.js$/,
                        loader: 'webpack-modernizr-loader'
                    },
                    {
                        test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                        use: [
                            {
                                loader: 'file-loader',
                                options: {
                                    name: '[name].[ext]',
                                    outputPath: 'fonts/'
                                }
                            }
                        ]
                    }
                ]
            },
            plugins: [
                new CleanWebpackPlugin(rootPath, { verbose: env.dev ? true : false }),
                new MiniCssExtractPlugin({
                    filename: 'vendor.css'
                }),
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
                path: rootPath,
                filename: 'app.js',
                publicPath: 'dist/'
            },
            plugins: [
                new webpack.ProvidePlugin({
                    _: 'underscore'
                })
            ]
        },
        //{
        //    entry: './wwwroot/src/css/vendor.css',
        //    output: {
        //        path: path.resolve(__dirname, 'wwwroot/dist')
        //    },
        //    plugins: [
        //        new CleanWebpackPlugin(path.resolve(__dirname, 'wwwroot/dist'), { verbose: env.dev ? true : false }),
        //        new MiniCssExtractPlugin({
        //            filename: 'vendor.css'
        //        })
        //    ],
        //    module: {
        //        rules: [
        //            {
        //                test: /\.css$/,
        //                use: [
        //                    MiniCssExtractPlugin.loader,
        //                    "css-loader"
        //                ]
        //            },
        //            {
        //                test: /\.(jpe?g|png|gif)$/i,
        //                loader: "file-loader",
        //                options: {
        //                    name: '[name].[ext]',
        //                    outputPath: 'assets/images/'
        //                    //the images will be emited to dist/assets/images/ folder
        //                }
        //            },
        //            {
        //                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
        //                use: [
        //                    {
        //                        loader: 'file-loader',
        //                        options: {
        //                            name: '[name].[ext]',
        //                            outputPath: 'fonts/'
        //                        }
        //                    }
        //                ]
        //            }
        //        ]
        //    },
        //    resolve: {
        //        modules: ['node_modules']
        //    }
        //}
    ];
};
