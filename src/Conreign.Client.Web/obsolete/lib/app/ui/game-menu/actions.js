'use strict';
import {definePromiseAction} from 'core/core';
import {ApiClient} from 'api/api';

export const arrive = definePromiseAction({
    type: 'ARRIVE',
    dependencies: [ApiClient],
    mapPayload: (api) => api.dispatch({type: 'arrive'})
});