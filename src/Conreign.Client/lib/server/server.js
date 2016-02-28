global.self = {};
global.ROOT_DIR = __dirname;
const render = require('./build/server').render;
const config = require('./server.config');

module.exports = {
    render: render,
    config: config
};