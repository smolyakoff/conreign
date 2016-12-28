/* eslint-disable global-require, no-console */
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux';

import createContainer from './core';
import { createStore, Root } from './ui';

if (COMPILATION_MODE === 'debug') {
  window.React = React;
}

function render({ RootComponent, store, DevTools, history }) {
  ReactDOM.render(
    <RootComponent
      history={history}
      store={store}
      DevTools={DevTools}
    />,
    document.getElementById('root'),
  );
}

// Hot-Reload
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

function run() {
  const state = {};
  const container = createContainer(BUILD_CONFIG);
  const store = createStore(state);
  const history = syncHistoryWithStore(browserHistory, store);
  render({
    RootComponent: Root,
    store,
    history,
    DevTools: store.DevTools,
  });

  const { apiClient } = container;
  apiClient.connect();
}

run();
