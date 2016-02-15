'use strict';
import React from 'react';
import {createAppStore} from './store';
import {createRoutes} from './routes';

export function init(history, data = {}) {
    const store = createAppStore(history, data);
    const routes = createRoutes(store);

    let DevTools = null;
    if (ENV === 'development') {
        DevTools = require('./dev-tools').default;
    }

    const app = {
        store: store,
        routes: routes
    };

    if (DevTools) {
        app.devTools = <DevTools/>;
    }

    return app;
}

export default init;