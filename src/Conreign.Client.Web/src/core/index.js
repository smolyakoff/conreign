import { get } from 'lodash';

import createApiClient from './api-client';
import ValueStore from './value-store';
import {
  asyncDispatcher as createAsyncDispatcher,
} from './async-dispatcher';

export * from './async-dispatcher';
export * from './gameplay';

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
  const apiDispatcher = createAsyncDispatcher(apiClient.send);
  return {
    history,
    apiClient,
    apiDispatcher,
    accessTokenProvider,
    userStore,
  };
}
