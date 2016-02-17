'use strict';
const path = require('path');

const webpack = require('webpack');
const _ = require('lodash');
const autoprefixer = require('autoprefixer');
const AssetsPlugin = require('assets-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const OccurenceOrderPlugin = webpack.optimize.OccurenceOrderPlugin;
const HotModuleReplacementPlugin = webpack.HotModuleReplacementPlugin;
const NoErrorsPlugin = webpack.NoErrorsPlugin;
const CommonsChunkPlugin = webpack.optimize.CommonsChunkPlugin;
const UglifyJsPlugin =  webpack.optimize.UglifyJsPlugin;
const DefinePlugin = webpack.DefinePlugin;

const params = require('./lib/client/client.config.js');
const libs = require('./vendor-libs');

const root = __dirname;
const DIRS = {
    root: root,
    client: path.join(root, 'lib', 'client'),
    app: path.join(root, 'lib', 'app'),
    dist: path.join(root, 'lib', 'server', 'static')
};
const ENV = params.get('ENV');
process.env.NODE_ENV = ENV;
const DEBUG = params.get('DEBUG');

const styleLoaders = [
    'style?sourceMap',
    'css?modules&importLoaders=1&localIdentName=[path]___[name]__[local]___[hash:base64:5]',
    'sass?sourceMap'
];

const config = {
    entry: {
        app: ['babel-polyfill', path.join(DIRS.client, 'entry.js')],
        vendor: libs
    },
    output: {
        path: DIRS.dist,
        filename: 'app.js'
    },
    devtool: 'source-map',
    resolve: {
        root: DIRS.app
    },
    module: {
        // TODO: babel-transform-runtime https://phabricator.babeljs.io/T6922
        loaders: [{
            test: /\.js$/,
            loader: 'babel',
            exclude: /node_modules/,
            query: {cacheDirectory: true}
        }, {
            key: 'sass',
            test: /\.scss$/,
            loader: ExtractTextPlugin.extract('styles', styleLoaders.join('!'))
        }]
    },
    plugins: [
        new DefinePlugin({
            CONFIG: JSON.stringify(params.get('')),
            DEBUG: params.get('DEBUG'),
            ENV: JSON.stringify(params.get('ENV')),
            BROWSER: true
        }),
        new HtmlWebpackPlugin({
            template: path.join(DIRS.app, 'index.html')
        }),
        new CommonsChunkPlugin({
            name: 'vendor',
            filename: '[name]-[hash].js'
        }),
        new ExtractTextPlugin('styles.css'),
        new AssetsPlugin({
            path: DIRS.dist,
            filename: 'assets.json',
            prettyPrint: true
        })
    ]
};

function removePlugins(types) {
    types = _.isArray(types) ? types : [types];
    const toRemove = _.find(config.plugins, p => _.some(types, t => p instanceof t));
    config.plugins = _.difference(config.plugins, toRemove);
    return config.plugins;
}

if (ENV === 'development') {
    config.entry.app.unshift('webpack-hot-middleware/client');
    config.devtool = 'cheap-module-eval-source-map';
    config.devServer = {
        contentBase: DIRS.dist,
        hot: true,
        inline: true,
        noInfo: true,
        historyApiFallback: true
    };

    const sassLoader = _.find(config.module.loaders, {key: 'sass'});
    sassLoader.loader = styleLoaders.join('!');

    removePlugins(ExtractTextPlugin, AssetsPlugin);
    config.plugins = config.plugins.concat([
        new OccurenceOrderPlugin(),
        new HotModuleReplacementPlugin(),
        new NoErrorsPlugin()
    ]);
}

module.exports = config;
