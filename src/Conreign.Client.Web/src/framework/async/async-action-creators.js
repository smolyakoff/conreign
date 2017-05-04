import serializeError from 'serialize-error';
import { get, isObject } from 'lodash';

import { isNonEmptyString } from './../../util';
import AsyncOperationState from './async-operation-state';
import {
  createPendingAsyncActionType,
  createSucceededAsyncActionType,
  createFailedAsyncActionType,
} from './async-action-type-conventions';

export function createPendingAction(originalAction, correlationId) {
  return {
    type: createPendingAsyncActionType(originalAction.type),
    payload: originalAction.payload,
    meta: {
      ...originalAction.meta,
      $async: {
        correlationId,
        originalType: originalAction.type,
        state: AsyncOperationState.Pending,
      },
    },
  };
}

export function createSucceededAction(
  { payload, meta },
  originalAction,
  correlationId,
) {
  return {
    payload,
    originalPayload: originalAction.payload,
    type: createSucceededAsyncActionType(originalAction.type),
    meta: {
      ...originalAction.meta,
      ...meta,
      $async: {
        correlationId,
        originalType: originalAction.type,
        state: AsyncOperationState.Succeeded,
      },
    },
  };
}

export function createFailedAction(error, originalAction, correlationId) {
  return {
    type: createFailedAsyncActionType(originalAction.type),
    payload: serializeError(error),
    originalPayload: originalAction.payload,
    error: true,
    meta: {
      ...originalAction.meta,
      $async: {
        correlationId,
        originalType: originalAction.type,
        state: AsyncOperationState.Succeeded,
      },
    },
  };
}

export function isAsyncAction(action, originalType = null) {
  const asyncMeta = get(action, 'meta.$async');
  if (!isObject(asyncMeta)) {
    return false;
  }
  if (isNonEmptyString(originalType)) {
    return originalType === asyncMeta.originalType;
  }
  return true;
}

export function isCompletedAsyncAction(action, originalType = null) {
  if (!isAsyncAction(action, originalType)) {
    return false;
  }
  const state = action.meta.$async.state;
  return state !== AsyncOperationState.Pending;
}

export function isFailedAsyncAction(action, originalType = null) {
  return action.error &&
    isCompletedAsyncAction(action, originalType);
}

export function isSucceededAsyncAction(action, originalType = null) {
  return !action.error &&
    isCompletedAsyncAction(action, originalType);
}

export function getAsyncOperationCorrelationId(action) {
  return action.meta.$async.correlationId;
}

export function getOriginalPayload(action) {
  return action.originalPayload;
}
