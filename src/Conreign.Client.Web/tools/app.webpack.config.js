const path = require('path');

const _ = require('lodash');
const {
  DllReferencePlugin,
  HotModuleReplacementPlugin,
  NamedModulesPlugin,
  DefinePlugin,
  LoaderOptionsPlugin,
  optimize,
} = require('webpack');
const webpackMerge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const AddAssetToHtmlPlugin = require('add-asset-html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const LodashModuleReplacementPlugin = require('lodash-webpack-plugin');

const { UglifyJsPlugin } = optimize;
const { PATHS, TASK, COMPILATION_MODE, SIZE_LIMITS } = require('./constants');

function createConfiguration(options) {
  const { compilationMode } = options;
  // eslint-disable-next-line global-require
  const conreignLibAssets = require('./../build/conreign-lib.assets.json');
  let config = {
    entry: {
      app: [path.join(PATHS.SRC, 'app')],
    },
    output: {
      path: PATHS.BUILD,
      filename: options.task === TASK.RUN ? 'conreign-[name].js' : 'conreign-[name].[chunkhash].js',
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
          enforce: 'pre',
          loader: 'eslint-loader',
          include: PATHS.SRC,
        },
        {
          test: /\.jsx?$/,
          loader: 'babel-loader',
          include: PATHS.SRC,
        },
        {
          test: /\.scss$/,
          loader: ExtractTextPlugin.extract({
            fallback: 'style-loader',
            use: [
              'css-loader?importLoaders=1',
              'postcss-loader',
              'sass-loader',
            ],
          }),
        },
        {
          test: /\.svg$/,
          loader: 'file-loader',
        },
      ],
    },
    resolve: {
      extensions: ['.js', '.jsx', '.json'],
    },
    plugins: [
      new DllReferencePlugin({
        context: PATHS.ROOT,
        // eslint-disable-next-line global-require
        manifest: require('./../build/conreign-lib.manifest.json'),
      }),
      new HtmlWebpackPlugin({
        template: path.join(PATHS.SRC, 'index.html'),
      }),
      new AddAssetToHtmlPlugin({
        filepath: path.join(PATHS.BUILD, conreignLibAssets.lib.js),
        includeSourcemap: false,
      }),
      new ExtractTextPlugin({
        filename: 'conreign-[name].[contenthash].css',
        disable: options.task === TASK.RUN,
      }),
      new DefinePlugin({
        'process.env.NODE_ENV': JSON.stringify(
          compilationMode === COMPILATION_MODE.RELEASE ? 'production' : 'development'
        ),
        COMPILATION_MODE: JSON.stringify(compilationMode),
        TASK: JSON.stringify(options.task),
        BUILD_CONFIG: JSON.stringify(options),
      }),
      new LodashModuleReplacementPlugin({
        collections: true,
        deburring: true,
        unicode: true,
        paths: true,
        currying: true,
      }),
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
      },
      plugins: [
        new HotModuleReplacementPlugin(),
        new NamedModulesPlugin(),
      ],
    });
  }

  if (compilationMode === COMPILATION_MODE.RELEASE) {
    config = webpackMerge(config, {
      plugins: [
        new UglifyJsPlugin({
          sourceMap: true,
        }),
        new LoaderOptionsPlugin({
          minimize: true,
        }),
      ],
    });
  }

  return config;
}

module.exports = createConfiguration;
