'use strict';
import * as React from 'react';
import {IndexRoute, Route} from 'react-router';
import {App} from './app';
import {StartGamePage} from './start-game/start-game';
import {PlayGamePage} from './play-game/play-game';

export function createRoutes(store) {
    return (
        <Route path="/" component={App}>
            <IndexRoute component={StartGamePage} />
            <Route path="/game" component={PlayGamePage} />
        </Route>
    );
}

export default createRoutes;