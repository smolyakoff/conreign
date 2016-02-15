'use strict';
import Promise from 'bluebird';

import {reportError} from './actions';

export const errorReporter = store => next => action => {
    try {
        return next(action);
    } catch(err) {
        console.error('Caught an error!', err);
        //return next(reportError(err));
    }
};

export default errorReporter;