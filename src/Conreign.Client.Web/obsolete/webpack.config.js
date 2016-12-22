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
const UglifyJsPlugin = webpack.optimize.UglifyJsPlugin;
const ProvidePlugin = webpack.ProvidePlugin;
const DefinePlugin = webpack.DefinePlugin;

const params = require('./lib/client/client.config.js');
const libs = require('./vendor-libs');

const root = __dirname;
const DIRS = {
    root: root,
    client: path.join(root, 'lib', 'client'),
    server: path.join(root, 'lib', 'server'),
    app: path.join(root, 'lib', 'app'),
    static: path.join(root, 'lib', 'server', 'static'),
    build: path.join(root, 'lib', 'server', 'build'),
    nodeModules: path.join(root, 'node_modules')
};
const ENV = params.get('ENV');
process.env.NODE_ENV = ENV;
const DEBUG = params.get('DEBUG');

const styleLoaders = [
    'style?sourceMap',
    'css?modules&importLoaders=1&localIdentName=[path]___[name]__[local]___[hash:base64:5]',
    'postcss',
    'sass?sourceMap'
];

const clientConfig = {
    entry: {
        app: [path.join(DIRS.client, 'entry.js')],
        vendor: libs
    },
    output: {
        path: DIRS.static,
        filename: 'app-[hash].js'
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
            include: [
                DIRS.app,
                DIRS.client,
                DIRS.server
            ],
            query: {cacheDirectory: true}
        }, {
            key: 'sass',
            test: /\.scss$/,
            loader: ExtractTextPlugin.extract('style', _.drop(styleLoaders, 1).join('!'))
        }, {
            test: /\.json$/,
            loader: 'json'
        }, {
            test: /\.(png|jpg)$/,
            loader: 'url?limit=8192'
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
            path: DIRS.static,
            filename: 'assets.json',
            prettyPrint: true
        }),
        new ProvidePlugin({
            'window.jQuery': 'jquery'
        })
    ],
    postcss: () => [autoprefixer('last 2 versions')]
};

function removePlugins(config, types) {
    types = _.isArray(types) ? types : [types];
    const toRemove = _.filter(config.plugins, p => _.some(types, t => p instanceof t));
    config.plugins = _.difference(config.plugins, toRemove);
    return config.plugins;
}

if (ENV === 'development') {
    clientConfig.entry.app.unshift('webpack-hot-middleware/client');
    clientConfig.devtool = 'cheap-module-eval-source-map';
    clientConfig.devServer = {
        contentBase: DIRS.static,
        hot: true,
        inline: true,
        noInfo: true,
        historyApiFallback: true
    };

    const sassLoader = _.find(clientConfig.module.loaders, {key: 'sass'});
    sassLoader.loader = styleLoaders.join('!');

    removePlugins(clientConfig, [ExtractTextPlugin, AssetsPlugin]);
    clientConfig.plugins = clientConfig.plugins.concat([
        new OccurenceOrderPlugin(),
        new HotModuleReplacementPlugin(),
        new NoErrorsPlugin()
    ]);

    // Build only client
    module.exports = clientConfig;

} else {

    const serverConfig = _.extend({}, clientConfig, {
        target: 'node',
        entry: {
            server: path.join(DIRS.server, 'entry.js')
        },
        output: {
            path: DIRS.build,
            filename: 'server.js',
            libraryTarget: 'commonjs2'
        }
    });
    removePlugins(serverConfig, [
        CommonsChunkPlugin,
        HtmlWebpackPlugin,
        DefinePlugin,
        ProvidePlugin
    ]);
    serverConfig.plugins = serverConfig.plugins.concat([
        new DefinePlugin({
            CONFIG: JSON.stringify(params.get('')),
            DEBUG: params.get('DEBUG'),
            ENV: JSON.stringify(params.get('ENV')),
            BROWSER: false
        })
    ]);

    // Build both server and client
    module.exports = [
        clientConfig,
        serverConfig
    ];
}