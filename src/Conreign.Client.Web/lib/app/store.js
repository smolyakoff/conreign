'use strict';
import {createStore, applyMiddleware, compose} from 'redux';
import thunkMiddleware from 'redux-thunk';
import {syncHistory} from 'react-router-redux';

import {rootReducer} from './reducer';

export function createAppStore(history, data) {
    const middleware = [
        syncHistory(history),
        thunkMiddleware
    ];
    const compositions = [
        applyMiddleware(...middleware)
    ];

    if (ENV === 'development') {
        const DevTools = require('./dev-tools').default;
        compositions.push(DevTools.instrument());
    }

    const store = createStore(
        rootReducer,
        data,
        compose(...compositions)
    );

    // Hot reload reducers (requires Webpack or Browserify HMR to be enabled)
    if (module.hot) {
        module.hot.accept('./reducer', () =>
            store.replaceReducer(require('./reducer').default)
        );
    }

    return store;
}

export default createAppStore;