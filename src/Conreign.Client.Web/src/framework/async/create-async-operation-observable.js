import Rx from 'rxjs';
import { uniqueId } from 'lodash';

import {
  createPendingAction,
  createSucceededAction,
  createFailedAction,
} from './async-actions';

function generateCorrelationId() {
  return uniqueId('async-operation-');
}

export default function createAsyncOperationObservable(action, dispatch) {
  const id = generateCorrelationId();
  return Rx.Observable.from(dispatch(action))
    .map(result => createSucceededAction(result, action, id))
    .catch(error => Rx.Observable.of(createFailedAction(error, action, id)))
    .startWith(createPendingAction(action, id));
}
