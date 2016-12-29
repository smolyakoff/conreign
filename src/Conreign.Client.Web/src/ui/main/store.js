import { compose, createStore, applyMiddleware } from 'redux';
import { createEpicMiddleware } from 'redux-observable';

import main from './main';

const { createEpic, reducer } = main;
const DevTools = COMPILATION_MODE === 'debug'
  ? require('./dev-tools').default()
  : null;

function createEnhancer(container) {
  const epic = createEpic(container);
  const epicMiddleware = createEpicMiddleware(epic);
  const enhancers = [
    applyMiddleware(epicMiddleware),
    DevTools ? DevTools.instrument() : null,
  ].filter(x => x);
  return compose(...enhancers);
}

function createDebugStore({ state = {}, container }) {
  const enhancer = createEnhancer(container);
  const store = createStore(reducer, state, enhancer);
  if (module.hot) {
    module.hot.accept('./main', () => {
      // eslint-disable-next-line
      const updatedReducer = require('./main').default.reducer;
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
  store.DevTools = DevTools;
  return store;
}

export default createApplicationStore;
