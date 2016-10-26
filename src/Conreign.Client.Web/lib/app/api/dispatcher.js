'use strict';
import defaults from 'lodash/defaults';
import merge from 'lodash/merge';
import fetch from 'isomorphic-fetch';

export class Dispatcher {
    constructor(authenticator, options) {
        this._authenticator = authenticator;
        this._options = options;
    }
    dispatch(action) {
        const url = `${this._options.apiUrl}/dispatch`;
        return this._authenticator.authenticate()
            .then(auth => {
                const data = merge({}, action, {meta: {auth: auth}});
                return fetch(url, {
                    method: 'POST',
                    body: JSON.stringify(data),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    }
                });
            })
            .then(r => r.json());
    }
}

export default Dispatcher;