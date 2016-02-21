'use strict';
import Promise from 'bluebird';
import {Subject} from '@reactivex/rxjs';
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