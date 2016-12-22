'use strict';
import _ from 'lodash';

class ServiceRegistry {
    constructor() {
        this._map = new Map();
    }
    register(tag, instance) {
        const key = this._getKey(tag);
        if (!instance) {
            throw new Error('Could not register empty instance.');
        }
        this._map.set(key, instance);
        return instance;
    }
    resolve(tag) {
        const key = this._getKey(tag);
        const instance = this._map.get(key);
        if (!instance) {
            throw new Error(`Could not resolve service by key: ${key}.`);
        }
        return instance;
    }
    _getKey(tag) {
        if (!tag) {
            throw new Error('Tag is invalid!');
        }
        const key = _.isFunction(tag) ? tag.name : tag.toString();
        if (!_.isString(key) || key.length === 0) {
            throw new Error('Tag should coerce to not empty string!')
        }
        return key;
    }
}

export const defaultServiceRegistry = new ServiceRegistry();

export default defaultServiceRegistry;