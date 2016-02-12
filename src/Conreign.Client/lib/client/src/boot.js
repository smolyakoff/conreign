'use strict';
import {createStore, applyMiddleware, compose} from 'redux';
import {syncHistory} from 'react-router-redux';

import {rootReducer} from './ui/ui';

export function createReduxStore(history, data) {
    const middleware = [
        syncHistory(history)
    ];
    const store = createStore(
        rootReducer,
        data,
        compose(
            applyMiddleware(...middleware)
            //DevTools.instrument()
        )
    );
    return store;
}

export default createReduxStore;