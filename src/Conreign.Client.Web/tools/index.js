const createLibWebpackConfig = require('./lib.webpack.config');
const createAppWebpackConfig = require('./app.webpack.config');

module.exports = {
  constants: require('./constants'),
  loadVariables: require('./vars'),
  createLibWebpackConfig,
  createAppWebpackConfig,
};
