'use strict';
import _ from 'lodash';

const DEFAULT_MESSAGE = 'Unexpected error occurred.';

export class AppError extends Error {
    constructor(info, inner) {
        const message = _.isString(info)
            ? info
            : _.isObject(info) ? info.message || DEFAULT_MESSAGE : DEFAULT_MESSAGE;
        super(message);

        Object.defineProperty(this, 'message', {
            enumerable: false,
            value: message
        });
        Object.defineProperty(this, 'name', {
            enumerable : false,
            value : this.constructor.name
        });
        if (Error.hasOwnProperty('captureStackTrace')) {
            Error.captureStackTrace(this, this.constructor);
            return;
        }
        Object.defineProperty(this, 'stack', {
            enumerable : false,
            value : (new Error(message)).stack
        });

        if (inner) {
            Object.defineProperty(this, 'inner', {
                enumerable : false,
                value : inner
            });
        }

        if (info) {
            _.each(info, (value, key) => {
                if (key === 'message' || key === 'name') {
                    return;
                }
                Object.defineProperty(this, key, {
                    enumerable: true,
                    value: value
                });
            })
        }
    }
    toString() {
        const base = Error.prototype.toString.bind(this)();
        const messages = [base];
        if (this.inner) {
            messages.push([
                '------------------inner-------------------',
                this.inner.toString()
            ])
        }
        return messages.join('\n');
    }
}

export class UserError extends AppError {
    constructor(info, inner) {
        super(info, inner);
    }
}

export class SystemError extends AppError {
    constructor(info, inner) {
        super(info, inner);
    }
}