import Rx from 'rxjs';
import { defaults, isObject, get, isString } from 'lodash';

function isNonEmptyString(x) {
  return isString(x) && x.length > 0;
}

export const AsyncOperationState = {
  Pending: 'PENDING',
  Completed: 'COMPLETED',
};

function createPendingAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Pending}`;
}

function createCompletedAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Completed}`;
}

export function createPendingAction(action) {
  return Rx.Observable.of({
    type: createPendingAsyncActionType(action.type),
    meta: {
      ...action.meta,
      $async: {
        originalType: action.type,
        state: AsyncOperationState.Pending,
      },
    },
  });
}

export function createSucceededAction(result, action) {
  return Rx.Observable.of({
    ...result,
    type: createCompletedAsyncActionType(action.type),
    meta: {
      ...action.meta,
      ...result.meta,
      $async: {
        causeType: action.type,
        state: AsyncOperationState.Completed,
      },
    },
  });
}

export function createFailedAction(err, action) {
  return Rx.Observable.of({
    type: createCompletedAsyncActionType(action.type),
    payload: err,
    error: true,
    meta: {
      ...action.meta,
      $async: {
        causeType: action.type,
        state: AsyncOperationState.Completed,
      },
    },
  });
}

export function createAsyncActionTypes(originalType) {
  return {
    [AsyncOperationState.Pending]: createPendingAsyncActionType(originalType),
    [AsyncOperationState.Completed]: createCompletedAsyncActionType(originalType),
  };
}

function toObservable(value) {
  return value instanceof Rx.Observable ? value : Rx.Observable.of(value);
}

export function isAsyncAction(action, originalType = null) {
  return isObject(get(action, 'meta.$async')) &&
    (!isNonEmptyString(originalType) ||
      action.type === createPendingAsyncActionType(originalType) ||
      action.type === createCompletedAsyncActionType(originalType)
    );
}

export function isCompletedAsyncAction(action, originalType = null) {
  return isAsyncAction(action) &&
    action.meta.$async.state === AsyncOperationState.Completed &&
    (!isNonEmptyString(originalType) ||
     action.type === createCompletedAsyncActionType(originalType));
}

export function isFailedAsyncAction(action, originalType = null) {
  return isCompletedAsyncAction(action, originalType) && action.error;
}

export function isSucceededAsyncAction(action, originalType = null) {
  return isCompletedAsyncAction(action, originalType) && !action.error;
}

export function asyncDispatcher(dispatch, globalOptions = {}) {
  globalOptions = defaults(globalOptions, {
    createSucceededAction,
    createFailedAction,
    createPendingAction,
  });
  function dispatchWithStates(action, options = {}) {
    options = defaults(options, globalOptions);
    const pendingAction = toObservable(options.createPendingAction(action));
    return pendingAction
      .concat(
        Rx.Observable.from(dispatch(action))
          .concatMap((result) => {
            const completedAction = options.createSucceededAction(
              result,
              action,
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
