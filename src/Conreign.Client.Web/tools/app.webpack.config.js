const path = require('path');

const _ = require('lodash');
const {
  DllReferencePlugin,
  HotModuleReplacementPlugin,
  NamedModulesPlugin,
} = require('webpack');
const webpackMerge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const AddAssetToHtmlPlugin = require('add-asset-html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

const { PATHS, TASK, COMPILATION_MODE, SIZE_LIMITS } = require('./constants');

function createConfiguration(options) {
  const { compilationMode } = options;
  const conreignLibAssets = require('./../build/conreign-lib.assets.json');
  let config = {
    entry: {
      app: [path.join(PATHS.SRC, 'app.js')],
    },
    output: {
      path: PATHS.BUILD,
      filename: options.task === TASK.RUN ? 'conreign-[name].js' : 'conreign-[name].[chunkhash].js',
    },
    devtool: compilationMode === COMPILATION_MODE.DEBUG ? 'inline-source-map' : 'source-map',
    module: {
      rules: [
        {
          test: /\.html$/,
          loader: 'html-loader',
        },
        {
          test: /\.js$/,
          loader: 'babel-loader',
          include: PATHS.SRC,
        },
        {
          test: /\.scss$/,
          loader: ExtractTextPlugin.extract({
            fallbackLoader: 'style-loader',
            loader: [
              'css-loader?importLoaders=1',
              'postcss-loader',
              'sass-loader',
            ]
          })
        }
      ]
    },
    plugins: [
      new DllReferencePlugin({
        context: PATHS.ROOT,
        manifest: require('./../build/conreign-lib.manifest.json'),
      }),
      new HtmlWebpackPlugin({
        template: path.join(PATHS.SRC, 'index.html'),
      }),
      new AddAssetToHtmlPlugin({
        filepath: path.join(PATHS.BUILD, conreignLibAssets.lib.js),
        includeSourcemap: compilationMode === COMPILATION_MODE.RELEASE,
      }),
      new ExtractTextPlugin({
        filename: 'conreign-[name].[contenthash].css',
        disable: options.task === TASK.RUN,
      }),
    ],
    performance: _.clone(SIZE_LIMITS),
  };

  if (options.task === TASK.RUN) {
    config = webpackMerge(config, {
      entry: {
        app: [
          `webpack-dev-server/client?http://localhost:${options.devServerPort}`,
          'webpack/hot/only-dev-server',
        ]
      },
      plugins: [
        new HotModuleReplacementPlugin(),
        new NamedModulesPlugin(),
      ]
    });
  }
  return config;
}

module.exports = createConfiguration;
