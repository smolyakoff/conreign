const convict = require('convict');
const _ = require('lodash');

const { COMPILATION_MODE, TRACE_LEVEL } = require('./constants');

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
    arg: 'port'
  }
});

function load() {
  config.validate({ strict: true });
  return config.getProperties();
}

module.exports = load;
