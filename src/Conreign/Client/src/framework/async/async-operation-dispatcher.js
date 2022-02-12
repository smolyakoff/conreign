import createAsyncOperationObservable from './create-async-operation-observable';

export default function createAsyncOperationDispatcher(dispatch) {
  function dispatchAsObservable(originalAction) {
    return createAsyncOperationObservable(originalAction, dispatch);
  }
  return dispatchAsObservable;
}
