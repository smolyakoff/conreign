'use strict';
import {ErrorPolicy} from './error-policy';

export const ignoreError = ErrorPolicy.create(e => {});

export const notifyError = ErrorPolicy.create(e => {
    // TODO: transform error if needed
    if (BROWSER) {
        const toastr = require('toastr');
        toastr.error(e.message);
    }
    console.error('Unhandled error: ', e);
});

export default {
    ignoreError: ignoreError,
    notifyError: notifyError
};