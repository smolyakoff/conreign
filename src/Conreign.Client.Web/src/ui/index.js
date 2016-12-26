/* eslint-disable global-require */
export { default as createStore } from './store';

export const Root = TASK === 'run'
  ? require('./root-hot').default
  : require('./root');

export const createDevTools = COMPILATION_MODE === 'debug'
  ? require('./dev-tools').default
  : () => null;

