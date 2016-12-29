import { routerReducer } from 'react-router-redux';
import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';

import login from './../login';
import home from './../home';

const CONNECT_TO_SERVER = 'CONNECT_TO_SERVER';
export const CONNECT_TO_SERVER_COMPLETED = 'CONNECT_TO_SERVER_COMPLETED';

function notifyServerConnected() {
  return { type: CONNECT_TO_SERVER_COMPLETED };
}

export function connectToServer() {
  return { type: CONNECT_TO_SERVER };
}

const reducer = combineReducers({
  routing: routerReducer,
});

function createEpic(container) {
  const { apiClient } = container;
  function connectEpic(action$) {
    return action$
      .ofType(CONNECT_TO_SERVER)
      .mergeMap(() => apiClient.connect())
      .mapTo(notifyServerConnected())
      .merge(apiClient.events);
  }
  return combineEpics(
    connectEpic,
    login.createEpic(container),
    home.createEpic(container),
  );
}

export default {
  createEpic,
  reducer,
};
