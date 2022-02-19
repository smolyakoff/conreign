const path = require('path');

const _ = require('lodash');
const {
  DllReferencePlugin,
  DefinePlugin,
} = require('webpack');
const { merge: webpackMerge } = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const AddAssetToHtmlPlugin = require('add-asset-html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const LodashModuleReplacementPlugin = require('lodash-webpack-plugin');
const ESLintPlugin = require('eslint-webpack-plugin');

const {PATHS, TASK, COMPILATION_MODE, SIZE_LIMITS} = require('./constants');

function createConfiguration(options) {
  const {compilationMode, task} = options;
  // eslint-disable-next-line global-require
  const conreignLibAssets = require('./../build/conreign-lib.assets.json');
  console.log(conreignLibAssets);
  let config = {
    mode: compilationMode === COMPILATION_MODE.DEBUG ? 'development' : 'production',
    entry: {
      app: [
        'core-js',
        'regenerator-runtime/runtime',
        path.join(PATHS.SRC, 'index.jsx'),
      ],
    },
    output: {
      path: PATHS.BUILD,
      filename: task === TASK.RUN ? 'conreign-[name].js' : 'conreign-[name].[chunkhash].js',
      publicPath: '/',
    },
    devtool: compilationMode === COMPILATION_MODE.DEBUG ? 'inline-source-map' : 'source-map',
    module: {
      rules: [
        {
          test: /\.html$/,
          loader: 'html-loader',
        },
        {
          test: /\.jsx?$/,
          loader: 'babel-loader',
          include: PATHS.SRC,
          options: {
            cacheDirectory: true,
          },
        },
        {
          test: /\.scss$/,
          use: [
            task === TASK.RUN
              ? 'style-loader'
              : MiniCssExtractPlugin.loader,
            'css-loader',
            'sass-loader',
            'postcss-loader'
          ],
        },
        {
          test: /\.svg$/,
          loader: 'svg-sprite-loader',
        },
      ],
    },
    resolve: {
      extensions: ['.js', '.jsx', '.json'],
    },
    plugins: [
      new ESLintPlugin({
        context: PATHS.SRC,
        failOnWarning: task === TASK.BUILD,
        failOnError: task === TASK.BUILD,
      }),
      new DllReferencePlugin({
        context: PATHS.ROOT,
        // eslint-disable-next-line global-require
        manifest: require('./../build/conreign-lib.manifest.json'),
      }),
      new HtmlWebpackPlugin({
        template: path.join(PATHS.SRC, 'index.html'),
        favicon: path.join(PATHS.SRC, 'favicon.ico'),
      }),
      new AddAssetToHtmlPlugin({
        filepath: path.join(PATHS.BUILD, conreignLibAssets.lib.js.replace(/^auto\//, '')),
        includeSourcemap: false,
      }),
      new MiniCssExtractPlugin({
        filename: 'conreign-[name].[contenthash].css',
      }),
      new DefinePlugin({
        'process.env.NODE_ENV': JSON.stringify(
          compilationMode === COMPILATION_MODE.RELEASE ? 'production' : 'development'
        ),
        COMPILATION_MODE: JSON.stringify(compilationMode),
        TASK: JSON.stringify(options.task),
        BUILD_CONFIG: JSON.stringify(options),
      }),
      // new LodashModuleReplacementPlugin({
      //   collections: true,
      //   deburring: true,
      //   unicode: true,
      //   paths: true,
      //   currying: true,
      // }),
    ],
    performance: _.extend({}, SIZE_LIMITS, {
      hints: options.compilationMode === COMPILATION_MODE.RELEASE ? 'warning' : false,
    }),
  };

  if (options.task === TASK.RUN) {
    config = webpackMerge(config, {
      entry: {
        app: [
          'react-hot-loader/patch',
          `webpack-dev-server/client?http://localhost:${options.devServerPort}`,
          'webpack/hot/only-dev-server',
        ],
      }
    });
  }

  return config;
}

module.exports = createConfiguration;
