const path = require('path');

const COMPILATION_MODE = {
  DEBUG: 'debug',
  RELEASE: 'release',
};

const TRACE_LEVEL = {
  MINIMAL: 'minimal',
  NORMAL: 'normal',
  VERBOSE: 'verbose',
};

const TASK = {
  RUN: 'run',
  BUILD: 'build',
};

const ROOT = path.resolve(__dirname, '..');
const PATHS = {
  ROOT,
  SRC: path.join(ROOT, 'src'),
  BUILD: path.join(ROOT, 'build'),
};

const SIZE_LIMITS = {
  maxEntrypointSize: 1000000, // 1MB
  maxAssetSize: 1000000, // 1MB
};

const SUPPORTED_BROWSERS = '> 5%';

module.exports = {
  COMPILATION_MODE,
  TRACE_LEVEL,
  PATHS,
  TASK,
  SIZE_LIMITS,
  SUPPORTED_BROWSERS,
};
