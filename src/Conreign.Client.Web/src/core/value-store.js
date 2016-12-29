import { defaults, isString, isUndefined } from 'lodash';

export default class ValueStore {
  constructor(storage, options) {
    this._storage = storage;
    this._options = defaults(options || {}, {
      key: null,
    });
  }
  get() {
    const json = this._storage.getItem(this._options.key);
    return isString(json) ? JSON.parse(json) : undefined;
  }
  set(value) {
    const json = isUndefined(value) ? value : JSON.stringify(value);
    this._storage.setItem(this._options.key, json);
    return this;
  }
}
