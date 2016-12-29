import { get } from 'lodash';

import createApiClient from './api-client';
import ValueStore from './value-store';

export default function createContainer(imports, config) {
  const userStore = new ValueStore(imports.storage, {
    key: config.userStorageKey,
  });
  const accessTokenProvider = () => get(userStore.get(), 'accessToken');
  const apiClient = createApiClient(accessTokenProvider, {
    baseUrl: config.apiServerUrl,
  });
  return {
    apiClient,
    accessTokenProvider,
    userStore,
  };
}
