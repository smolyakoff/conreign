import Rx from 'rxjs';

import {
  createPendingAction,
  createSucceededAction,
  createFailedAction,
} from './async-action-creators';

export default function createAsyncOperationObservable(action, dispatch) {
  return Rx.Observable.from(dispatch(action))
    .map(result => createSucceededAction(result, action))
    .catch(error => Rx.Observable.of(createFailedAction(error, action)))
    .startWith(createPendingAction(action));
}
