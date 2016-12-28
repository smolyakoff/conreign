import createApiClient from './api-client';

export default function createContainer(config) {
  const apiClient = createApiClient({
    baseUrl: config.apiServerUrl,
  });

  return {
    apiClient,
  };
}
