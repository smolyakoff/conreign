'use strict';
const config = require('./server.config');
const ENV = config.get('ENV');

global.ENV = ENV;
global.CONFIG = config.get();
global.DEBUG = false;
global.BROWSER = false;

const app = ENV === 'development'
    ? require('./development-server')
    : require('./live-server');

app.listen(3003);