'use strict';
import {combineReducers} from 'redux';
import {routeReducer as router} from 'react-router-redux'

export const rootReducer = combineReducers({
    router
});

export default rootReducer;