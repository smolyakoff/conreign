const path = require('path');
const _ = require('lodash');

const {
  DllPlugin,
} = require('webpack');
const AssetsWebpackPlugin = require('assets-webpack-plugin');

const { PATHS, SIZE_LIMITS } = require('./constants');
const CHUNK_NAME = 'lib';
const LIB_VARIABLE_NAME = 'conreignLib';

function createConfiguration(options) {
  const config = {
    entry: {
      [CHUNK_NAME]: [path.join(PATHS.SRC, 'lib.js')],
    },
    output: {
      path: PATHS.BUILD,
      filename: 'conreign-[name].[chunkhash].js',
      library: LIB_VARIABLE_NAME
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
    ],
    performance: _.clone(SIZE_LIMITS),
  };
  return config;
}

module.exports = createConfiguration;
