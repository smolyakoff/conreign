import serializeError from 'serialize-error';
import { parseInt, negate } from 'lodash';
import { combineEpics } from 'redux-observable';

import { isFailedAsyncAction } from './../core';
import { isRouteLoadingAction } from './../root';
import { showNotification } from './../notifications';

const INITIAL_STATE = {
  routingError: null,
};

function isFailedRouteAsyncAction(action) {
  return isFailedAsyncAction(action) && isRouteLoadingAction(action);
}

function reducer(state = INITIAL_STATE, action) {
  if (isFailedRouteAsyncAction(action)) {
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
      .filter(isFailedRouteAsyncAction)
      .map(action => action.payload)
      .map(mapErrorToStatusCode)
      .do(code => history.push(`${ERROR_PAGE_PREFIX}/${code}`))
      .ignoreElements();
  }

  function showErrorNotificationEpic(action$) {
    return action$
      .filter(action => action.error)
      .filter(negate(isFailedRouteAsyncAction))
      .map(action => action.payload)
      .map(error => showNotification({
        content: error,
        rendererName: 'ErrorNotification',
      }));
  }

  return combineEpics(
    redirectOnRouteErrorEpic,
    showErrorNotificationEpic,
  );
}

export default {
  reducer,
  createEpic,
};
