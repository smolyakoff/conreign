'use strict';
const config = require('./lib/server/server.config.js');
const ENV = config.get('ENV');

global.ENV = ENV;
global.CONFIG = config.get();
global.DEBUG = false;
global.BROWSER = false;

const app = ENV === 'development'
    ? require('./dev-server')
    : require('./live-server');

app.listen(3000);