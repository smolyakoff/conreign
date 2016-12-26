/* eslint-disable global-require, no-console */
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux';

import { createStore, createDevTools, Root } from './ui';

if (COMPILATION_MODE === 'debug') {
  window.React = React;
}

const state = {};
const DevTools = createDevTools();
const store = createStore({ state, DevTools });
const history = syncHistoryWithStore(browserHistory, store);

function render(RootComponent) {
  ReactDOM.render(
    <RootComponent
      history={history}
      store={store}
      DevTools={DevTools}
    />,
    document.getElementById('root'),
  );
}

render(Root);

if (TASK === 'run') {
  if (module.hot) {
    // eslint-disable-next-line
    function update() {
      const UpdatedRoot = require('./ui/root-hot').default;
      render(UpdatedRoot);
    }

    // HACK: react-router v3 is incompatible with hot reload, so need to hide some odd messages
    // https://github.com/gaearon/react-hot-loader/issues/298
    const _ = require('lodash');
    const originalConsoleError = console.error.bind(console);
    console.error = (message) => {
      if (_.isString(message) && message.indexOf('You cannot change <Router routes>;') >= 0) {
        return;
      }
      originalConsoleError(message);
    };

    module.hot.accept('./ui/index', update);
    module.hot.accept('./ui/root-hot', update);
  }
}
