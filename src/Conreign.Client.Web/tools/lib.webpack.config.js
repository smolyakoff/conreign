const path = require('path');
const _ = require('lodash');

const {
  DllPlugin,
  DefinePlugin,
  LoaderOptionsPlugin,
  optimize,
} = require('webpack');
const webpackMerge = require('webpack-merge');
const AssetsWebpackPlugin = require('assets-webpack-plugin');
const { PATHS, SIZE_LIMITS, COMPILATION_MODE } = require('./constants');

const { UglifyJsPlugin } = optimize;
const CHUNK_NAME = 'lib';
const LIB_VARIABLE_NAME = 'conreignLib';

function createConfiguration({ compilationMode }) {
  let config = {
    entry: {
      [CHUNK_NAME]: [path.join(PATHS.SRC, 'lib.js')],
    },
    output: {
      path: PATHS.BUILD,
      filename: 'conreign-[name].[chunkhash].js',
      library: LIB_VARIABLE_NAME,
    },
    plugins: [
      new DllPlugin({
        path: path.join(PATHS.BUILD, 'conreign-[name].manifest.json'),
        name: LIB_VARIABLE_NAME,
      }),
      new AssetsWebpackPlugin({
        path: PATHS.BUILD,
        filename: `conreign-${CHUNK_NAME}.assets.json`,
      }),
      new DefinePlugin({
        'process.env.NODE_ENV': JSON.stringify(
          compilationMode === COMPILATION_MODE.RELEASE ? 'production' : 'development'
        ),
      }),
    ],
    performance: _.extend({}, SIZE_LIMITS, {
      hints: compilationMode === COMPILATION_MODE.RELEASE ? 'warning' : false,
    }),
  };

  if (compilationMode === COMPILATION_MODE.RELEASE) {
    config = webpackMerge(config, {
      plugins: [
        new UglifyJsPlugin(),
        new LoaderOptionsPlugin({
          minimize: true,
        }),
      ],
    });
  }

  return config;
}

module.exports = createConfiguration;
