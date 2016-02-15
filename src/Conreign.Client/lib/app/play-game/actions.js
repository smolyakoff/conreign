'use strict';
import {definePromiseAction} from './../core/core';
import {ApiClient} from './../api/api';

const client = new ApiClient();

export const fetchGame = definePromiseAction({
    type: 'FETCH_GAME',
    mapPayload: x => client.getGame(x)
});