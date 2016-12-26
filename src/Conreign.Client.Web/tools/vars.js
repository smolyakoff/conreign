const convict = require('convict');
const _ = require('lodash');
const git = require('git-rev-sync');

const { COMPILATION_MODE, TRACE_LEVEL } = require('./constants');
const packageJson = require('./../package.json');

function getGitVersion() {
  try {
    return {
      short: git.short(),
      long: git.long(),
      branch: git.branch(),
      tag: git.tag(),
    };
  } catch (e) {
    return {};
  }
}

const load = _.memoize(() => {
  const config = convict({
    compilationMode: {
      doc: 'Compilation mode.',
      format: _.values(COMPILATION_MODE),
      default: COMPILATION_MODE.DEBUG,
      env: 'COMPILATION_MODE',
      arg: 'mode',
    },
    traceLevel: {
      doc: 'Trace level.',
      format: _.values(TRACE_LEVEL),
      default: TRACE_LEVEL.MINIMAL,
      env: 'TRACE_LEVEL',
    },
    devServerPort: {
      doc: 'Development server port.',
      format: 'port',
      default: 9000,
      env: 'PORT',
      arg: 'port',
    },
  });
  config.validate({ strict: true });
  const props = config.getProperties();
  _.extend(props, {
    version: {
      main: packageJson.version,
      git: getGitVersion(),
    },
  });
  return props;
});


module.exports = load;
