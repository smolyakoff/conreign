'use strict';
import {defineAction} from './../redux/redux';

export const reportError = defineAction({
    type: 'REPORT_ERROR',
    mapPayload: err => ({
        message: err.message,
        title: 'Unexpected error',
        err: err
    })
});