import Rx from 'rxjs';
import { defaults, isObject, get } from 'lodash';

export const AsyncOperationState = {
  Pending: 'PENDING',
  Completed: 'COMPLETED',
  Failed: 'FAILED',
};

export function createPendingAction(action) {
  return Rx.Observable.of({
    type: `${action.type}_${AsyncOperationState.Pending}`,
    meta: {
      ...action.meta,
      $async: {
        originalType: action.type,
        states: AsyncOperationState,
        state: AsyncOperationState.Pending,
      },
    },
  });
}

export function createCompletedAction(result, action) {
  return Rx.Observable.of({
    ...result,
    type: `${action.type}_${AsyncOperationState.Completed}`,
    meta: {
      ...action.meta,
      ...result.meta,
      $async: {
        causeType: action.type,
        states: AsyncOperationState,
        state: AsyncOperationState.Completed,
      },
    },
  });
}

export function createFailedAction(err, action) {
  return Rx.Observable.of({
    type: `${action.type}_${AsyncOperationState.Failed}`,
    payload: err,
    error: true,
    meta: {
      ...action.meta,
      $async: {
        causeType: action.type,
        states: AsyncOperationState,
        state: AsyncOperationState.Failed,
      },
    },
  });
}

export function createAsyncActionTypes(pendingType) {
  return {
    [AsyncOperationState.Pending]: `${pendingType}_${AsyncOperationState.Pending}`,
    [AsyncOperationState.Failed]: `${pendingType}_${AsyncOperationState.Failed}`,
    [AsyncOperationState.Completed]: `${pendingType}_${AsyncOperationState.Completed}`,
  };
}

function toObservable(value) {
  return value instanceof Rx.Observable ? value : Rx.Observable.of(value);
}

export function isAsyncAction(action) {
  return isObject(get(action, 'meta.$async'));
}

export function isAsyncFailureAction(action) {
  return isAsyncAction(action) && action.error;
}

export function asyncDispatcher(dispatch, globalOptions = {}) {
  globalOptions = defaults(globalOptions, {
    createCompletedAction,
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
            const completedAction = options.createCompletedAction(
              result,
              action,
              createCompletedAction,
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
