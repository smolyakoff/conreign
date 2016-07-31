'use strict';
import * as React from 'react';
import {IndexRoute, Route} from 'react-router';
import {
    Layout,
    GameMenu,
    Room
} from 'ui/ui';

export function createRoutes(store) {
    return (
        <Route path="/" component={Layout}>
            <IndexRoute component={GameMenu} />
            <Route path="/:game" component={Room} />
        </Route>
    );
}

export default createRoutes;