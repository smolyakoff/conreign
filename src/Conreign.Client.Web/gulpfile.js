const _ = require('lodash');
const Promise = require('bluebird');
const gulp = require('gulp');
const del = require('del');
const WebpackDevServer = require('webpack-dev-server');
const webpack = require('webpack');
const webpackAsync = Promise.promisify(require('webpack'));
const { PluginError, log } = require('gulp-util');

const {
  createLibWebpackConfig,
  createAppWebpackConfig,
  loadVariables,
  constants,
} = require('./tools');

const { PATHS, TASK, COMPILATION_MODE } = constants;
const vars = loadVariables();
process.env.NODE_ENV = vars.compilationMode === COMPILATION_MODE.DEBUG ? 'development' : 'production';

function buildWithWebpack(configFactory) {
  const options = _.extend({}, vars, { task: TASK.BUILD });
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
  const options = _.extend({}, vars, { task: TASK.RUN });
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
        target: 'http://localhost:3000',
        ws: true,
      },
    },
  });
  server.listen(vars.devServerPort);
  log(`Webpack dev server is listening on ${vars.devServerPort}.`);
}

gulp.task('clean', () => del(PATHS.BUILD));

gulp.task('build:lib', ['clean'], () => buildWithWebpack(createLibWebpackConfig));
gulp.task('build:app', ['build:lib'], () => buildWithWebpack(createAppWebpackConfig));
gulp.task('build', ['build:app']);
gulp.task('develop', ['build:lib'], () => runWebpackServer(createAppWebpackConfig));
