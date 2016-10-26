'use strict';
import _ from 'lodash';

import {Authenticator} from './authenticator';
import {Dispatcher} from './dispatcher';
import {Listener} from './listener';

export class ApiClient {
    constructor(storage, options) {
        options = _.defaults(options, {
            storagePrefix: 'conreign.'
        });
        const authenticator = new Authenticator(storage, options);
        this._listener = new Listener(authenticator, options);
        this._dispatcher = new Dispatcher(authenticator, options);
    }
    dispatch(action) {
        if (!action) {
            throw new Error('Action could not be empty.');
        }
        return this._dispatcher.dispatch(action);
    }
    get events() {
        return this._listener.listen();
    }
}