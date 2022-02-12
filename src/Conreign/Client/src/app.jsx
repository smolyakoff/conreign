import ReactDOM from 'react-dom';
import React from 'react';
import { get } from 'lodash';
import { createBrowserHistory } from 'history';

import { createApiClient } from './api';
import { ValueStore, createAsyncOperationDispatcher } from './framework';
import { createStore, AppContainer } from './root';

export default function createApplication(config) {
  const {
    userStorageKey,
    apiServerUrl,
  } = config;

  const history = createBrowserHistory({
    basename: window.location.pathname,
  });
  const storage = window.localStorage;
  const userStore = new ValueStore(storage, { key: userStorageKey });
  const accessTokenProvider = () => get(userStore.get(), 'accessToken');
  const apiClientOptions = { baseUrl: apiServerUrl };
  const apiClient = createApiClient(accessTokenProvider, apiClientOptions);
  const apiDispatcher = createAsyncOperationDispatcher(apiClient.send);
  const container = {
    history,
    apiDispatcher,
    apiClient,
    userStore,
  };
  const store = createStore({
    container,
    state: {},
  });
  const rootElement = document.getElementById('root');

  function render(Root) {
    ReactDOM.render(
      <Root
        history={history}
        store={store}
      />,
      rootElement,
    );
  }

  function start() {
    render(AppContainer);
    if (module.hot) {
      // eslint-disable-next-line no-inner-declarations
      function update() {
        // eslint-disable-next-line global-require
        const UpdatedRoot = require('./root/app-container').default;
        render(UpdatedRoot);
      }
      module.hot.accept('./root/app-container', update);
      module.hot.accept('./root/index', update);
    }
  }

  return start;
}
