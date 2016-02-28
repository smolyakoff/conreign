'use strict';
import * as React from 'react';
import {IndexRoute, Route} from 'react-router';
import {
    Layout,
    GameMenu,
    PlayGamePage
} from 'ui/ui';

export function createRoutes(store) {
    return (
        <Route path="/" component={Layout}>
            <IndexRoute component={GameMenu} />
            <Route path="/game" component={PlayGamePage} />
        </Route>
    );
}

export default createRoutes;