'use strict';
import Promise from 'promise';
import isFunction from 'lodash/isFunction';
import partial from 'lodash/partial';

export function resolve(resolution) {
    if (!isFunction) {
        throw new Error(`Resolution should be a function.`);
    }
    return function decorate(Component) {
        Component.prototype.resolve = function() {
            const self = this;
            const func = partial(resolution, self.props).bind(self);
            return Promise.try(func);
        };
        return Component;
    }
}

export default resolve;