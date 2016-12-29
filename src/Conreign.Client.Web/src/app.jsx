/* eslint-disable global-require, no-console */
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux';

import createContainer from './core';
import { createStore, Root, connectToServer } from './ui';

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

function run() {
  const state = {};
  const imports = { storage: window.localStorage };
  const container = createContainer(imports, {
    ...BUILD_CONFIG,
    userStorageKey: 'conreign.user',
  });
  const store = createStore({ state, container });
  const history = syncHistoryWithStore(browserHistory, store);
  const props = {
    RootComponent: Root,
    store,
    history,
    DevTools: store.DevTools,
  };
  render(props);
  store.dispatch(connectToServer());
  return props;
}

const rootProps = run();

// Hot-Reload
if (TASK === 'run') {
  if (module.hot) {
    // eslint-disable-next-line
    function update() {
      const UpdatedRoot = require('./ui/main/root-hot').default;
      render({
        ...rootProps,
        RootComponent: UpdatedRoot,
      });
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

    module.hot.accept('./ui/main/root-hot', update);
    module.hot.accept('./ui/index', update);
  }
}
