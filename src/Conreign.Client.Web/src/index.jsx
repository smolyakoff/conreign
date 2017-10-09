/* eslint-disable global-require, no-console */
import 'babel-polyfill';
import React from 'react';
import ReactDOM from 'react-dom';
import { createBrowserHistory } from 'history';
import './rx';

import {
  createStore,
  AppContainer,
  createContainer,
} from './root';

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
  const history = createBrowserHistory();
  const state = {};
  const imports = {
    storage: window.localStorage,
    history,
  };
  const container = createContainer(imports, {
    ...BUILD_CONFIG,
    userStorageKey: 'conreign.user',
  });
  const store = createStore({ state, container });
  const props = {
    RootComponent: AppContainer,
    store,
    history,
  };
  render(props);
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

    module.hot.accept('./root/app-container-hot', update);
    module.hot.accept('./root/index', update);
  }
}
