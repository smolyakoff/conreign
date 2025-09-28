import { combineEpics } from 'redux-observable';

import { createEventAction, routeTransitionEpic } from './../framework';
import errors from './../errors';
import auth from './../auth';
import home from './../home';
import room from './../room';

function listenForServerEvents(action$, store, { apiClient }) {
  return apiClient.events.map(createEventAction);
}

export default combineEpics(
  listenForServerEvents,
  routeTransitionEpic,
  errors.epic,
  auth.epic,
  home.epic,
  room.epic,
);
