import { isAsyncAction, isCompletedAsyncAction } from './async-actions';

export default function countAsyncActions(state = 0, action) {
  if (!isAsyncAction(action)) {
    return state;
  }
  return isCompletedAsyncAction(action)
    ? Math.max(state - 1, 0)
    : state + 1;
}
