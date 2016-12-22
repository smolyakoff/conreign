'use strict';
import _ from 'lodash';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import {browserHistory} from 'react-router';

import {init} from './../app/app';
import {BrowserStorage} from './browser-storage';
import {Root} from './root';

const history = browserHistory;
const storage = new BrowserStorage(window.localStorage);
const initialState = window.__INITIAL_STATE__;
const initialContext = window.__INITIAL_CONTEXT__;
if (initialContext) {
    _.each(initialContext, (value, key) => {
        storage.setItem(key, value);
    });
}
const app = init(history, storage, initialState);

// Render the React application to the DOM
const props = {
    ...app,
    history
};

ReactDOM.render(
    <Root {...props}/>,
    document.getElementById('root')
);