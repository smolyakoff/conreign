'use strict';
import isFunction from 'lodash/isFunction';
import every from 'lodash/every';
import find from 'lodash/find';
import last from 'lodash/last';
import dropRight from 'lodash/dropRight';

function rethrow(e) {
    throw e;
}

function isErrorConstructor(f) {
    return f === Error && f.prototype instanceof Error
}

export class ErrorPolicy {
    static create(defaultHandler = rethrow) {
        return new ErrorPolicy(defaultHandler);
    }
    static resolve(errorPolicy) {
        return errorPolicy instanceof ErrorPolicy ? errorPolicy.build() : errorPolicy;
    }
    constructor(defaultHandler = rethrow) {
        this._registry = [{predicate: e => true, handler: defaultHandler}];
    }
    when(errorClassOrPredicate, predicateOrAction, action) {
        const policy = new ErrorPolicy();
        let predicates = [];
        let handler = null;
        if (isFunction(action)) {
            predicates = [
                e => e instanceof errorClassOrPredicate,
                predicateOrAction
            ];
            handler = action;
        } else if (isErrorConstructor(errorClassOrPredicate)) {
            predicates = [e => e instanceof errorClassOrPredicate];
            handler = predicateOrAction;
        } else {
            predicates = [errorClassOrPredicate];
            handler = predicateOrAction;
        }
        const predicate = e => every(predicates, p => p(e));
        policy._registry = [{predicate: predicate, handler: handler}].concat(this._registry);
        return policy;
    }
    join(errorPolicy) {
        if (!errorPolicy) {
            throw new Error('Error policy argument should be an error policy instance or error policy modification function.');
        }
        if (!(errorPolicy instanceof ErrorPolicy)) {
            return errorPolicy(this);
        }
        const policy = new ErrorPolicy();
        const items = dropRight(errorPolicy._registry);
        policy._registry = items.concat(policy._registry);
        return policy;
    }
    build() {
        return (e) => {
            const registryItem = find(this._registry, h => h.predicate(e));
            const defaultHandler = last(this._registry).handler;
            if (!registryItem) {
                return defaultHandler(e);
            }
            const handler = registryItem.handler;
            try {
                return handler(e);
            } catch(e) {
                return defaultHandler(e);
            }
        };
    }
}

export default ErrorPolicy;