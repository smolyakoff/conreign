/* eslint-disable global-require */
export { default as createStore } from './store';

export const RouterContainer = TASK === 'run'
  ? require('./router-container-hot').default
  : require('./router-container');

export * from './root';

