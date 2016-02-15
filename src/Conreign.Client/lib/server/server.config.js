'use strict';
const convict = require('convict');

const utility = require('./../config/config-utility');

const container = convict({
    ENV: {
        format: ['development', 'local', 'live'],
        default: 'development',
        env: 'NODE_ENV',
        arg: 'env',
        doc: 'The environment that we\'re running in.'
    },
    API_URL: {
        format: 'url',
        doc: 'Base URL for API server.',
        default: 'http://localhost:9000'
    }
});

const config = {
};

const env = container.get('ENV');
container.load(config[env] || {});

utility.validate(container);

module.exports = container;