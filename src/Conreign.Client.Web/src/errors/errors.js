import serializeError from 'serialize-error';
import { negate, set, get, cond, matches } from 'lodash';
import { combineEpics } from 'redux-observable';

import { ValidationErrorCode, UserErrorCategory } from './../api';
import { isFailedAsyncAction, isRouteAction } from '../framework';
import { showNotification } from './../notifications';
import SystemErrorNotification from './system-error-notification';
import ValidationErrorNotification from './validation-error-notification';

const INITIAL_STATE = {
  routingError: null,
};

const SKIP_ERROR_NOTIFICATION_PROP = 'meta.$skipErrorNotification';

export function ignoreErrorActionNotification(action) {
  if (!action.error) {
    return action;
  }
  set(action, SKIP_ERROR_NOTIFICATION_PROP, true);
  return action;
}

function isIgnoredErrorAction(action) {
  const skip = !!get(action, SKIP_ERROR_NOTIFICATION_PROP);
  return skip;
}

function isFailedRouteAsyncAction(action) {
  return isFailedAsyncAction(action) && isRouteAction(action);
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

const ERROR_PAGE_PREFIX = '/$/errors';
export const ERROR_PAGE_PATH = `${ERROR_PAGE_PREFIX}/:statusCode`;

function mapErrorToStatusCode() {
  return 500;
}

function redirectOnRouteError(action$, state, { history }) {
  return action$
    .filter(isFailedRouteAsyncAction)
    .map(action => action.payload)
    .map(mapErrorToStatusCode)
    .do(code => history.push(`${ERROR_PAGE_PREFIX}/${code}`))
    .ignoreElements();
}


const chooseErrorNotificationRenderer = cond([
  [
    matches({
      category: UserErrorCategory.Validation,
      code: ValidationErrorCode.BadInput,
    }),
    () => ValidationErrorNotification.name,
  ],
  [
    () => true,
    () => SystemErrorNotification.name,
  ],
]);

function showErrorNotification(action$) {
  return action$
    .filter(action => action.error)
    .filter(negate(isFailedRouteAsyncAction))
    .filter(negate(isIgnoredErrorAction))
    .map(action => action.payload)
    .map(error => showNotification({
      content: error,
      rendererName: chooseErrorNotificationRenderer(error),
    }));
}

export function selectRoutingError(state) {
  return state.routingError;
}

const epic = combineEpics(redirectOnRouteError, showErrorNotification);

export default {
  reducer,
  epic,
};
