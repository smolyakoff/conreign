'use strict';
const path = require('path');

const express = require('express');
const webpack = require('webpack');
const webpackDev = require('webpack-dev-middleware');
const webpackHot = require('webpack-hot-middleware');

const webpackConfig = require('./webpack.config.js');

const app = express();

const compiler = webpack(webpackConfig);
const devMiddleware = webpackDev(compiler, {
    noInfo: true
});
app.use(devMiddleware);
app.use(webpackHot(compiler));
app.get('*', (req, res) => {
    const indexPath = path.join(webpackConfig.output.path, 'index.html');
    const index = devMiddleware.fileSystem.readFileSync(indexPath);
    res.end(index);
});

module.exports = app;