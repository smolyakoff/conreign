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
    try {
      return isString(json) ? JSON.parse(json) : undefined;
    } catch (e) {
      // eslint-disable-next-line no-console
      console.warn(`[conreign/core/value-store] Failed to parse ${this._options.key}`);
      return undefined;
    }
  }
  set(value) {
    if (isUndefined(value)) {
      this._storage.removeItem(this._options.key);
      return this;
    }
    const json = JSON.stringify(value);
    this._storage.setItem(this._options.key, json);
    return this;
  }
}
