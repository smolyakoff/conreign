const fs = require('fs');
const path = require('path');
const chalk = require('chalk');
//require('babel-polyfill');

try {
    //const babelConfig = JSON.parse(fs.readFileSync(path.join(__dirname, '.babelrc')));
    //require('babel-core/register')(babelConfig);
} catch (e) {
    //console.log(chalk.red(`Failed to read your .babelrc: ${e.message}`));
}

require('./server');