const path = require('path');
const _ = require('lodash');

const {
  DllPlugin,
  DefinePlugin,
} = require('webpack');
const AssetsWebpackPlugin = require('assets-webpack-plugin');
const { PATHS, SIZE_LIMITS, COMPILATION_MODE } = require('./constants');

const CHUNK_NAME = 'lib';
const LIB_VARIABLE_NAME = 'conreignLib';

function createConfiguration({ compilationMode }) {
  let config = {
    mode: compilationMode === COMPILATION_MODE.DEBUG ? 'development' : 'production',
    entry: {
      [CHUNK_NAME]: [path.join(PATHS.SRC, 'lib.js')],
    },
    output: {
      path: PATHS.BUILD,
      filename: 'conreign-[name].[chunkhash].js',
      library: LIB_VARIABLE_NAME,
    },
    devtool: 'source-map',
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

  return config;
}

module.exports = createConfiguration;
