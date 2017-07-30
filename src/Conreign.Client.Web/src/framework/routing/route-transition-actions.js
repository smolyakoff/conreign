
import { isArray, get } from 'lodash';

export const EXECUTE_ROUTE_TRANSITION = 'EXECUTE_ROUTE_TRANSITION';
export const BEGIN_ROUTE_TRANSITION = 'BEGIN_ROUTE_TRANSITION';
export const END_ROUTE_TRANSITION = 'END_ROUTE_TRANSITION';

function mapToRouteAction(actionOrActions) {
  function mapSingleAction(action) {
    return {
      ...action,
      meta: {
        ...action.meta,
        $route: true,
      },
    };
  }
  return isArray(actionOrActions)
    ? actionOrActions.map(mapSingleAction)
    : mapSingleAction(actionOrActions);
}

export function isRouteAction(action) {
  const isRoute = get(action, 'meta.$route');
  return isRoute;
}

export function executeRouteTransition(actionOrActions) {
  const actions = isArray(actionOrActions) ? actionOrActions : [actionOrActions];
  const routeActions = actions.map(mapToRouteAction);
  return {
    type: EXECUTE_ROUTE_TRANSITION,
    payload: {
      actions: routeActions,
    },
  };
}

export function beginRouteTransition() {
  return { type: BEGIN_ROUTE_TRANSITION };
}

export function endRouteTransition() {
  return { type: END_ROUTE_TRANSITION };
}
