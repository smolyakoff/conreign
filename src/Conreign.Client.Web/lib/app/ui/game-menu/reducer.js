'use strict';
import {handleActions} from 'core/core';
import {arrive} from './actions';

export const menu = handleActions([
    [arrive, (state, action) => {
        return {
            ...state,
            ...action.payload
        };
    }]
], {
    playerName: null,
    galaxyName: null
});