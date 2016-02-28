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
    },
    SESSION_SECRET: {
        format: String,
        doc: 'Session cryptographic key.',
        default: 'TheF0rceAwakens!'
    },
    MONGO_URL: {
        format: String,
        doc: 'Mongo connection string.',
        default: 'mongodb://localhost/conreign-sessions'
    }
});

const config = {
};

const env = container.get('ENV');
container.load(config[env] || {});

utility.validate(container);

module.exports = container;