'use strict';
import defaults from 'lodash/defaults';
import Promise from 'bluebird';
import fetch from 'isomorphic-fetch';
import jwtDecode from 'jwt-decode';

export class Authenticator {
    constructor(storage, options) {
        this._storage = storage;
        this._options = defaults(options, {
            tokenKey: 'conreign.token'
        });
    }
    authenticate() {
        return Promise.try(() => {
            const key = this._options.tokenKey;
            const token = this._storage.getItem(key);
            if (token) {
                return {accessToken: token.value}
            }
            const url = `${this._options.apiUrl}/dispatch`;
            const action = {type: 'arrive'};
            return fetch(url, {
                method: 'POST',
                body: JSON.stringify(action),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            })
                .then(r => r.json())
                .then(d => {
                    if (!d.accessToken) {
                        throw new Error('No access token in response!');
                    }
                    const data = jwtDecode(d.accessToken);
                    const token = {
                        value: d.accessToken,
                        data: data
                    };
                    this._storage.setItem(key, token);
                    return {accessToken: token.value};
                })
        });
    }
}

export default Authenticator;