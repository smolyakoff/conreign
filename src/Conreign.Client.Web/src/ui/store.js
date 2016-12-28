import { compose, createStore } from 'redux';

import reducer from './reducer';

const DevTools = COMPILATION_MODE === 'debug'
  ? require('./dev-tools').default()
  : null;

function createDebugStore(state = {}) {
  const enhancer = compose(
    DevTools.instrument(),
  );
  const store = createStore(reducer, state, enhancer);
  if (module.hot) {
    module.hot.accept('./reducer', () => {
      // eslint-disable-next-line
      const updatedReducer = require('./reducer').default;
      store.replaceReducer(updatedReducer);
    });
  }
  return store;
}

function createReleaseStore(state = {}) {
  return createStore(reducer, state);
}

export function createApplicationStore(options) {
  const store = process.env.NODE_ENV === 'production'
    ? createReleaseStore(options)
    : createDebugStore(options);
  store.DevTools = DevTools;
  return store;
}

export default createApplicationStore;
