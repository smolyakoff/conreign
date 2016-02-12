'use strict';
const path = require('path');

const React = require('React');
const historyLib = require('history');
const express = require('express');
const reactRouter = require('react-router');
const dom = require('react-dom/server');

const boot = require('./../client/src/boot');
const routesFactory = require('./../client/src/ui/routes');
const htmlTemplate = require('./html');

const match = reactRouter.match;
const RouterContext = reactRouter.RouterContext;

const app = express();

const dist = path.join(__dirname, './../client/dist');
const history = historyLib.createMemoryHistory();
const store = boot.createReduxStore(history, {});
const routes = routesFactory.createRoutes(store);

app.use(express.static(dist, {index: false}));

app.use((req, res) => {
    match({ routes, location: req.url }, (error, redirectLocation, renderProps) => {
        if (error) {
            res.status(500).send(error.message)
        } else if (redirectLocation) {
            res.redirect(302, redirectLocation.pathname + redirectLocation.search)
        } else if (renderProps) {
            // You can also check renderProps.components or renderProps.routes for
            // your "not found" component or route respectively, and send a 404 as
            // below, if you're using a catch-all route.
            const content = dom.renderToString(<RouterContext {...renderProps} />);
            const html = htmlTemplate(store, content);
            res.status(200).send(html);
        } else {
            res.status(404).send('Not found')
        }
    });
});

module.exports = app;