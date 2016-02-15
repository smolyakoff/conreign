'use strict';
const path = require('path');

const React = require('react');
const historyLib = require('history');
const express = require('express');
const reactRouter = require('react-router');
const dom = require('react-dom/server');

const init = require('./../app/app').init;
const indexPage = require('./index-page');
const load = require('./load');
const Root = require('./root');

const match = reactRouter.match;

const staticPath = path.join(__dirname, './static');
const history = historyLib.createMemoryHistory();
const app = init(history);
const store = app.store;
const routes = app.routes;

const server = express();

server.use(express.static(staticPath, {index: false}));

server.use((req, res, next) => {
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
                    const html = indexPage(store, content);
                    res.status(200).send(html);
                })
                .catch(next)
                .done();
        } else {
            res.status(404).send('Not found')
        }
    });
});

server.use((err, req, res, next) => {
    console.error(err.stack);
    res.status(500).send('Unexpected server error.');
});

module.exports = server;