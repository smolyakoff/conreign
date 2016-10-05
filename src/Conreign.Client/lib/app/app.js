'use strict';
import _ from 'lodash';
import React from 'react';
import {configure} from './services';
import {createAppStore} from './store';
import {createRoutes} from './routes';

export function init(history, storage, data = {}) {

    const services = configure({storage: storage}, CONFIG);
    const store = createAppStore(history, data);
    const routes = createRoutes(store);

    let DevTools = null;
    if (ENV === 'development') {
        DevTools = require('./dev-tools').default;
    }

    const app = {
        store: store,
        routes: routes,
        services: services
    };

    if (DevTools) {
        app.devTools = <DevTools defaultIsVisible={false}/>;
    }

    if (BROWSER) {
        const toastr = require('toastr');
        const position = ENV === 'development' ? 'toast-bottom-left' : 'toast-bottom-right';
        toastr.options.positionClass = position;
    }

    return app;
}

export default init;