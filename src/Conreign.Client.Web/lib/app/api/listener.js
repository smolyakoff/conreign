'use strict';
import Promise from 'bluebird';
import {Subject} from '@reactivex/rxjs';

const CONNECT_INTERVAL = 2000;

export class Listener {
    constructor(authenticator, options) {
        this._connectionTimer = null;
        this._subject = new Subject();
        this._authenticator = authenticator;
        this._options = options;
    }
    listen() {
        return this._initializeConnection().then(connection => this._connect(connection));
    }
    _connect(connection) {
        const hub = connection.createHubProxy('eventHub');
        hub.on('handleEvent', (e) => this._subject.next(e));
        this._connectionTimer = setInterval(() => {
            this._tryConnect(connection)
                .then(() => {
                    clearInterval(this._connectionTimer);
                    this._connectionTimer = null;
                })
        }, CONNECT_INTERVAL);
        return this._subject.asObservable();
    }
    _tryConnect(connection) {
        const $ = require('jquery');
        return this._authenticator.authenticate()
            .then(auth => {
                $(document).ajaxSend((event, jqxhr) => {
                    const header = `Bearer ${auth.accessToken}`;
                    jqxhr.setRequestHeader('Authorization', header);
                });
                return new Promise((resolve, reject) => connection.start().done(resolve).fail(reject));
            })
            .then(() => this._subject.next({type: 'CONNECTION_ESTABLISHED'}))
            .catch(e => {
                this._subject.error({
                    type: 'CONNECTION_ERROR',
                    payload: e,
                    error: true
                });
                return Promise.reject(e);
            });
    }
    _initializeConnection() {
        const $ = require('jquery');
        require('./signalr');
        const signalr = $.hubConnection;
        const url = `${this._options.apiUrl}/events`;
        const connection = signalr(url, { useDefaultPath: false });
        return Promise.resolve(connection);
    }
}

export default Listener;