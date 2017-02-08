import Rx from 'rxjs';
import { defaults, isObject, get, isString, uniqueId, invert } from 'lodash';
import serializeError from 'serialize-error';

function isNonEmptyString(x) {
  return isString(x) && x.length > 0;
}

export const AsyncOperationState = {
  Pending: 'PENDING',
  Succeeded: 'SUCCEEDED',
  Failed: 'FAILED',
};

function generateCorrelationId() {
  return uniqueId('async-op-');
}

function createPendingAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Pending}`;
}

function createSucceededAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Succeeded}`;
}

function createFailedAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Failed}`;
}

export function createPendingAction(action, correlationId) {
  return Rx.Observable.of({
    type: createPendingAsyncActionType(action.type),
    meta: {
      ...action.meta,
      $async: {
        correlationId,
        originalType: action.type,
        state: AsyncOperationState.Pending,
      },
    },
  });
}

export function createSucceededAction(result, originalAction, correlationId) {
  return Rx.Observable.of({
    ...result,
    type: createSucceededAsyncActionType(originalAction.type),
    meta: {
      ...originalAction.meta,
      ...result.meta,
      $async: {
        correlationId,
        causeType: originalAction.type,
        state: AsyncOperationState.Succeeded,
      },
    },
  });
}

export function createFailedAction(error, originalAction, correlationId) {
  return Rx.Observable.of({
    type: createFailedAsyncActionType(originalAction.type),
    payload: serializeError(error),
    error: true,
    meta: {
      ...originalAction.meta,
      $async: {
        correlationId,
        causeType: originalAction.type,
        state: AsyncOperationState.Succeeded,
      },
    },
  });
}

export function createAsyncActionTypes(originalType) {
  return {
    [AsyncOperationState.Pending]: createPendingAsyncActionType(originalType),
    [AsyncOperationState.Succeeded]: createSucceededAsyncActionType(originalType),
    [AsyncOperationState.Failed]: createFailedAsyncActionType(originalType),
  };
}

function toObservable(value) {
  return value instanceof Rx.Observable ? value : Rx.Observable.of(value);
}

export function isAsyncAction(action, originalType = null) {
  if (!isObject(get(action, 'meta.$async'))) {
    return false;
  }
  if (isNonEmptyString(originalType)) {
    const derivedTypes = invert(createAsyncActionTypes(originalType));
    return isNonEmptyString(derivedTypes[action.type]);
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

export function asyncDispatcher(dispatch, globalOptions = {}) {
  globalOptions = defaults(globalOptions, {
    createSucceededAction,
    createFailedAction,
    createPendingAction,
  });
  function dispatchWithStates(action, options = {}) {
    options = defaults(options, globalOptions);
    const correlationId = generateCorrelationId();
    const pendingAction = toObservable(options.createPendingAction(action, correlationId));
    return pendingAction
      .concat(
        Rx.Observable.from(dispatch(action))
          .concatMap((result) => {
            const completedAction = options.createSucceededAction(
              result,
              action,
              correlationId,
              createSucceededAction,
            );
            if (!completedAction) {
              return Rx.Observable.empty();
            }
            return completedAction;
          })
          .catch((err) => {
            const failedAction = options.createFailedAction(
              err,
              action,
              correlationId,
              createFailedAction,
            );
            if (!failedAction) {
              return Rx.Observable.empty();
            }
            return failedAction;
          }),
      );
  }
  dispatchWithStates.AsyncOperationState = AsyncOperationState;
  return dispatchWithStates;
}

export default asyncDispatcher;
