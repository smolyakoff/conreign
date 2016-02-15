'use strict';
import {combineReducers} from 'redux';
import {routeReducer as router} from 'react-router-redux'
import {gameReducer as game} from './play-game/play-game';

export const rootReducer = combineReducers({
    router,
    game
});

export default rootReducer;