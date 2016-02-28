'use strict';
import {combineReducers} from 'redux';
import {routeReducer as router} from 'react-router-redux'

import {
    menu
} from './ui/ui';

export const rootReducer = combineReducers({
    router,
    menu
});

export default rootReducer;