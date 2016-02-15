'use strict';
import extend from 'lodash/extend';
import {handleActions} from './../core/core';
import {fetchGame} from './actions';

export const gameReducer = handleActions([
    [fetchGame, {
        success: (state, action) => {
            return extend({}, state || {}, {text: action.payload});
        }
    }]
]);