'use strict';
import 'babel-polyfill';
import fs from 'fs';
import path from 'path';

import Promise from 'bluebird';
import _ from 'lodash';
import React from 'react';
import {createMemoryHistory} from 'history';
import {match} from 'react-router';
import dom from 'react-dom/server';
import serializeJs from 'serialize-javascript';


import {SessionStorage} from './session-storage';
import {Root} from './root';
import {init} from './../app/app';

const history = createMemoryHistory();
const index = fs.readFileSync(path.join(ROOT_DIR, 'static/index.html'), 'utf8');

function load(renderProps, store) {
    const componentsToResolve = renderProps.components.filter(c => _.isFunction(c.resolve));
    const props = {
        location: renderProps.location,
        params: renderProps.params,
        dispatch: store.dispatch.bind(store)
    };
    const results = componentsToResolve.map(c => c.resolve(props));
    return Promise.all(results);
}

function renderPage(store, context, content) {
    const state = store.getState();
    const part =
        `<div id="root">${content}</div>
         <script type="text/javascript">
            window.__INITIAL_STATE__ = ${serializeJs(state)};
            window.__INITIAL_CONTEXT__ = ${serializeJs(context)};
         </script>`;
    const html = index.replace('<div id="root"></div>', part);
    return html;
}

export function render(req, res, next) {
    const storage = new SessionStorage(req.session);
    const app = init(history, storage);
    const store = app.store;
    const routes = app.routes;

    match({ routes, location: req.url }, (error, redirectLocation, renderProps) => {
        if (error) {
            next(error);
        } else if (redirectLocation) {
            res.redirect(302, redirectLocation.pathname + redirectLocation.search)
        } else if (renderProps) {
            // You can also check renderProps.components or renderProps.routes for
            // your "not found" component or route respectively, and send a 404 as
            // below, if you're using a catch-all route.

            load(renderProps, store)
                .then(() => {
                    const content = dom.renderToString(<Root store={store} renderProps={renderProps}/>);
                    const context = _.omit(req.session, ['id', 'cookie']);
                    const html = renderPage(store, context, content);
                    res.status(200).send(html);
                })
                .catch(next)
                .done();
        } else {
            res.status(404).send('Not found')
        }
    });
}

export default render;