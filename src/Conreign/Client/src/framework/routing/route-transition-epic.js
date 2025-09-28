import Rx from 'rxjs';
import { isArray } from 'lodash';

import {
  EXECUTE_ROUTE_TRANSITION,
  beginRouteTransition,
  endRouteTransition,
  isRouteAction,
} from './route-transition-actions';
import {
  isAsyncAction,
  isCompletedAsyncAction,
} from './../async';

// This file contains pure magic
// I don't really understand how, but it works

export default function transitionRoute(action$) {
  let pendingRouteActions = 0;
  action$
    .filter(isRouteAction)
    .filter(isAsyncAction)
    .map(action => isCompletedAsyncAction(action) ? -1 : 1)
    .scan((total, value) => value + total, 0)
    .subscribe((value) => {
      pendingRouteActions = value;
    });

  const transitionEnd$ = action$
    .startWith({ type: 'DUMMY' })
    .takeWhile(() => pendingRouteActions > 0)
    .ignoreElements();

  return action$
    .ofType(EXECUTE_ROUTE_TRANSITION)
    .mergeMap(({ payload: { actions } }) => {
      if (actions.length === 0) {
        return Rx.Observable.empty();
      }
      const epics = actions
        .map(x => isArray(x) ? x : [x])
        .map(x => Rx.Observable.concat(
          Rx.Observable.from(x),
          transitionEnd$,
        ));
      return Rx.Observable.concat(
        Rx.Observable.of(beginRouteTransition()),
        ...epics,
        Rx.Observable.of(endRouteTransition()),
      );
    });
}
