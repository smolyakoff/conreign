'use strict';
import Promise from 'bluebird';
import {Subject} from '@reactivex/rxjs';
import fetch from 'isomorphic-fetch';
import {signalr} from './lib/lib';

const stream = new Subject();
const connection = signalr(`${CONFIG.API_URL}`);
const hub = connection.createHubProxy('actionHub');
hub.on('handleEvent', (e) => stream.next(e));

function connect() {
    return new Promise((resolve, reject) => {
        connection.start()
            .done(() => {
                debugger;
                resolve();
                stream.next({type: 'CONNECTED'});
            })
            .fail(e => {
                reject(e);
                stream.error(new Error('Could not connect.'));
            });
    });
}

const API_CLIENT_STATE = {
    INITIALIZED: 'INITIALIZED',
    AUTHENTICATED: 'AUTHENTICATED',
    CONNECTED: 'CONNECTED'
};

const TOKEN_STORAGE_KEY = 'Conreign.Auth';

export class ApiClient {
    constructor(options) {
        this._storage = options.storage;
        this._accessToken = null;
        this._state = API_CLIENT_STATE.INITIALIZED;
        this._subject = new Subject();
    }
    get stream() {
        return this._subject.asObservable();
    }
    login(credentials) {
    }
    dispatch(message) {
        return fetch()
    }
    _ensureAuthenticated() {

    }
    _authenticate() {
    }
    _startConnectTimer() {
    }
    _ensureConnected() {
        if (this._state == API_CLIENT_STATE.CONNECTED) {
            return Promise.resolve();
        }
        return this._ensureAuthenticated()
            .then(auth => {

            })
    }
    _connect() {
    }
}


let connected = false;

function ensureConnected() {
    if (connected) {
        return Promise.resolve();
    } else {
        return connect().then(() => connected = true);
    }
}

ensureConnected().catch(e => console.error(e));

function send(packet) {
    return ensureConnected()
        .then(() => new Promise((resolve, reject) => {
            hub.invoke('dispatch', packet).done(resolve).fail(reject);
        }));
}

export const api = {
    stream: stream.asObservable(),
    send: send
};

export default api;