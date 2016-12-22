'use strict';
const path = require('path');

const express = require('express');
const session = require('express-session');
const MongoStore = require('connect-mongo')(session);

const app = require('./lib/server/server');
const staticPath = path.join(__dirname, './lib/server/static');

const server = express();

server.use(express.static(staticPath, {index: false}));

server.use(session({
    secret: app.config.get('SESSION_SECRET'),
    rolling: true,
    store: new MongoStore({url: app.config.get('MONGO_URL')}),
    resave: false,
    saveUninitialized: true
}));

server.use(app.render);

server.use((err, req, res) => {
    console.error(err.stack);
    res.status(500).send('Unexpected server error.');
});

module.exports = server;