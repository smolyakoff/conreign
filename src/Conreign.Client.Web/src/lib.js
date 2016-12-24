import 'babel-polyfill';
import 'react';
import 'react-dom';
import 'redux';
import 'react-redux';
import 'react-router';
import 'react-router-redux';

if (TASK === 'run') {
  require('react-hot-loader');
}

if (COMPILATION_MODE === 'debug') {
  require('redux-devtools');
  require('redux-devtools-dock-monitor');
  require('redux-devtools-inspector');
}
