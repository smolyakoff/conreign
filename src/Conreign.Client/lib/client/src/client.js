'use strict';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import {browserHistory} from 'react-router';

import {createReduxStore} from './boot';
import {createRoutes, Window} from './ui/ui';

const history = browserHistory;
const initialState = window.__INITIAL_STATE__;
const store = createReduxStore(history, initialState);
const routes = createRoutes(store);

// Render the React application to the DOM
ReactDOM.render(
    <Window history={history} routes={routes} store={store}/>,
    document.getElementById('root')
);