import { compose, createStore } from 'redux';

import reducer from './reducer';

function createDebugStore({ state = {}, DevTools }) {
  const enhancer = compose(
    DevTools.instrument(),
  );
  const store = createStore(reducer, state, enhancer);
  if (module.hot) {
    module.hot.accept('./reducer', () => {
      const updatedReducer = require('./reducer').default;
      store.replaceReducer(updatedReducer);
    })
  }
  return store;
}

function createReleaseStore({ state = {} }) {
  return createStore(reducer, state);
}

export function createApplicationStore(options) {
  return process.env.NODE_ENV === 'production'
    ? createReleaseStore(options)
    : createDebugStore(options);
}

export default createApplicationStore;
