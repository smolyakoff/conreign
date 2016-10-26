'use strict';
const fs = require('fs');

const _ = require('lodash');
const chalk = require('chalk');

function load(name, env) {
    var patterns = [
        name + '.json',
        name + '.js'
    ];
    const files = fs.readdirSync(__dirname);
    const exists = _.intersection(files, patterns).length > 0;
    const config = exists ? require(`./${name}`) : {};
    return config[env] || {};
}

function validate(container) {
    try {
        container.validate();
    } catch(e) {
        var message = 'Invalid or missing configuration. Please ensure that you have all required configuration files on your machine.';
        console.log(chalk.red(message));
        console.log(chalk.yellow(e.message));
        throw new Error('Invalid configuration');
    }
}

module.exports = {
    validate: validate,
    load: load
};