/* eslint-disable global-require, no-console */
import 'babel-polyfill';
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';
import './rx';

import createContainer from './ioc-container';
import { createStore, AppContainer, listenForServerEvents } from './root';

if (COMPILATION_MODE === 'debug') {
  window.React = React;
}

function render({ RootComponent, store, history }) {
  ReactDOM.render(
    <RootComponent
      history={history}
      store={store}
    />,
    document.getElementById('root'),
  );
}

function run() {
  const state = {};
  const imports = {
    storage: window.localStorage,
    history: browserHistory,
  };
  const container = createContainer(imports, {
    ...BUILD_CONFIG,
    userStorageKey: 'conreign.user',
  });
  const store = createStore({ state, container });
  const props = {
    RootComponent: AppContainer,
    store,
    history: browserHistory,
  };
  render(props);
  store.dispatch(listenForServerEvents());
  return props;
}

const rootProps = run();

// Hot-Reload
if (TASK === 'run') {
  if (module.hot) {
    // eslint-disable-next-line
    function update() {
      const UpdatedRoot = require('./root/app-container-hot').default;
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

    module.hot.accept('./root/app-container-hot', update);
    module.hot.accept('./root/index', update);
  }
}
