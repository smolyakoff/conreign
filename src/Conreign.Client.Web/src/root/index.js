/* eslint-disable global-require */
export { default as createStore } from './store';

export const AppContainer = TASK === 'run'
  ? require('./app-container-hot').default
  : require('./app-container').default;

export * from './root';

