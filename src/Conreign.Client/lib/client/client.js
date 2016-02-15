'use strict';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import {browserHistory} from 'react-router';

import {init} from './../app/app';
import {Root} from './root';

const history = browserHistory;
const initialState = window.__INITIAL_STATE__;
const app = init(history, initialState);

// Render the React application to the DOM
const props = {
    ...app,
    history
};

ReactDOM.render(
    <Root {...props}/>,
    document.getElementById('root')
);