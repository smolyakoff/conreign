import { isArray, get, keyBy, mapValues, isFunction, isEqual } from 'lodash';
import { matchRoutes } from 'react-router-config';

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
  return !!isRoute;
}

export function executeRouteTransition(routes, state, currentLocation, previousLocation) {
  const currentRouteMatches = matchRoutes(routes, currentLocation.pathname);
  const previousRouteMatches = previousLocation
    ? matchRoutes(routes, previousLocation.pathname)
    : [];
  const previousMatchesByPath = mapValues(
    keyBy(previousRouteMatches, x => x.match.path),
    x => x.match,
  );
  const initializers = currentRouteMatches
    .map(({ route, match }) => ({
      match,
      init: route.component.init,
    }))
    .filter(({ init }) => isFunction(init))
    .filter(({ match: { path, params: currentParams } }) => {
      const { params: previousParams } = previousMatchesByPath[path] || { params: null };
      const previousRouteParams = {
        params: previousParams,
        search: previousLocation ? previousLocation.search : null,
      };
      const currentRouteParams = {
        params: currentParams,
        search: currentLocation.search,
      };
      return !isEqual(previousRouteParams, currentRouteParams);
    });
  const actions = initializers.map(({ init, match }) => init(match, state));
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
