import serializeError from 'serialize-error';
import { parseInt } from 'lodash';

import { isAsyncFailureAction } from './../core';
import { isRouteLoadingAction } from './../root';

const INITIAL_STATE = {
  routingError: null,
};

function isRouteFailureAction(action) {
  return isAsyncFailureAction(action) && isRouteLoadingAction(action);
}

function reducer(state = INITIAL_STATE, action) {
  if (isRouteFailureAction(action)) {
    return {
      ...state,
      routingError: serializeError(action.payload),
    };
  }
  return state;
}
reducer.$key = 'errors';

const ERROR_PAGE_PREFIX = '/errors';
export const ERROR_PAGE_PATH = `${ERROR_PAGE_PREFIX}/:statusCode`;

function mapErrorToStatusCode() {
  return 500;
}

const DUMMY_ERROR = new Error('Nothing to do here 😉');

export function selectErrorPageProps(state, { params }) {
  const statusCode = parseInt(params.statusCode);
  return {
    statusCode,
    showStack: COMPILATION_MODE === 'debug',
    error: state.errors.routingError || DUMMY_ERROR,
  };
}

function createEpic({ history }) {
  function redirectOnRouteErrorEpic(action$) {
    return action$
      .filter(isRouteFailureAction)
      .map(a => a.error)
      .map(mapErrorToStatusCode)
      .do(code => history.push(`${ERROR_PAGE_PREFIX}/${code}`))
      .ignoreElements();
  }
  return redirectOnRouteErrorEpic;
}

export default {
  reducer,
  createEpic,
};
