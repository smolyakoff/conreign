/* eslint-disable global-require,import/no-extraneous-dependencies */
import 'babel-polyfill';
import 'jquery';
import 'ms-signalr-client';
import 'rxjs/Rx';
import 'react';
import 'react-dom';
import 'redux';
import 'react-redux';

import 'redux-observable';
import 'react-router';
import 'react-router-redux';
import 'redux-devtools-extension/developmentOnly';
import 'classnames';
import 'bem-cn';
import 'serialize-error';
import 'xregexp/src/xregexp';
import 'xregexp/src/addons/unicode-base';
import 'xregexp/src/addons/unicode-categories';

if (TASK === 'run') {
  require('react-hot-loader');
}
