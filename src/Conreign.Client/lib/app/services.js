'use strict';
import {services} from './core/core';
import {ApiClient} from './api/api';

export function configure(imports, options) {
    const storage = imports.storage;

    const apiOptions = {apiUrl: options.API_URL, tokenKey: 'conreign.token'};
    const apiClient = new ApiClient(storage, apiOptions);

    services.register(ApiClient, apiClient);

    return services;
}

export default configure;