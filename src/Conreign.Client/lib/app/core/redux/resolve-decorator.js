'use strict';
import isFunction from 'lodash/isFunction';

export function resolve(resolution) {
    if (!isFunction) {
        throw new Error(`Resolution should be a function which returns promise.`);
    }
    return function decorate(Component) {
        Component.prototype.resolve = resolution.bind(Component);
        return Component;
    }
}

export default resolve;