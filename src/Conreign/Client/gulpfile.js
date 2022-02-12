const _ = require('lodash');
const Promise = require('bluebird');
const {series} = require('gulp');
const del = require('del');
const WebpackDevServer = require('webpack-dev-server');
const webpack = require('webpack');
const webpackAsync = Promise.promisify(require('webpack'));
const {PluginError, log} = require('gulp-util');

const {
  createLibWebpackConfig,
  createAppWebpackConfig,
  loadVariables,
  constants,
} = require('./tools');

const {PATHS, TASK, COMPILATION_MODE} = constants;
const vars = loadVariables();
process.env.NODE_ENV = vars.compilationMode === COMPILATION_MODE.DEBUG ? 'development' : 'production';

function buildWithWebpack(configFactory) {
  const options = _.extend({}, vars, {task: TASK.BUILD});
  const config = configFactory(options);
  return webpackAsync(config)
    .then((stats) => {
      log(stats.toString(vars.traceLevel));
      if (stats.hasErrors()) {
        throw new Error('Compilation failed.');
      }
    })
    .catch((err) => {
      throw new PluginError('webpack', err);
    });
}

function runWebpackServer(configFactory) {
  const options = _.extend({}, vars, {task: TASK.RUN});
  const config = configFactory(options);
  const compiler = webpack(config);
  const server = new WebpackDevServer(compiler, {
    compress: true,
    hot: vars.compilationMode === COMPILATION_MODE.DEBUG,
    publicPath: '/',
    contentBase: PATHS.BUILD,
    stats: vars.traceLevel,
    historyApiFallback: true,
    proxy: {
      '/$/api': {
        target: process.env.ASPNETCORE_URLS || 'http://localhost:9001',
        ws: true,
      },
    },
  });
  server.listen(vars.devServerPort);
  log(`Webpack dev server is listening on ${vars.devServerPort}.`);
}


function clean() {
  return del(PATHS.BUILD);
}

function buildLib() {
  return buildWithWebpack(createLibWebpackConfig);
}

function buildApp() {
  return buildWithWebpack(createAppWebpackConfig);
}

function develop() {
  return runWebpackServer(createAppWebpackConfig);
}

module.exports = {
  clean,
  build: series(clean, buildLib, buildApp),
  develop: series(clean, buildLib, develop)
}
