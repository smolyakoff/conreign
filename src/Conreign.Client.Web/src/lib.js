/* eslint-disable global-require,import/no-extraneous-dependencies */
import 'babel-polyfill';
import 'jquery';
import 'ms-signalr-client';
import 'rxjs/Rx';
import 'react';
import 'react-transition-group';
import 'react-dom';
import 'redux';
import 'recompose';
import 'revalidate';
import 'reselect';
import 'react-redux';
import 'redux-observable';
import 'react-router-dom';
import 'react-router-config';
import 'redux-devtools-extension/developmentOnly';
import 'react-measure';
import 'react-virtualized';
import 'classnames';
import 'color';
import 'bem-cn';
import 'serialize-error';
import 'jwt-decode';
import 'xregexp/src/xregexp';
import 'xregexp/src/addons/unicode-base';
import 'xregexp/src/addons/unicode-categories';

if (TASK === 'run') {
  require('react-hot-loader');
}
