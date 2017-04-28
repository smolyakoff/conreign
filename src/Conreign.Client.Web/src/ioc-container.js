import { get } from 'lodash';

import { createApiClient } from './api';
import { ValueStore, createAsyncOperationDispatcher } from './framework';

export default function createContainer(imports, config) {
  const { storage, history } = imports;
  const { apiServerUrl } = config;

  const userStore = new ValueStore(storage, {
    key: config.userStorageKey,
  });
  const accessTokenProvider = () => get(userStore.get(), 'accessToken');
  const apiClient = createApiClient(accessTokenProvider, {
    baseUrl: apiServerUrl,
  });
  const apiDispatcher = createAsyncOperationDispatcher(apiClient.send);
  return {
    history,
    apiClient,
    apiDispatcher,
    accessTokenProvider,
    userStore,
  };
}
