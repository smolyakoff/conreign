'use strict';
const config = require('./server.config');
const ENV = config.get('ENV');

const app = ENV === 'development'
    ? require('./development-server')
    : require('./live-server');

app.listen(3003);