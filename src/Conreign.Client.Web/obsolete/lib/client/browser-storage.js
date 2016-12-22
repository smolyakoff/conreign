'use strict';
import _ from 'lodash';

export class BrowserStorage {
    constructor(storage) {
        this._storage = storage;
    }
    setItem(key, value) {
        this._ensureKeyNotEmpty(key);
        this._storage.setItem(key, this._serialize(value));
        return value;
    }
    getItem(key) {
        this._ensureKeyNotEmpty(key);
        const value = this._storage.getItem(key);
        if (!value) {
            return value;
        }
        return this._deserialize(value);
    }
    removeItem(key) {
        const item = this.getItem(key);
        this._storage.removeItem(key);
        return item;
    }
    _ensureKeyNotEmpty(key) {
        if (!_.isString(key) || key.length === 0) {
            throw new Error('Key should not be empty!');
        }
    }
    _serialize(value) {
        return JSON.stringify(value);
    }
    _deserialize(value) {
        return JSON.parse(value);
    }
}