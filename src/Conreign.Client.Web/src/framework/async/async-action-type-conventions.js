import AsyncOperationState from './async-operation-state';

export function createPendingAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Pending}`;
}

export function createSucceededAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Succeeded}`;
}

export function createFailedAsyncActionType(originalType) {
  return `${originalType}_${AsyncOperationState.Failed}`;
}

export function createAsyncActionTypes(originalType) {
  return {
    [AsyncOperationState.Pending]: createPendingAsyncActionType(originalType),
    [AsyncOperationState.Succeeded]: createSucceededAsyncActionType(originalType),
    [AsyncOperationState.Failed]: createFailedAsyncActionType(originalType),
  };
}
