import { combineReducers, compose, createStore } from 'redux';
import { routerReducer } from 'react-router-redux';

const reducer = combineReducers({
  routing: routerReducer,
});

function createDebugStore({ state = {}, DevTools }) {
  const enhancer = compose(
    DevTools.instrument(),
  );
  return createStore(reducer, state, enhancer);
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
