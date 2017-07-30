import { createStore, applyMiddleware } from 'redux';
import { createEpicMiddleware } from 'redux-observable';
import { composeWithDevTools } from 'redux-devtools-extension/developmentOnly';
import { get } from 'lodash';

import reducer from './../state';
import epic from './root-epic';

function createEnhancer(container) {
  const epicMiddleware = createEpicMiddleware(epic, {
    dependencies: container,
  });
  const devToolsOptions = {
    predicate: (state, action) => !get(action, 'meta.$hideFromDevTools'),
  };
  return composeWithDevTools(devToolsOptions)(applyMiddleware(epicMiddleware));
}

function createDebugStore({ state = {}, container }) {
  const enhancer = createEnhancer(container);
  const store = createStore(reducer, state, enhancer);
  if (module.hot) {
    module.hot.accept('./../state', () => {
      // eslint-disable-next-line
      const updatedReducer = require('./../state').default;
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
