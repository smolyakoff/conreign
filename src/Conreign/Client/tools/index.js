const createLibWebpackConfig = require('./lib.webpack.config');
const createAppWebpackConfig = require('./app.webpack.config');
const constants = require('./constants');
const loadVariables = require('./vars');

module.exports = {
  constants,
  loadVariables,
  createLibWebpackConfig,
  createAppWebpackConfig,
};
