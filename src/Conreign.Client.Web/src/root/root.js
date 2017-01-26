import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { snakeCase, get } from 'lodash';

import { AsyncOperationState } from './../core';
import errors from './../errors';
import auth from './../auth';
import home from './../home';
import room from './../room';

const LISTEN_FOR_SERVER_EVENTS = 'LISTEN_FOR_SERVER_EVENTS';
const EXECUTE_ROUTE_ACTIONS = 'EXECUTE_ROUTE_ACTIONS';
const START_ROUTE_TRANSACTION = 'START_ROUTE_TRANSACTION';
const END_ROUTE_TRANSACTION = 'END_ROUTE_TRANSACTION';

export function listenForServerEvents() {
  return { type: LISTEN_FOR_SERVER_EVENTS };
}

export function startRouteTransaction() {
  return { type: START_ROUTE_TRANSACTION };
}

export function endRouteTransaction() {
  return { type: END_ROUTE_TRANSACTION };
}

export function executeRouteActions(actions) {
  return {
    type: EXECUTE_ROUTE_ACTIONS,
    payload: actions,
  };
}

const INITIAL_OPERATIONS_STATE = {
  routePending: 0,
  totalPending: 0,
};

function positiveOrZero(value) {
  return value > 0 ? value : 0;
}

export function isRouteLoadingAction(action) {
  return get(action, 'meta.$route');
}

function operationsReducer(state = INITIAL_OPERATIONS_STATE, action) {
  switch (action.type) {
    case START_ROUTE_TRANSACTION:
      return {
        ...state,
        routePending: state.routePending + 1,
      };
    case END_ROUTE_TRANSACTION:
      return {
        ...state,
        routePending: positiveOrZero(state.routePending - 1),
      };
    default: {
      const asyncState = get(action, 'meta.$async.state');
      const isRouteAction = isRouteLoadingAction(action);
      switch (asyncState) {
        case AsyncOperationState.Pending:
          return {
            ...state,
            routePending: isRouteAction
              ? state.routePending + 1
              : state.routePending,
            totalPending: state.totalPending + 1,
          };
        case AsyncOperationState.Completed:
        case AsyncOperationState.Failed:
          return {
            ...state,
            routePending: isRouteAction
              ? positiveOrZero(state.routePending - 1)
              : state.routePending,
            totalPending: positiveOrZero(state.totalPending - 1),
          };
        default:
          return state;
      }
    }
  }
}

const reducer = combineReducers({
  operations: operationsReducer,
  [auth.reducer.$key]: auth.reducer,
  [errors.reducer.$key]: errors.reducer,
  [room.reducer.$key]: room.reducer,
});

function createEpic(container) {
  const { apiClient } = container;

  function listenForServerEventsEpic(action$) {
    return action$
      .ofType(LISTEN_FOR_SERVER_EVENTS)
      .mergeMap(() => apiClient.events)
      .map(event => ({
        ...event,
        type: `HANDLE_${snakeCase(event.type).toUpperCase()}`,
        meta: {
          ...event.meta,
          $event: true,
        },
      }));
  }

  function executeRouteActionsEpic(action$) {
    return action$
      .ofType(EXECUTE_ROUTE_ACTIONS)
      .flatMap(({ payload }) => payload)
      .map(action => ({
        ...action,
        meta: {
          ...action.meta,
          $route: true,
        },
      }));
  }

  return combineEpics(
    listenForServerEventsEpic,
    executeRouteActionsEpic,
    errors.createEpic(container),
    auth.createEpic(container),
    home.createEpic(container),
    room.createEpic(container),
  );
}

export default {
  createEpic,
  reducer,
};
