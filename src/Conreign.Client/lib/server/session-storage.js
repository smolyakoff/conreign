'use strict';
const _ = require('lodash');

export class SessionStorage {
    constructor(session) {
        this._session = session;
    }
    setItem(key, value) {
        this._ensureKeyNotEmpty(key);
        this._session[key] = this._serialize(value);
        return value;
    }
    getItem(key) {
        this._ensureKeyNotEmpty(key);
        const value = this._session[key];
        if (!value) {
            return value;
        }
        return this._deserialize(value);
    }
    removeItem(key) {
        const item = this.getItem(key);
        delete this._session[key];
        return item;
    }
    _ensureKeyNotEmpty(key) {
        if (!_.isString(key) || key.length === 0) {
            throw new Error('Key should not be empty!');
        }
    }
    _serialize(value) {
        return _.cloneDeep(value);
    }
    _deserialize(value) {
        return _.cloneDeep(value);
    }
}

export default SessionStorage;