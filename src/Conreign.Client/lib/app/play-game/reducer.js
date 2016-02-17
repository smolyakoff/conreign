'use strict';
import extend from 'lodash/extend';
import {handleActions} from './../core/core';
import {fetchGame} from './actions';

export const gameReducer = handleActions([
    [fetchGame, {
        success: (state, action) => {
            return extend({}, state || {}, {data: action.payload});
        },
        error: (state, action) => {
            if (action.payload.status !== 400) {
                // ideally, here I would like to notify my global error handler
                return state;
            }
            return extend({}, state, {formError: action.payload.message});
        }
    }]
]);