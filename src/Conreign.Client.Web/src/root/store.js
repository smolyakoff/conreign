import { createStore, applyMiddleware } from 'redux';
import { createEpicMiddleware } from 'redux-observable';
import { composeWithDevTools } from 'redux-devtools-extension/developmentOnly';

import main from './root';

const { createEpic, reducer } = main;

function createEnhancer(container) {
  const epic = createEpic(container);
  const epicMiddleware = createEpicMiddleware(epic);
  return composeWithDevTools(applyMiddleware(epicMiddleware));
}

function createDebugStore({ state = {}, container }) {
  const enhancer = createEnhancer(container);
  const store = createStore(reducer, state, enhancer);
  if (module.hot) {
    module.hot.accept('./root', () => {
      // eslint-disable-next-line
      const updatedReducer = require('./root').default.reducer;
      store.replaceReducer(updatedReducer);
    });
  }
  return store;
}

function createReleaseStore({ state = {}, container }) {
  const enhancer = createEnhancer(container);
  return createStore(reducer, state, enhancer);
}

export function createApplicationStore(options) {
  const store = process.env.NODE_ENV === 'production'
    ? createReleaseStore(options)
    : createDebugStore(options);
  return store;
}

export default createApplicationStore;
